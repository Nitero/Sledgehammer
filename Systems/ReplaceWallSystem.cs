using System.Linq;
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
    [HarmonyPatch(typeof(LayoutBuilder))]
    public class LayoutBuilder_Patch
    {
        public const string KITCHEN_FLOORPLAN_NAME = "Kitchen Floorplan";
        public const string REPLACE_WALL_NAME = "ReplaceWall";

        [HarmonyPatch("BuildWallBetween")]
        [HarmonyPrefix]
        public static void BuildWallBetween_PrefixPatch(LayoutBuilder __instance, Vector2 tile2, ref LayoutPrefabSet ___Prefabs, ref LayoutBuilderPatchState __state)
        {
            // Only patch layout building for the Kitchen floorplan (not Franchise floorplan)
            if (!__instance.Parent.gameObject.name.StartsWith(KITCHEN_FLOORPLAN_NAME))
                return;

            // Remove only internal walls
            if (!LayoutHelpers.IsInside(__instance.Blueprint[tile2].Type))
                return;

            // Quick-swap the wall prefab with an empty GameObject (with a particular name)
            __state.ShortWallPrefab = ___Prefabs.ShortWallPrefab;
            ___Prefabs.ShortWallPrefab = new GameObject(REPLACE_WALL_NAME);
        }

        [HarmonyPatch("BuildWallBetween")]
        [HarmonyPostfix]
        public static void BuildWallBetween_PostfixPatch(LayoutBuilder __instance, Vector2 tile2, ref LayoutPrefabSet ___Prefabs, LayoutBuilderPatchState __state)
        {
            // Only patch layout building for the Kitchen floorplan (not Franchise floorplan)
            if (!__instance.Parent.gameObject.name.StartsWith(KITCHEN_FLOORPLAN_NAME))
                return;

            // Remove only internal walls
            if (!LayoutHelpers.IsInside(__instance.Blueprint[tile2].Type))
                return;

            // Remove the replacement GO and swap the original prefab back in
            Object.Destroy(___Prefabs.ShortWallPrefab);
            ___Prefabs.ShortWallPrefab = __state.ShortWallPrefab;
        }

        public struct LayoutBuilderPatchState
        {
            public GameObject ShortWallPrefab;
        }
    }

    public class ResetWallsInPractise : StartOfDaySystem, IModSystem
    {
        protected override void OnUpdate()
        {
            ReplaceWallSystem.Instance?.Reset(); //TODO: only in practise
        }
    }

    public class ResetHammeredWallsInHQ : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            ReplaceWallSystem.Instance?.Reset();
        }
    }

    public class ReplaceWallSystem : NightSystem, IModSystem //TODO: why doesnt RestaurantInitialisationSystem work? or StartOfNightSystem?
    {
        private static ReplaceWallSystem _instance;
        public static ReplaceWallSystem Instance => _instance;

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

            using NativeArray<Entity> existingReplacedWalls = replacedWallQuery.ToEntityArray(Allocator.TempJob);

            if (existingReplacedWalls.Length > 0)
            {
                SetupExistingWallEntities(existingReplacedWalls);
            }
            else
            {
                LayoutView kitchenLayout = Object.FindObjectsOfType<LayoutView>().FirstOrDefault(l => l.name.StartsWith(LayoutBuilder_Patch.KITCHEN_FLOORPLAN_NAME));

                if (!kitchenLayout)
                {
                    Mod.LogInfo("Couldn't find kitchen layout while adding wall entities");
                    return;
                }

                AddWallEntities(kitchenLayout.transform);
            }
        }
        
        private void SetupExistingWallEntities(NativeArray<Entity> existingReplacedWalls)
        {

            foreach (Entity replacedWall in existingReplacedWalls)
            {
                if (EntityManager.RequireComponent(replacedWall, out CWallReplaced cWallHasBeenReplaced))
                {
                    cWallHasBeenReplaced.HammeringWasAttemptedToday = false;
                    EntityManager.SetComponentData(replacedWall, cWallHasBeenReplaced);
                }
                if (EntityManager.RequireComponent(replacedWall, out CTakesDuration cTakesDuration))
                {
                    cTakesDuration.Manual = false;
                    cTakesDuration.Total = WallReplaced.HammerDuration;
                    cTakesDuration.Remaining = WallReplaced.HammerDuration;
                    EntityManager.SetComponentData(replacedWall, cTakesDuration);
                }
            }
        }

        private void AddWallEntities(Transform transform)
        {
            Mod.LogInfo("Replacing wall entities...");
            foreach (Transform child in transform)
            {
                if (!child.gameObject.name.StartsWith(LayoutBuilder_Patch.REPLACE_WALL_NAME))
                    continue;

                //TODO: make this actually be what side the player made the hatch from
                Vector3 from = new Vector3(Mathf.Floor(child.position.x), 0, Mathf.Floor(child.position.z));
                Vector3 to = new Vector3(Mathf.Ceil(child.position.x), 0, Mathf.Ceil(child.position.z));

                // Failsafe, might not be needed
                if (!LayoutHelpers.IsInside(GetTile(from).Type))
                    continue;
                if (!LayoutHelpers.IsInside(GetTile(to).Type))
                    continue;

                int wallMaterial = MaterialUtils.GetExistingMaterial("Wall Main").GetInstanceID(); //TODO: get the actual materials of walls when mod was added mid run
                bool isHorizontal = Mathf.Approximately(from.z, to.z);

                Reachability reachabilitySideA = default(Reachability);
                Reachability reachabilitySideB = default(Reachability);
                if (isHorizontal)
                {
                    int direction = (int)Mathf.Sign(to.x - from.x);
                    reachabilitySideA[direction, 1] = true;
                    reachabilitySideA[direction, 0] = true;
                    reachabilitySideA[direction, -1] = true;

                    reachabilitySideB[direction * -1, 1] = true;
                    reachabilitySideB[direction * -1, 0] = true;
                    reachabilitySideB[direction * -1, -1] = true;
                }
                else
                {
                    int direction = (int)Mathf.Sign(to.z - from.z);
                    reachabilitySideA[1, direction] = true;
                    reachabilitySideA[0, direction] = true;
                    reachabilitySideA[-1, direction] = true;

                    reachabilitySideB[1, direction * -1] = true;
                    reachabilitySideB[0, direction * -1] = true;
                    reachabilitySideB[-1, direction * -1] = true;
                }
                //TODO: should set the rachability on the layout tiles directly? how? check LayoutExtensions

                AddWallEntity(child, from, to, reachabilitySideA, reachabilitySideB, wallMaterial);
            }
        }
        
        private void AddWallEntity(Transform child, Vector3 from, Vector3 to, Reachability reachabilitySideA, Reachability reachabilitySideB, int wallMaterial)
        {

            Entity entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new CCreateAppliance { ID = Refs.WallReplaced.ID });
            EntityManager.AddComponentData(entity, new CPosition(child.position, child.rotation));
            EntityManager.AddComponentData(entity, new CFixedRotation());
            EntityManager.AddComponentData(entity, new CWallReplaced(child.position, from, to, GetRoom(from), GetRoom(to), reachabilitySideA, reachabilitySideB, wallMaterial, wallMaterial, false));
        }
    }
}