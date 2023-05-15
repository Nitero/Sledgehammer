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
    public class WallReplaced : CustomAppliance
    {
        public override string UniqueNameID => "WallReplaced";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("WallReplaced");
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

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CFireImmune(),
            new CImmovable(),
            
            KitchenPropertiesUtils.GetCDisplayDuration(false, Refs.SledgehammerProcess.ID, false),
            KitchenPropertiesUtils.GetCTakesDuration(10f, 10f, false, false, false, DurationToolType.None, InteractionMode.Appliances, false, true, false, false, 0),
        };

        public class WallReplacedViewSystem : ViewSystemBase
        {
            EntityQuery wallReplacedViewQuery;
            protected override void Initialise()
            {
                base.Initialise();
                wallReplacedViewQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallReplaced), typeof(CLinkedView)));
            }
            protected override void OnUpdate()
            {
                using var views = wallReplacedViewQuery.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using var duration = wallReplacedViewQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);

                using var walls = wallReplacedViewQuery.ToComponentDataArray<CWallReplaced>(Allocator.Temp);
                bool isDay = HasSingleton<SIsDayTime>();

                for (int i = 0; i < views.Length; i++)
                {
                    var deconstruct = walls[i];
                    var dur = duration[i];
                    SendUpdate(views[i], new WallReplacedView.ViewData
                    {
                        IsHammered = deconstruct.HasBeenHammered,
                    });
                }
            }
        }

        public class WallReplacedView : UpdatableObjectView<WallReplacedView.ViewData>
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
                    return view.GetSubView<WallReplacedView>();
                }
            }

            public GameObject Hatch;
            public GameObject Wall;
            public ProgressView progressView;

            protected override void UpdateData(ViewData data)
            {
                Hatch.SetActive(data.IsHammered);
                Wall.SetActive(!data.IsHammered);
            }
        }

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

            WallReplacedView hammeredView = Prefab.AddComponent<WallReplacedView>();
            hammeredView.Wall = wall;
            hammeredView.Hatch = hatch;
        }
    }
}