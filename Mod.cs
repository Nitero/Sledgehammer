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
            //AddGameDataObject<Sledgehammer>();
            //AddGameDataObject<SledgehammerProvider>();

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

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }



    [UpdateBefore(typeof(ItemTransferGroup))]
    public class SwitchDualProvider : ItemInteractionSystem, IModSystem
    {
        protected override bool IsPossible(ref InteractionData data)
        {
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (GameInfo.CurrentScene != SceneType.Kitchen)
                return;

            if (!EntityManager.RequireComponent<CPosition>(data.Interactor, out CPosition playerPosition))
                return;

            Transform closestWall = Helpers.TryToRepalceWallWithHatch(playerPosition.Position);
            if(closestWall != null)
                SRestoreHammeredWalls.Instance?.Hammered(closestWall.transform.position);
        }
    }


    [HarmonyPatch]
    public static class CanReachOverHammeredWallPatch
    {
        [HarmonyPatch(typeof(GenericSystemBase), "CanReach")]
        [HarmonyPrefix]
        static bool CanReach_Prefix(ref bool __result, Vector3 from, Vector3 to, bool do_not_swap = false)
        {
            if (SRestoreHammeredWalls.Instance.IsHammeredWallBetween(from, to))
            {
                __result = true;
                return false;
            }
            return true;//run original
        }
    }
}

