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
                Vector3 from = new Vector3(Mathf.Floor(child.position.x), Mathf.Floor(child.position.y), Mathf.Floor(child.position.z));
                Vector3 to = new Vector3(Mathf.Ceil(child.position.x), Mathf.Ceil(child.position.y), Mathf.Ceil(child.position.z));

                if (!LayoutHelpers.IsInside(GetTile(from).Type))
                    continue;
                if (!LayoutHelpers.IsInside(GetTile(to).Type))
                    continue;

                child.gameObject.SetActive(false);

                if (alreadyReplacedWalls)
                    continue;

                Entity entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new CCreateAppliance{ ID = Refs.WallReplaced.ID });
                EntityManager.AddComponentData(entity, new CPosition(child.position, child.rotation));
                EntityManager.AddComponentData(entity, new CFixedRotation());

                int wallMaterial = MaterialUtils.GetExistingMaterial("Wall Main").GetInstanceID(); //TODO: get the actual materials of walls when mod was added mid run
                EntityManager.AddComponentData(entity, new CWallReplaced(child.position, from, to, GetRoom(from), GetRoom(to), wallMaterial, wallMaterial, false));
            }
        }

        public void Hammered(Entity replacedWall)
        {
            if (!EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced cReplacedWall))
                return;

            cReplacedWall.HasBeenHammered = true;
            EntityManager.SetComponentData(replacedWall, cReplacedWall);
        }

        public bool IsReplacedWallHammeredBetween(Vector3 from, Vector3 to)
        {
            from = new Vector3(Mathf.Round(from.x), 0, Mathf.Round(from.z));
            to = new Vector3(Mathf.Round(to.x), 0, Mathf.Round(to.z));

            using NativeArray<Entity> replacedWalls = replacedWallQuery.ToEntityArray(Allocator.TempJob);
            foreach (Entity replacedWall in replacedWalls)
            {
                if (!EntityManager.RequireComponent<CWallReplaced>(replacedWall, out CWallReplaced cReplacedWall))
                    continue;

                if (!cReplacedWall.HasBeenHammered)
                    continue;

                var sideA = cReplacedWall.SideA;
                var sideB = cReplacedWall.SideB;

                // Check if the two positions are in the same row or column
                if (Mathf.Approximately(from.x, to.x))
                {
                    // Same column, check if the wall is between the two positions horizontally
                    if ((Mathf.Approximately(sideA.x, from.x) && Mathf.Approximately(sideB.x, to.x))
                    || (Mathf.Approximately(sideA.x, to.x) && Mathf.Approximately(sideB.x, from.x)))
                    {
                        if (Mathf.Min(from.z, to.z) <= sideA.z && sideA.z <= Mathf.Max(from.z, to.z))
                            return true;
                    }
                }
                else if (Mathf.Approximately(from.z, to.z))
                {
                    // Same row, check if the wall is between the two positions vertically
                    if ((Mathf.Approximately(sideA.z, from.z) && Mathf.Approximately(sideB.z, to.z))
                    || (Mathf.Approximately(sideA.z, to.z) && Mathf.Approximately(sideB.z, from.z)))
                    {
                        if (Mathf.Min(from.x, to.x) <= sideA.x && sideA.x <= Mathf.Max(from.x, to.x))
                            return true;
                    }
                }
                else
                {
                    // Diagonal match, check if the wall is between the two positions diagonally
                    if ((Mathf.Approximately(sideA.x, from.x) && Mathf.Approximately(sideB.z, to.z))
                    || (Mathf.Approximately(sideA.x, to.x) && Mathf.Approximately(sideB.z, from.z)))
                    {
                        if (Mathf.Min(from.z, to.z) <= sideA.z && sideA.z <= Mathf.Max(from.z, to.z))
                            return true;
                    }
                    else if ((Mathf.Approximately(sideA.z, from.z) && Mathf.Approximately(sideB.x, to.x))
                    || (Mathf.Approximately(sideA.z, to.z) && Mathf.Approximately(sideB.x, from.x)))
                    {
                        if (Mathf.Min(from.x, to.x) <= sideA.x && sideA.x <= Mathf.Max(from.x, to.x))
                            return true;
                    }
                }
            }

            return false;
        }
    }

    [HarmonyPatch]
    public static class CanReachOverHammeredWallPatch
    {
        [HarmonyPatch(typeof(GenericSystemBase), "CanReach")]
        [HarmonyPrefix]
        static bool CanReach_Prefix(ref bool __result, Vector3 from, Vector3 to, bool do_not_swap = false)
        {
            if (ReplaceWalls.Instance.IsReplacedWallHammeredBetween(from, to))
            {
                __result = true;
                return false;
            }
            return true;//run original
        }
    }
}