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

            public CHasBeenHammered(Vector3 wallPosition)
            {
                WallPosition = wallPosition;
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
            EntityManager.AddComponentData(EntityManager.CreateEntity(), new CHasBeenHammered(wallPosition));
        }
    }
}