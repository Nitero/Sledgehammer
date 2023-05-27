using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private static Dictionary<int, int> wallpaperIdsToMaterialIds = new Dictionary<int, int>();
        public static Dictionary<int, int> WallpaperIdsToMaterialIds => wallpaperIdsToMaterialIds;

        private static Dictionary<int, string> materialIdsToNames = new Dictionary<int, string>();
        public static Dictionary<int, string> MaterialIdsToNames => materialIdsToNames;

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

            AddGameDataObject<WallReplaced>();
            AddGameDataObject<SledgehammerProcess>();
            //AddGameDataObject<SledgehammerApplianceProcess>();
            AddGameDataObject<Sledgehammer>();
            AddGameDataObject<SledgehammerProvider>();

            LogInfo("Done loading game data.");
        }


        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
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
            };

            Events.BuildGameDataPostViewInitEvent += (s, args) =>
            {
                foreach (Decor decor in args.gamedata.Get<Decor>().Where(x => x.IsAvailable))
                {
                    if (decor.Type != LayoutMaterialType.Wallpaper)
                        continue;
                    string decorName = $"{decor.ID}";
                    wallpaperIdsToMaterialIds.Add(decor.ID, decor.Material.GetInstanceID());
                    materialIdsToNames.Add(decor.Material.GetInstanceID(), decor.Material.name);
                }
                materialIdsToNames.Add(MaterialUtils.GetExistingMaterial("Wall Main").GetInstanceID(), "Wall Main");
            };
        }

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