/*--------- Non appliance GDOs ---------
Fish - Fillet Raw (-2145487392)
Beans - Serving (-2138118944)
Wok (-2135410839)
Rolling Pin (-2110926326)
Sauce - Mushroom Raw (-2105805937)
Nuts - Chopped (-2100850612)
Mushroom - Chopped (-2093899333)
Mop - Fast (-2083359821)
Crate (-2065950566)
Cherry (-2056677123)
Salad - Potato Plated (-2053442418)
Fish - Spiny Raw (-2047884387)
Crab - Cooked Cake (-2007852530)
Roast Leg - Depleted (-2002011353)
Potato (-1972529263)
Boiled Potato - Cooked (-1965870011)
Burned Food (-1960690485)
Boned Steak - Bone (-1955934157)
Birthday Cake (-1950713115)
Nut Mixture - Baked (-1945246136)
Carrot (-1944015682)
Dumplings - Plated (-1938035042)
Nut Roast - Plated (-1934880099)
Disposable Rubbish (-1931641307)
Beans - Raw Pot (-1921097327)
Oil (-1900989960)
Bread - Baked (-1867438686)
Table Block Rubbish (-1863985141)
Soup - Tomato Raw (-1863787598)
Corn - Husked (-1854029532)
Oil - Ingredient (-1853193980)
Seaweed - Cooked (-1847818036)
Colouring Book (-1843738466)
Cake Slice (-1842891105)
Salad - Plated (-1835015742)
Turkey - Ingredient (-1831502471)
Roast Leg - Cooked (-1801513942)
Cranberry Sauce (-1788071646)
Broccoli - Raw (-1774883004)
Burger Bun (-1756808590)
Turkey - Burned (-1755371377)
Fish - Spiny Bones (-1724190260)
Thin Steak - Rare (-1720486713)
Pie - Vegetable Raw - Blind Baked (-1701915481)
Franchise Card Set (-1677093775)
Bin Bag (-1660145659)
Bamboo Pot (-1652763586)
Corn - Cooked (-1640761177)
Bamboo Raw (-1635701703)
Onion - Cooked Wrapped (-1633089577)
Steak - Well-done (-1631681807)
Pie - Vegetable Cooked (-1612932608)
Fish - Pink Plated (-1608542149)
Soup - Carrot Cooked (-1582466042)
Roast Leg - Raw (-1574653982)
Turkey - Cooked (-1568853395)
Plate - Dirty with food (-1527669626)
Broccoli - Serving (-1520921913)
Nut Mixture (-1515496760)
Pumpkin Seeds (-1498186615)
Crab - Raw (-1452580334)
Pie - Vegetable Raw (-1428220456)
Carrot - Chopped Container Cooked (-1406021079)
Potato - Chopped Cooked (-1399719685)
Lettuce - Chopped (-1397390776)
Soup - Carrot Raw (-1361723814)
Boiled Potato - Mashed (-1341614392)
Tomato Sauce (-1317168923)
Leave (-1310307277)
Ice Cream - Serving (-1307479546)
Nut Mixture - Portion (-1294491269)
Coffee Cup - Coffee (-1293050650)
Soup - Meat Cooked (-1284423669)
Onion - Chopped (-1252408744)
Sauce - Mushroom Portion (-1217105161)
Pizza - Cooked (-1196800934)
Condiment - Mustard (-1114203942)
Pizza - Plated (-1087205958)
Onion Rings - Cooked (-1086687302)
Condiment - Ketchup (-1075930689)
Pizza - Burned (-1063655063)
Steak - Plated (-1034349623)
Meat - Chopped Container Cooked (-1018018897)
Soup - Chopped Tomato Raw (-996662132)
Thin Steak - Well-done (-989359657)
Roast Potato Item (-939434748)
Fish - Oyster Shucked (-920494794)
Turkey - Slice (-914826716)
Fish - Spiny Deboned (-890521693)
Burger - Plated (-884392267)
Floor Buffer (-864849315)
Tomato - Chopped (-853757044)
Sugar (-849164789)
Napkin (-834566246)
Carrot - Chopped (-830135945)
Mop - Lasting (-819389746)
Patience (-808698209)
Boned Steak - Plated (-783008587)
Turkey Bones (-777417645)
Cheese (-755280170)
Boiled Potato - Raw (-735644169)
Candle (-731135737)
Soup - Refilled (-719587509)
Pumpkin Pieces (-711877651)
Potato - Chopped Pot Raw (-706413527)
Pie - Pumpkin Raw - Blind Baked (-677830190)
Pie - Meat Raw - Blind Baked (-671227602)
Sharp Knife (-670427032)
Pie - Apple Raw (-642148977)
Serving Board (-626784042)
Non Loadout Crate (-620886547)
Supply Box (-601076588)
Test Non Combine Item (-587882643)
Specials Menu (-538929686)
Thick Steak - Rare (-510353055)
Dish Choice (-509800267)
Fish - Fillet Cooked (-505249062)
Fish - Spiny Plated (-491640227)
Pot (-486398094)
Pie - Meat Raw (-469170277)
Burger - Unplated (-417685193)
Forget-Me-Not (-401734755)
Cheese - Wrapped Cooked (-369505908)
Stir Fry - Plated (-361808208)
Stuffing (-352397598)
Mushroom - Cooked Wrapped (-336580972)
Meat - Boned (-315069952)
Bread - Slice (-306959510)
Sauce - Red Portion (-285798592)
Thick Steak - Medium (-283606362)
Burned Bread (-263299406)
Mandarin Slices - 2 Serving (-263257027)
Boned Steak - Rare (-260257840)
Chips - Cooked (-259844528)
Affogato Item (-249136431)
Hotdog - Cooked (-248200024)
Fire Extinguisher (-241697184)
Onion (-201067776)
Roast Leg - Served (-166749992)
Pumpkin (-165143951)
Pie - Cherry Raw - Blind Baked (-135657781)
Pie - Pumpkin Cooked (-126602470)
Scrubbing Brush (-110929446)
Layout Map (-70952701)
Broth - Cooked Onion (-69847810)
Lettuce (-65594226)
Pizza - Crust (-48499881)
Meat - Thick (-45632521)
Breadsticks (-44050480)
Fish - Burned (9768533)
Test Item Group (26858422)
Potato - Chopped (35611244)
Research Flask (56610526)
Sauce - Mushroom Cooked (65943925)
Beans - Ingredient (75221795)
Pie - Apple Cooked (82666420)
Broccoli - Cooked Pot (98665743)
Boiled Potato - Serving (107345299)
Fish Special - Extra Choice 1 (107399665)
Stir Fry - Cooked (150639636)
Boned Steak - Well-done (153969149)
Cranberries - Chopped (163163953)
Pie Crust - Raw (164600160)
Ice Cream - Strawberry (186895094)
Fish - Oyster Raw (216090589)
Mandarin Slices - 4 Serving (226055037)
Soup - Broccoli Cheese Cooked (226578993)
Breadcrumbs (235356204)
Apple Slices (252763172)
Cheese - Grated (263830100)
Sharp Cutlery (269092883)
Pie - Mushroom Cooked (280553412)
Gravy - Turkey Cooked (294281422)
Mushroom (313161428)
Steak - Burned (320607572)
Plate - Dirty with Bone (348289471)
Coffee Cup (364023067)
Beer Mug (369328905)
Egg - Cracked (378690159)
Fish - Oyster Plated (403539963)
Soup - Pumpkin Cooked (407468560)
Served Soup - Carrot (409276704)
Fish - Pink Fried (411057095)
Pie - Mushroom Raw - Blind Baked (415541985)
Boned Steak - Medium (418682003)
Pie - Mushroom Raw (427507425)
Bread - Toast (428559718)
Pizza - Raw (445221203)
Mandarin Slice (448483396)
Fish - Blue Fried (454058921)
Thin Steak - Burned (469714996)
Ice Cream - Chocolate (502129042)
Corn - Raw (529258958)
Fish - Blue Plated (536781335)
Mayonnaise (564003642)
Salad - Apple Plated (599544171)
Nuts - Ingredient (609827370)
Cooked Apple (617153544)
Thick Steak - Well-done (623804310)
Apple (681117884)
Burger Patty - Cooked (687585830)
Prepared Dumplings (718093067)
Steak - Medium (744193417)
Broccoli - Chopped (748471091)
Christmas Cracker (749675166)
Hotdog Bun (756326364)
Served Soup - Pumpkin (790436685)
Plate (793377380)
Soup - Pumpkin Raw (801092248)
Pie - Plated (861630222)
Tray (869580494)
Bamboo Cooked - Container Cooked (880804869)
Olive (892659864)
Served Soup - Tomato (894680043)
Flammable Bin Bag (895813906)
Boned Steak - Burned (936242560)
Pizza - Slice (938942828)
Pumpkin Hollow (951737916)
Thick Steak - Burned (958173724)
Meat - Chopped (1005005768)
Fish - Fillet Plated (1011454010)
Pumpkin Seeds - Roasted (1018675021)
Pot - Dirty (1026000491)
Soup - Broccoli Cheese Raw (1030599135)
Pie - Meat Cooked (1030798878)
Soup - Meat Raw (1064697910)
Thick Steak - Plated (1067846341)
Corn - Husk (1075166571)
Hotdog - Unplated (1134979829)
Test Combine Item 2 (1135389096)
Mop (1142792325)
Burger Patty - Raw (1150879908)
Turkey Gravy (1168127977)
Thin Steak - Plated (1173464355)
Condiment - Soy Sauce (1190974918)
Crab - Cake Egged (1195805465)
Pie - Cherry Raw (1196761342)
Tomato (1242961771)
Fish - Pink Raw (1244918234)
Fish - Spiny Cooked (1247388187)
Meat - Thin (1256038534)
Rice (1271508828)
Beans - Cooked (1286433124)
Sauce - Red Raw (1289839594)
Mandarin Raw (1291848678)
Dough (1296980128)
Seaweed (1297982178)
Meat (1306214641)
Egg - Cooked (1324261001)
Broth - Raw Onion (1370203151)
Flour (1378842682)
Served Soup - Broccoli Cheese (1384211889)
Wine Bottle (1387195911)
Stuffing - Raw (1427021177)
Test Combine Item 1 (1453292775)
Broccoli - Chopped Container Cooked (1453647256)
Cranberries (1474921248)
Stir Fry - Raw (1475451665)
Menu (1491776620)
Bread Starter Item (1503471951)
Plate - Dirty (1517992271)
Ice Cream - Vanilla (1570518340)
Fish - Blue Raw (1592653566)
Pie - Dessert Plated (1605432111)
Fish - Fillet (1607298447)
Cheese Board - Serving (1639948793)
Cooked Dumplings (1640282430)
Thin Steak - Medium (1645212811)
Soup - Chopped Carrot Raw (1655490768)
Water (1657174953)
Crab - Chopped (1678080982)
Served Soup - Meat (1684936685)
Sauce - Red Cooked (1690253467)
Gravy - Turkey Raw (1696315132)
Hotdog - Plated (1702578261)
Hotdog - Raw (1702717896)
Soup - Tomato Cooked (1752228187)
Breakfast - Plated (1754241573)
Egg (1755299639)
Wok - Burned (1770849684)
Pie - Pumpkin Raw (1776321746)
Contract (1778270917)
Turkey - Plated (1792757441)
Onion Rings - Raw (1818895897)
Pie - Cherry Cooked (1842093636)
Soup - Depleted (1859809622)
Dumplings Raw (1867434040)
Plate - Dirty Soaked (1882569246)
Crab - Cake Floured (1914908152)
Rice - Container Cooked (1928939081)
Steak - Rare (1936140106)
Crab - Cake Plated (1939124686)
Pie Crust - Cooked (1963815217)
Potato - Chopped Pot Cooked (2010203194)
Bamboo Pot Cooked (2019756794)
Bamboo Cooked (2037858460)
Soup - Chopped Meat Raw (2043533161)
Fish Special - Extra Choice 2 (2113587247)
Broccoli - Pot (2141493703)
*/