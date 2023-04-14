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
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenMyMod
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

        //public static AssetBundle Bundle;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            // AddGameDataObject<MyCustomGDO>();

            LogInfo("Done loading game data.");
        }


        [UpdateBefore(typeof(ItemTransferGroup))]
        public class SwitchDualProvider : ItemInteractionSystem, IModSystem
        {
            //private CDualProvider dualProvider;
            private CItemProvider itemProvider;
            private EntityQuery AppliancesQuery;

            protected override void Initialise()
            {
                base.Initialise();
                AppliancesQuery = GetEntityQuery(new QueryHelper()
                        .All(typeof(CAppliance))
                        .None(
                            typeof(CFire),
                            typeof(CIsOnFire),
                            typeof(CFireImmune)
                        ));
            }

            protected override bool IsPossible(ref InteractionData data)
            {
                /*bool flag = !Require(data.Target, out dualProvider);
                bool result;
                if (flag)
                {
                    result = false;
                }
                else
                {
                    bool flag2 = !Require(data.Target, out itemProvider);
                    result = !flag2;
                }
                return result;*/
                return true;
            }

            protected override void Perform(ref InteractionData data)
            {
                //dualProvider.Current = (dualProvider.Current + 1) % 2;
                //int provide = dualProvider.Provide;
                //SetComponent(data.Target, dualProvider);
                //itemProvider.SetAsItem(provide);
                //SetComponent(data.Target, itemProvider);

                LogInfo("TEST");


                if (!EntityManager.RequireComponent<CPosition>(data.Interactor, out CPosition playerPosition))
                    return;

                GameObject floorplan = GameObject.Find("Kitchen Floorplan(Clone)");
                Transform closestWall = null;
                float closestDistance = float.MaxValue;
                foreach (Transform child in floorplan.transform)
                {
                    if (child.name != "Short Wall Section(Clone)" || !child.gameObject.activeSelf)
                        continue;
                    
                    float distance = Vector3.Distance(child.position, playerPosition.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestWall = child;
                    }
                }

                if (closestWall != null && closestDistance <= 1)
                {
                    closestWall.gameObject.SetActive(false);

                    //GameObject.Instantiate(GDOUtils.GetExistingGDO(-488268556), closestWall.position, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GDOUtils.GetExistingGDO(-1265562836), closestWall.position + Vector3.right * 2, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GDOUtils.GetExistingGDO(-758567246), closestWall.position + Vector3.right * 4, Quaternion.identity, floorplan.transform);

                    //Entity entity = EntityManager.CreateEntity(ApplianceReferences.WallPiece);

                    GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.WorkshopFence) as Appliance).Prefab), closestWall.position, closestWall.rotation, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.WallPiece) as Appliance).Prefab), closestWall.position, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.InternalWallPiece) as Appliance).Prefab), closestWall.position + Vector3.right * 2, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.Fencing) as Appliance).Prefab), closestWall.position + Vector3.right * 4, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.GarageDivider) as Appliance).Prefab), closestWall.position + Vector3.right * 6, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.WorkshopFence) as Appliance).Prefab), closestWall.position + Vector3.right * 8, Quaternion.identity, floorplan.transform);
                    //GameObject.Instantiate(GameObject.Instantiate((GDOUtils.GetExistingGDO(ApplianceReferences.WorkshopGate) as Appliance).Prefab), closestWall.position + Vector3.right * 10, Quaternion.identity, floorplan.transform);
                    //TODO: cant find the hatch, need to manually reference model & material?
                    //TODO: also this just stays forever... need to create it via entity system?
                }


                /*if (!EntityManager.RequireComponent<CPosition>(data.Interactor, out CPosition playerPosition))
                    return;

                NativeArray<Entity> appliances = AppliancesQuery.ToEntityArray(Allocator.TempJob);
                Entity closestAppliance = Entity.Null;
                float closestDistance = float.MaxValue;
                foreach (Entity appliance in appliances)
                {
                    if (!EntityManager.RequireComponent<CPosition>(appliance, out CPosition appliancePos))
                        continue;

                    float distance = math.distance(appliancePos.Position, playerPosition.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestAppliance = appliance;
                    }
                }
                if(closestAppliance != Entity.Null)
                    EntityManager.AddComponent<CIsOnFire>(closestAppliance);

                appliances.Dispose();*/
            }
        }



        //protected override void Player.ReportNewInput()
        /*private EntityQuery Appliances;
        protected override void OnUpdate()
        {
            //Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            Appliances = GetEntityQuery(new QueryHelper()
                    .All(typeof(wall))
                    .None(
                        typeof(CFire),
                        typeof(CIsOnFire),
                        typeof(CFireImmune),
                        typeof(CHasBeenSetOnFire)
                    ));
        }*/

        // Harmony Patches
        /*[HarmonyPatch(typeof(Door))] //TODO: as a test remove walls? or spawn a window?
        public static class DoorRemovalPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Door.Update))]
            public static void UpdatePostfix(Door __instance)
            {
                __instance.DoorGameObject.SetActive(false);
            }
        }*/

        /*[HarmonyPatch(typeof(LayoutBuilder), nameof(LayoutBuilder.BuildDoorBetween))]
        public static class LayoutBuilder_Patch
        {
            public static bool Prefix()
            {
                return false;
            }
        }*/


        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            // LogInfo("Attempting to load asset bundle...");
            // Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            // LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
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


/*using Kitchen;
using KitchenLib;
using KitchenMods;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenSledgehammer
{
    public class Mod : BaseMod
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

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void Initialise()
        {
            base.Initialise();
            // For log file output so the official plateup support staff can identify if/which a mod is being used
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!"); 

        }

        protected override void OnUpdate()
        {
            
        }

        #region Logging
        // You can remove this, I just prefer a more standardized logging
        public static void LogInfo(string _log) { Debug.Log($"{MOD_NAME} " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"{MOD_NAME} " + _log); }
        public static void LogError(string _log) { Debug.LogError($"{MOD_NAME} " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }

}
*/