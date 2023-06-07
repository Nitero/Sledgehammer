using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenSledgehammer
{
    [UpdateBefore(typeof(ItemTransferGroup))]
    public class HammerInteractionSystem : InteractionSystem, IModSystem
    {
        private CTakesDuration duration;
        private CWallReplaced wallReplaced;
        private CSledgehammer sledgeHammer;
        private CToolUser toolUser;
        private EntityQuery sledgehammerQuery;
        protected override bool RequireHold => true;
        protected override bool RequirePress => false;

        protected override void Initialise()
        {
            base.Initialise();
            sledgehammerQuery = GetEntityQuery(new QueryHelper().All(typeof(CSledgehammer)));
        }

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Target, out wallReplaced))
                return false;
            if (wallReplaced.HasBeenHammered)
                return false;
            if (!Require(data.Interactor, out toolUser))
                return false;

            using NativeArray<Entity> sledgehammers = sledgehammerQuery.ToEntityArray(Allocator.TempJob);
            foreach (var sledgehammer in sledgehammers)
            {
                if (sledgehammer == toolUser.CurrentTool)
                    return true;
            }
            return false;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (!Require(data.Target, out wallReplaced))
                return;
            if (!Require(data.Target, out duration))
                return;
            if (!Require(data.Interactor, out toolUser))
                return;
            if (!Require(toolUser.CurrentTool, out sledgeHammer))
                return;

            wallReplaced.HammeringWasAttemptedToday = true;
            EntityManager.SetComponentData(data.Target, wallReplaced);

            duration.Manual = true;
            duration.IsLocked = false;
            duration.Remaining -= 1f * Time.DeltaTime;
            duration.CurrentChange = 1f;
            EntityManager.SetComponentData(data.Target, duration);

            if (duration.Remaining <= 0f && duration.Active)
            {
                EntityManager.AddComponent<CIsInactive>(data.Target);
                //EntityManager.RemoveComponent<CIsInteractive>(data.Target);
                //-> need to use CIsInactive instead of CIsInteractive so that its not highlighted afterwards when looking at other appliances

                wallReplaced.HasBeenHammered = true;
                EntityManager.SetComponentData(data.Target, wallReplaced);

                duration.IsLocked = true;
                duration.Active = false;
                EntityManager.SetComponentData(data.Target, wallReplaced);

                EntityManager.DestroyEntity(toolUser.CurrentTool);
            }
        }
    }
}