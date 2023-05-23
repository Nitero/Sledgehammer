using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    [UpdateBefore(typeof(ItemTransferGroup))]
    public class HammerInteractionSystem : InteractionSystem, IModSystem
    {
        private CTakesDuration duration;
        private CWallReplaced wallReplaced;
        private CToolUser toolUser;
        private EntityQuery sledgehammerQuery;
        protected override bool RequireHold => true;
        protected override bool RequirePress => false;

        protected override void Initialise()
        {
            base.Initialise();
            sledgehammerQuery = GetEntityQuery(new QueryHelper().All(typeof(CItem)));
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
                if (!EntityManager.RequireComponent<CItem>(sledgehammer, out CItem item))
                    continue;

                if (item.ID == Refs.Sledgehammer.ID && sledgehammer == toolUser.CurrentTool)
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

            wallReplaced.HammeringWasAttemptedToday = true;
            EntityManager.SetComponentData(data.Target, wallReplaced);

            duration.Manual = true;
            duration.IsLocked = false;
            duration.Remaining -= 0.1f * 1f;
            duration.CurrentChange = 1f;
            EntityManager.SetComponentData(data.Target, duration);

            if (duration.Remaining <= 0f && duration.Active)
            {
                Debug.Log("Took: " + duration.Remaining);

                EntityManager.AddComponent<CIsInactive>(data.Target);
                wallReplaced.HasBeenHammered = true;
                EntityManager.SetComponentData(data.Target, wallReplaced);

                duration.IsLocked = true;
                duration.Active = false;
                EntityManager.SetComponentData(data.Target, wallReplaced);
            }
        }
    }
}