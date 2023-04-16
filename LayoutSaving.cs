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
            //SRestoreHammeredWalls.Instance?.Reset(); //TODO: only in practise
        }
    }

    public class SResetHammeredWallsInHQ : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            SRestoreHammeredWalls.Instance?.Reset();
        }
    }

    //[UpdateAfter(typeof(SLayout))]
    [UpdateAfter(typeof(SKitchenLayout))]
    public class SRestoreHammeredWalls : NightSystem, IModSystem //TODO: why doesnt RestaurantInitialisationSystem work? or StartOfNightSystem?
    {
        private static SRestoreHammeredWalls _instance;
        public static SRestoreHammeredWalls Instance => _instance;


        private EntityQuery HammeredWallsQuery;
        struct CHasBeenHammered : IModComponent
        {
            public Vector3 WallPosition;
            public Vector3 From;
            public Vector3 To;

            public CHasBeenHammered(Vector3 wallPosition, Vector3 from, Vector3 to)
            {
                WallPosition = wallPosition;
                From = from;
                To = to;
            }
        }

        private bool _didSetup;


        public void Reset()
        {
            _didSetup = false;
        }

        protected override void Initialise()
        {
            base.Initialise();
            HammeredWallsQuery = GetEntityQuery(new QueryHelper().All(typeof(CHasBeenHammered)));
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

            NativeArray<Entity> _hammeredWalls = HammeredWallsQuery.ToEntityArray(Allocator.TempJob);

            Transform floorplan = GameObject.Find("Kitchen Floorplan(Clone)").transform;
            foreach (var wall in _hammeredWalls)
            {
                if (!EntityManager.RequireComponent<CHasBeenHammered>(wall, out CHasBeenHammered hasBeenHammered))
                    continue;

                Helpers.TryToRepalceWallWithHatch(hasBeenHammered.WallPosition, floorplan);
            }
        }

        public void Hammered(Vector3 wallPosition)
        {
            Vector3 from = new Vector3(Mathf.Floor(wallPosition.x), Mathf.Floor(wallPosition.y), Mathf.Floor(wallPosition.z));
            Vector3 to = new Vector3(Mathf.Ceil(wallPosition.x), Mathf.Ceil(wallPosition.y), Mathf.Ceil(wallPosition.z));
            //TODO: make this actually be what side the player made the wall from

            EntityManager.AddComponentData(EntityManager.CreateEntity(), new CHasBeenHammered(wallPosition, from, to));
        }

        public bool IsHammeredWallBetween(Vector3 from, Vector3 to)
        {
            from = new Vector3(Mathf.Round(from.x), 0, Mathf.Round(from.z));
            to = new Vector3(Mathf.Round(to.x), 0, Mathf.Round(to.z));

            NativeArray<Entity> _hammeredWalls = HammeredWallsQuery.ToEntityArray(Allocator.TempJob);
            foreach (var wall in _hammeredWalls)
            {
                if (!EntityManager.RequireComponent<CHasBeenHammered>(wall, out CHasBeenHammered hasBeenHammered))
                    continue;

                var sideA = hasBeenHammered.From;
                var sideB = hasBeenHammered.To;

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
}