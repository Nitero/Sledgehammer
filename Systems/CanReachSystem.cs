using HarmonyLib;
using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    [HarmonyPatch]
    public static class CanReachPatch
    {
        [HarmonyPatch(typeof(GenericSystemBase), "CanReach")]
        [HarmonyPrefix]
        static bool CanReach_Prefix(ref bool __result, Vector3 from, Vector3 to, bool do_not_swap = false)
        {
            if (CanReachSystem.Instance.CanReach(from, to))
            {
                __result = true;
                return false;
            }
            return true;//run original
        }

    }
    //[UpdateAfter(typeof(SLayout))]
    [UpdateAfter(typeof(SKitchenLayout))]
    public class CanReachSystem : RestaurantSystem, IModSystem
    {
        private static CanReachSystem _instance;
        public static CanReachSystem Instance => _instance;

        private EntityQuery replacedWallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            replacedWallQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallReplaced)));
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        public bool CanReach(Vector3 from, Vector3 to)
        {
            using NativeArray<Entity> replacedWalls = replacedWallQuery.ToEntityArray(Allocator.TempJob);

            if (HasSingleton<SIsDayTime>() && CanReachNonHammeredWall(from, to, replacedWalls))//TODO: only if using hammer?
                return true;

            from = from.Rounded();
            to = to.Rounded();

            CLayoutRoomTile tileFrom = GetTile(from);
            CLayoutRoomTile tileTo = GetTile(to);

            if (tileFrom.RoomID == tileTo.RoomID)
                return false;

            return CanReachOverHammeredWall(from, to, replacedWalls);
        }

        private bool CanReachNonHammeredWall(Vector3 from, Vector3 to, NativeArray<Entity> replacedWalls)
        {
            foreach (Entity replacedWall in replacedWalls)
            {
                if (!EntityManager.RequireComponent(replacedWall, out CWallReplaced wallHammered))
                    continue;

                if (wallHammered.HasBeenHammered)
                    continue;

                if (Vector3.Distance(to, wallHammered.WallPosition) < 0.5f)//TODO: find a better solution?
                    return true;
            }
            return false;
        }

        private bool CanReachOverHammeredWall(Vector3 from, Vector3 to, NativeArray<Entity> replacedWalls)
        {
            Vector3 direction = to - from;
            foreach (Entity replacedWall in replacedWalls)
            {
                if (!EntityManager.RequireComponent(replacedWall, out CWallReplaced wallHammered))
                    continue;

                if (!wallHammered.HasBeenHammered)
                    continue;

                if (CanReachOverFromSide(from, wallHammered.SideA, wallHammered.ReachabilitySideA, direction))
                    return true;
                if (CanReachOverFromSide(from, wallHammered.SideB, wallHammered.ReachabilitySideB, direction))
                    return true;
            }
            return false;
        }

        private bool CanReachOverFromSide(Vector3 from, Vector3 side, Reachability wallSideReachability, Vector3 direction)
        {
            bool fromIsSide = Mathf.Approximately(from.x, side.x) && Mathf.Approximately(from.z, side.z);
            if (fromIsSide && wallSideReachability.GetDirectional(direction.x, direction.z))
                return true;
            return false;
        }
    }
}