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
            return;

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
                    reachabilitySideA[direction, 1] = true;
                    reachabilitySideA[direction, 0] = true;
                    reachabilitySideA[direction,-1] = true;

                    reachabilitySideB[direction * -1, 1] = true;
                    reachabilitySideB[direction * -1, 0] = true;
                    reachabilitySideB[direction * -1,-1] = true;
                }
                else
                {
                    int direction = (int)Mathf.Sign(to.z - from.z);
                    reachabilitySideA[ 1, direction] = true;
                    reachabilitySideA[ 0, direction] = true;
                    reachabilitySideA[-1, direction] = true;

                    reachabilitySideB[ 1, direction * -1] = true;
                    reachabilitySideB[ 0, direction * -1] = true;
                    reachabilitySideB[-1, direction * -1] = true;
                }
                //TODO: should set the rachability on the layout tiles directly? how? check LayoutExtensions

                Entity entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new CCreateAppliance{ ID = Refs.WallReplaced.ID });
                EntityManager.AddComponentData(entity, new CPosition(child.position, child.rotation));
                EntityManager.AddComponentData(entity, new CFixedRotation());
                EntityManager.AddComponentData(entity, new CWallReplaced(child.position, from, to, GetRoom(from), GetRoom(to), reachabilitySideA, reachabilitySideB, wallMaterial, wallMaterial, false));
            }
        }
    }
}