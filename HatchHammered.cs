using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class HatchHammered : CustomAppliance
    {
        public override string UniqueNameID => "HatchHammered";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("WallHatchHammered");//HatchHammered
        public override PriceTier PriceTier => PriceTier.ExtremelyExpensive;
        public override RarityTier RarityTier => RarityTier.Common;
        public override bool IsPurchasable => false;
        public override ShoppingTags ShoppingTags => ShoppingTags.None;

        public override bool IsNonInteractive => false;
        public override bool ForceHighInteractionPriority => true;
        public override OccupancyLayer Layer => OccupancyLayer.Wall;

        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Hammered Hatch", "There once was a wall here", new()
            {
                new Appliance.Section
                {
                    Title = "Provisional",
                    Description = "You can reach and pass things over this like with a hatch"
                }
            }, new()))
        };

        //public override List<Appliance.ApplianceProcesses> Processes => new List<Appliance.ApplianceProcesses>
        //{
        //    new Appliance.ApplianceProcesses()
        //    {
        //        Process = Refs.SledgehammerProcess,
        //        Speed = 2f,
        //        IsAutomatic = false

        //        //Duration = 2,
        //        ////Process = Helper.Find<Process>(ProcessReferences.Chop),
        //        //Process = Refs.SledgehammerProcess,
        //        ////Result = (Item)GDOUtils.GetCustomGameDataObject<Sushi_Avocado_Fish_Rolled>().GameDataObject
        //        //Result = Helper.Find<Item>(ItemReferences.SteakMedium)
        //    }
        //};
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CIsInteractive(),//{ IsLowPriority = false },
            new CFireImmune(),
            new CImmovable(),
            //new CIDeconstruct(),
            
            //new CTakesDuration(){ Total = 5f, Manual = true, ManualNeedsEmptyHands = false, IsInverse = false, RelevantTool = DurationToolType.None, Mode = InteractionMode.Items, PreserveProgress = true, IsLocked = true},//without isnt selectable
            KitchenPropertiesUtils.GetCTakesDuration(5f, 0, false, true, false, DurationToolType.FireExtinguisher, InteractionMode.Appliances, false, true, false, false, 0),//without isnt selectable
            KitchenPropertiesUtils.GetCDisplayDuration(false, Refs.SledgehammerProcess.ID, false),//without this no UI
            
            //new CLinkedView(){ Identifier = },
            //new CRequiresView(){ PhysicsDriven = false, Type = ViewType.Appliance},//ProgressView //ViewMode = ViewMode.World
            //new CDestroyAfterDuration(){},
            //new CApplyProcessAfterDuration(){ BreakOnFailure = false },
            //new CLockDurationTimeOfDay(){ LockDuringNight = true, LockDuringDay = false },
            //new CStoredPlates(){ PlatesCount = 0},
            //new CStoredTables(),
        };


        //public override bool PreInteract(InteractionData data, bool isSecondary = false)
        //{
        //    Debug.Log("~~~ " + (data.Interactor.Index == Refs.Sledgehammer.ID));//just doesnt happen
        //    return true;
        //}
        //public override bool IsInteractionPossible(InteractionData data)
        //{
        //    Debug.Log("--- " + (data.Interactor.Index == Refs.Sledgehammer.ID));//just doesnt happen
        //    return true;
        //}


        public class HatchHammeredViewSystem : ViewSystemBase
        {
            EntityQuery m_HatchHammeredViewQuery;
            protected override void Initialise()
            {
                base.Initialise();
                m_HatchHammeredViewQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallHasBeenReplaced), typeof(CLinkedView)));//, typeof(CTakesDuration)));
            }
            protected override void OnUpdate()
            {
                using var views = m_HatchHammeredViewQuery.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using var duration = m_HatchHammeredViewQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);

                using var deconstructs = m_HatchHammeredViewQuery.ToComponentDataArray<CWallHasBeenReplaced>(Allocator.Temp);
                bool isDay = HasSingleton<SIsDayTime>();

                for (int i = 0; i < views.Length; i++)
                {
                    var deconstruct = deconstructs[i];
                    var dur = duration[i];
                    SendUpdate(views[i], new HatchHammeredView.ViewData
                    {
                        IsHammered = deconstruct.HasBeenHammered,
                    });
                }
            }
        }

        public class HatchHammeredView : UpdatableObjectView<HatchHammeredView.ViewData>
        {
            [MessagePackObject(false)]
            public struct ViewData : ISpecificViewData, IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
            {
                [Key(0)]
                public bool IsHammered;

                public bool IsChangedFrom(ViewData check)
                {
                    return IsHammered != check.IsHammered;
                }

                public IUpdatableObject GetRelevantSubview(IObjectView view)
                {
                    return view.GetSubView<HatchHammeredView>();
                }
            }

            public GameObject Hatch;
            public GameObject Wall;
            public ProgressView progressView;

            protected override void UpdateData(ViewData data)
            {
                Hatch.SetActive(data.IsHammered);
                Wall.SetActive(!data.IsHammered);
                //progressView.UpdateData(data);
            }
        }

        //internal void OriginalLambdaBody(Entity e, ref CItemProvider provider, in CTakesDuration duration, in CChangeProviderAfterDuration change)
        //{
        //    if (!provider.Matches(change.ReplaceItem) && duration.Remaining <= 0f && duration.Active)
        //    {
        //        //provider.SetAsItem(change.ReplaceItem);
        //        Debug.Log("DONE!!!!!!!!");
        //    }
        //}

        /*public override List<Process> RequiresProcessForShop => new()
        {
            Refs.Research
        };

        public override List<IApplianceProperty> Properties => new()
        {

            GetCItemProvider(Refs.HatchHammered.ID, 1, 1, false, false, false, false, false, false, false)
        };*/

        public override void OnRegister(GameDataObject gameDataObject)
        {
            var hatch = Prefab.GetChild("wallsection");

            //Helper.SetupThinCounter(Prefab);
            //Helper.SetupThinCounterLimitedItem(Prefab, GetPrefab("HatchHammered"), false);

            hatch.ApplyMaterialToChild("Cube", "Wall Main", "BaseDefault", "Wall Main");
            hatch.ApplyMaterialToChild("Cube.001", "Wall Main", "Wall Main");
            hatch.ApplyMaterialToChild("Cube.002", "Wood - Default");

            var wall = Prefab.GetChild("WallHammerable");
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube", "Wall Main", "BaseDefault", "Wall Main");//TODO: there are wrong(?)
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube.001", "Wall Main", "Wall Main");
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube.002", "Wood - Default");

            HatchHammeredView deconstructorView = Prefab.AddComponent<HatchHammeredView>();
            deconstructorView.Wall = wall;
            deconstructorView.Hatch = hatch;

            //ProgressView progressView = Prefab.AddComponent<ProgressView>();
            //deconstructorView.progressView = progressView;
        }
    }
}