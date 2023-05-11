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
    public class Mod : BaseMod, IModSystem
    {
        // guid must be unique and is recommended to be in reverse domain name notation
        // mod name that is displayed to the player and listed in the mods menu
        // mod version must follow semver e.g. "1.2.3"
        public const string MOD_GUID = "Nito.PlateUp.Sledgehammer";
        public const string MOD_NAME = "Sledgehammer";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "Nito";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif
        public static AssetBundle Bundle;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            AddGameDataObject<HatchHammered>();
            AddGameDataObject<SledgehammerProcess>();
            //AddGameDataObject<SledgehammerApplianceProcess>();
            AddGameDataObject<Sledgehammer>();
            AddGameDataObject<SledgehammerProvider>();

            LogInfo("Done loading game data.");
        }


        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            LogInfo("Attempting to load asset bundle...");
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            LogInfo("Done loading asset bundle.");

            //Events.BuildGameDataPreSetupEvent += delegate (object s, BuildGameDataEventArgs args)
            //{
            //    VisualEffectHelper.SetupEffectIndex();
            //};


            // Register custom GDOs
            AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                //AddEnum<DurationToolType>(58);

                // Sledgehammer
                /*AddEnum<DurationToolType>(11);

                if (TryRemoveComponentsFromAppliance<Appliance>(Refs.ResearchDesk.ID, new Type[] { typeof(CTakesDuration) }))
                {
                    Refs.ResearchDesk.Properties.Add(GetCTakesDuration(5, 0, false, true, false, (DurationToolType)11, InteractionMode.Items, false, true, false, false, 0));
                }

                if (TryRemoveComponentsFromAppliance<Appliance>(Refs.DiscountDesk.ID, new Type[] { typeof(CTakesDuration) }))
                {
                    Refs.DiscountDesk.Properties.Add(GetCTakesDuration(5, 0, false, true, false, (DurationToolType)11, InteractionMode.Items, false, true, false, false, 0));
                }

                if (TryRemoveComponentsFromAppliance<Appliance>(Refs.CopyingDesk.ID, new Type[] { typeof(CTakesDuration) }))
                {
                    Refs.CopyingDesk.Properties.Add(GetCTakesDuration(5, 0, false, true, false, (DurationToolType)11, InteractionMode.Items, false, true, false, false, 0));
                }*/
            };
        }

        //public void AddEnum<T>(int numInEnum) where T : Enum
        //{
        //    Type enumType = typeof(T);
        //    object value = System.Convert.ChangeType(numInEnum, Enum.GetUnderlyingType(enumType));
        //    object enumValue = Enum.ToObject(enumType, value);
        //    T cursedEnum = (T)enumValue;
        //    Mod.LogInfo(cursedEnum);
        //}

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}