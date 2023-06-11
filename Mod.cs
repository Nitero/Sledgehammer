using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenLib.Utils;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
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


        #region Preferences
        public const string SLEDGEHAMMER_RARITY = "sledgehammerRarity";
        public const string SLEDGEHAMMER_PRICE = "sledgehammerPrice";
        public const string SLEDGEHAMMER_DURATION = "sledgehammerDuration";
        public const string SLEDGEHAMMER_CONSEQUENCE = "sledgehammerConsequence";
        #endregion

        internal static PreferenceManager PrefManager;
        internal static PreferenceInt SledgehammerRarity = new PreferenceInt(SLEDGEHAMMER_RARITY, 2);
        internal static PreferenceInt SledgehammerPrice = new PreferenceInt(SLEDGEHAMMER_PRICE, 1250);
        internal static PreferenceInt SledgehammerDuration = new PreferenceInt(SLEDGEHAMMER_DURATION, 15);
        internal static PreferenceInt SledgehammerConsequence = new PreferenceInt(SLEDGEHAMMER_CONSEQUENCE, 1);


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

        private void AddProcessIcons()
        {
            Bundle.LoadAllAssets<Texture2D>();
            Bundle.LoadAllAssets<Sprite>();

            var spriteAsset = Bundle.LoadAsset<TMP_SpriteAsset>("hammerIcon");
            TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets.Add(spriteAsset);
            spriteAsset.material = Object.Instantiate(TMP_Settings.defaultSpriteAsset.material);
            spriteAsset.material.mainTexture = Bundle.LoadAsset<Texture2D>("hammerIconSprite");
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


            PrefManager = new PreferenceManager(MOD_GUID);

            PrefManager.RegisterPreference(SledgehammerRarity);
            PrefManager.RegisterPreference(SledgehammerPrice);
            PrefManager.RegisterPreference(SledgehammerDuration);
            PrefManager.RegisterPreference(SledgehammerConsequence);

            PrefManager.Load();

            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu("Sledgehammer", typeof(SledgehammerMenu<PauseMenuAction>), typeof(PauseMenuAction));

            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(SledgehammerMenu<PauseMenuAction>), new SledgehammerMenu<PauseMenuAction>(args.Container, args.Module_list));
            };

            // Register custom GDOs
            AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                int rarity = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_RARITY).Get();
                Refs.SledgehammerProvider.RarityTier = (RarityTier)rarity;

                int price = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_PRICE).Get();
                if (price == 0)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.Free;
                if (price == 5)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.VeryCheap;
                if (price == 20)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.Cheap;
                if (price == 40)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.MediumCheap;
                if (price == 60)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.Medium;
                if (price == 100)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.Expensive;
                if (price == 250)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.VeryExpensive;
                if (price == 1250)
                    Refs.SledgehammerProvider.PriceTier = PriceTier.ExtremelyExpensive;

                int duration = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_DURATION).Get();
                WallReplaced.HammerDuration = duration;

                int consequence = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_CONSEQUENCE).Get();
                WallReplaced.HammerConsequence = consequence;
            };

            // Load process icons
            AddProcessIcons();

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