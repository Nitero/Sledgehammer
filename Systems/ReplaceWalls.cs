using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using KitchenLib.Utils;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class ResetWallsInPractise : StartOfDaySystem, IModSystem
    {
        protected override void OnUpdate()
        {
            ReplaceWalls.Instance?.Reset(); //TODO: only in practise
        }
    }

    public class ResetHammeredWallsInHQ : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            ReplaceWalls.Instance?.Reset();
        }
    }

    //[UpdateAfter(typeof(SLayout))]
    [UpdateAfter(typeof(SKitchenLayout))]
    public class ReplaceWalls : NightSystem, IModSystem //TODO: why doesnt RestaurantInitialisationSystem work? or StartOfNightSystem?
    {
        private static ReplaceWalls _instance;
        public static ReplaceWalls Instance => _instance;

        private EntityQuery replacedWallQuery;

        private bool _didSetup;

        public void Reset()
        {
            _didSetup = false;
        }

        protected override void Initialise()
        {
            base.Initialise();
            replacedWallQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallReplaced)));
            _instance = this;
        }

        protected override void OnUpdate()
        {
            if (_didSetup)
                return;
            _didSetup = true;

            //Debug.Log(GameInfo.CurrentScene);
            //if (GameInfo.CurrentScene == SceneType.Kitchen)//TODO: then doesnt work on franchise, maybe other stuff too?
            //    return;

            using NativeArray<Entity> replacedWalls = replacedWallQuery.ToEntityArray(Allocator.TempJob);
            bool alreadyReplacedWalls = replacedWalls.Length > 0;
            foreach (Entity replacedWall in replacedWalls)
            {
                if (EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced cWallHasBeenReplaced))
                {
                    cWallHasBeenReplaced.HammeringWasAttemptedToday = false;
                    EntityManager.SetComponentData(replacedWall, cWallHasBeenReplaced);
                }
                if (EntityManager.RequireComponent<CTakesDuration>(replacedWall, out CTakesDuration cTakesDuration))
                {
                    cTakesDuration.Manual = false;
                    EntityManager.SetComponentData(replacedWall, cTakesDuration);
                }
            }

            Transform floorplan = GameObject.Find("Kitchen Floorplan(Clone)").transform;
            foreach (Transform child in floorplan.transform)
            {
                if (child.name != "Short Wall Section(Clone)" || !child.gameObject.activeSelf)
                    continue;

                //TODO: make this actually be what side the player made the hatch from
                Vector3 from = new Vector3(Mathf.Floor(child.position.x), 0, Mathf.Floor(child.position.z));
                Vector3 to = new Vector3(Mathf.Ceil(child.position.x), 0, Mathf.Ceil(child.position.z));

                if (!LayoutHelpers.IsInside(GetTile(from).Type))
                    continue;
                if (!LayoutHelpers.IsInside(GetTile(to).Type))
                    continue;

                child.gameObject.SetActive(false);

                if (alreadyReplacedWalls)
                    continue;
                
                
                int wallMaterial = MaterialUtils.GetExistingMaterial("Wall Main").GetInstanceID(); //TODO: get the actual materials of walls when mod was added mid run
                bool isHorizontal = Mathf.Approximately(from.z, to.z);

                Reachability reachabilitySideA = default(Reachability);
                Reachability reachabilitySideB = default(Reachability);
                if(isHorizontal)
                {
                    int direction = (int)Mathf.Sign(to.x - from.x);
                    reachabilitySideA[direction, 1] = true;//useless?
                    reachabilitySideA[direction, 0] = true;
                    reachabilitySideA[direction,-1] = true;//useless?

                    reachabilitySideB[direction * -1, 1] = true;//useless?
                    reachabilitySideB[direction * -1, 0] = true;
                    reachabilitySideB[direction * -1,-1] = true;//useless?
                }
                else
                {
                    int direction = (int)Mathf.Sign(to.z - from.z);
                    reachabilitySideA[ 1, direction] = true;//useless?
                    reachabilitySideA[ 0, direction] = true;
                    reachabilitySideA[-1, direction] = true;//useless?

                    reachabilitySideB[ 1, direction * -1] = true;//useless?
                    reachabilitySideB[ 0, direction * -1] = true;
                    reachabilitySideB[-1, direction * -1] = true;//useless?
                }
                //TODO: diagonals dont work... do they need to start from the adjacent tile too, not just A/B? nvm didnt seem to work either... why works at night?

                Entity entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new CCreateAppliance{ ID = Refs.WallReplaced.ID });
                EntityManager.AddComponentData(entity, new CPosition(child.position, child.rotation));
                EntityManager.AddComponentData(entity, new CFixedRotation());
                EntityManager.AddComponentData(entity, new CWallReplaced(child.position, from, to, GetRoom(from), GetRoom(to), reachabilitySideA, reachabilitySideB, wallMaterial, wallMaterial, false));
            }
        }

        public void Hammered(Entity replacedWall)
        {
            if (!EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced cReplacedWall))
                return;

            cReplacedWall.HasBeenHammered = true;
            EntityManager.SetComponentData(replacedWall, cReplacedWall);
        }

        public bool CanReach(Vector3 from, Vector3 to)
        {
            using NativeArray<Entity> replacedWalls = replacedWallQuery.ToEntityArray(Allocator.TempJob);
            if (CanReachNonHammeredWall(from, to, replacedWalls))
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
                if (!EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced wallHammered))
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
                if (!EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced wallHammered))
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

    [HarmonyPatch]
    public static class CanReachPatch
    {
        [HarmonyPatch(typeof(GenericSystemBase), "CanReach")]
        [HarmonyPrefix]
        static bool CanReach_Prefix(ref bool __result, Vector3 from, Vector3 to, bool do_not_swap = false)
        {
            if (ReplaceWalls.Instance.CanReach(from, to))
            {
                __result = true;
                return false;
            }
            return true;//run original
        }
    }
}