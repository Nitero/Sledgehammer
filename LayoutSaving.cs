using Controllers;
using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenLib.References;
using KitchenLib.Utils;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class SResetHammeredWallsInPractise : StartOfDaySystem, IModSystem
    {
        protected override void OnUpdate()
        {
            SReplaceWalls.Instance?.Reset(); //TODO: only in practise
        }
    }

    public class SResetHammeredWallsInHQ : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            SReplaceWalls.Instance?.Reset();
        }
    }

    struct CWallHasBeenReplaced : IModComponent
    {
        public Vector3 WallPosition;
        public Vector3 SideA;
        public Vector3 SideB;
        public bool HasBeenHammered;

        public CWallHasBeenReplaced(Vector3 wallPosition, Vector3 sideA, Vector3 sideB, bool hasBeenHammered)
        {
            WallPosition = wallPosition;
            SideA = sideA;
            SideB = sideB;
            HasBeenHammered = hasBeenHammered;
        }
    }

    //[UpdateAfter(typeof(SLayout))]
    [UpdateAfter(typeof(SKitchenLayout))]
    public class SReplaceWalls : NightSystem, IModSystem //TODO: why doesnt RestaurantInitialisationSystem work? or StartOfNightSystem?
    {
        private static SReplaceWalls _instance;
        public static SReplaceWalls Instance => _instance;

        private EntityQuery ReplacedWallsQuery;

        private bool _didSetup;

        public void Reset()
        {
            _didSetup = false;
        }

        protected override void Initialise()
        {
            base.Initialise();
            ReplacedWallsQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallHasBeenReplaced)));
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

            NativeArray<Entity> replacedWalls = ReplacedWallsQuery.ToEntityArray(Allocator.TempJob);
            bool alreadyReplacedWalls = replacedWalls.Length > 0;
            replacedWalls.Dispose();

            Transform floorplan = GameObject.Find("Kitchen Floorplan(Clone)").transform;
            foreach (Transform child in floorplan.transform)
            {
                if (child.name != "Short Wall Section(Clone)" || !child.gameObject.activeSelf)
                    continue;

                child.gameObject.SetActive(false);
                
                if (alreadyReplacedWalls)
                    continue;

                Entity entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new CCreateAppliance{ ID = Refs.HatchHammered.ID });
                EntityManager.AddComponentData(entity, new CPosition(child.position, child.rotation));
                EntityManager.AddComponentData(entity, new CFixedRotation());
                //EntityManager.AddComponentData(entity, new CRequiresView() { PhysicsDriven = true, Type = ViewType.Appliance });

                //Entity progressView = EntityManager.CreateEntity();
                //ProgressView progressView = progressView.AddComponent<ProgressView>();
                //deconstructorView.progressView = progressView;
                //EntityManager.AddComponentData(entity, new CHasProgressIndicator() { Indicator = progressView });
                //EntityManager.AddComponentData(entity, new CHasProgressIndicator() {  });
                //EntityManager.AddComponentData(entity, new CProgressIndicator()
                //{
                //    Progress = 0.1f,
                //    IsBad = false,
                //    Process = Refs.SledgehammerProcess.ID,
                //    IsUnknownLength = false,
                //    CurrentChange = 0,
                //});

                //TODO: make this actually be what side the player made the wall from
                Vector3 from = new Vector3(Mathf.Floor(child.position.x), Mathf.Floor(child.position.y), Mathf.Floor(child.position.z));
                Vector3 to = new Vector3(Mathf.Ceil(child.position.x), Mathf.Ceil(child.position.y), Mathf.Ceil(child.position.z));
                EntityManager.AddComponentData(entity, new CWallHasBeenReplaced(child.position, from, to, false));
            }
        }

        public void Hammered(Entity replacedWall)
        {
            if (!EntityManager.RequireComponent<CWallHasBeenReplaced>(replacedWall, out CWallHasBeenReplaced cReplacedWall))
                return;

            cReplacedWall.HasBeenHammered = true;
            EntityManager.SetComponentData(replacedWall, cReplacedWall);
        }

        public bool IsReplacedWallHammeredBetween(Vector3 from, Vector3 to)
        {
            from = new Vector3(Mathf.Round(from.x), 0, Mathf.Round(from.z));
            to = new Vector3(Mathf.Round(to.x), 0, Mathf.Round(to.z));

            NativeArray<Entity> replacedWalls = ReplacedWallsQuery.ToEntityArray(Allocator.TempJob);
            try
            {
                foreach (Entity replacedWall in replacedWalls)
                {
                    if (!EntityManager.RequireComponent<CWallHasBeenReplaced>(replacedWall, out CWallHasBeenReplaced cReplacedWall))
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
            finally
            {
                replacedWalls.Dispose();
            }
        }
    }
}