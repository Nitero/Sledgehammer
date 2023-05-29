using Kitchen;
using Kitchen.Components;
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
                using var entities = wallReplacedViewQuery.ToEntityArray(Allocator.Temp);
                using var views = wallReplacedViewQuery.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using var walls = wallReplacedViewQuery.ToComponentDataArray<CWallReplaced>(Allocator.Temp);
                using var durations = wallReplacedViewQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);

                for (int i = 0; i < views.Length; i++)
                {
                    var entity = entities[i];
                    var wall = walls[i];
                    var duration = durations[i];
                    SendUpdate(views[i], new WallReplacedView.ViewData
                    {
                        HasBeenHammered = wall.HasBeenHammered,
                        HammeringWasAttemptedToday = wall.HammeringWasAttemptedToday,
                        MaterialA = wall.MaterialA,
                        MaterialB = wall.MaterialB,
                        IsBeingHammered = duration.CurrentChange > 0,
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
                public bool HasBeenHammered;
                [Key(1)]
                public bool HammeringWasAttemptedToday;
                [Key(2)]
                public int MaterialA;
                [Key(3)]
                public int MaterialB;
                [Key(4)]
                public bool IsBeingHammered;

                public bool IsChangedFrom(ViewData check)
                {
                    return HasBeenHammered != check.HasBeenHammered || HammeringWasAttemptedToday != check.HammeringWasAttemptedToday || MaterialA != check.MaterialA || MaterialB != check.MaterialB || IsBeingHammered != check.IsBeingHammered;
                }

                public IUpdatableObject GetRelevantSubview(IObjectView view)
                {
                    return view.GetSubView<WallReplacedView>();
                }
            }

            public ViewData LastData;
            public GameObject Hatch;
            public GameObject Wall;
            public SoundSource SourceHammerProgress;
            public SoundSource SourceHammerFinished;
            public bool IsBeingHammered;

            private void Update()
            {
                if (IsBeingHammered != SourceHammerProgress.IsPlaying)
                {
                    if (IsBeingHammered)
                        SourceHammerProgress.Play();
                    else
                        SourceHammerProgress.Stop();
                }
            }

            protected override void UpdateData(ViewData data)
            {
                if (!data.IsChangedFrom(LastData))
                    return;

                IsBeingHammered = data.IsBeingHammered;

                if (data.HasBeenHammered && !LastData.HasBeenHammered && !SourceHammerFinished.IsPlaying)
                    SourceHammerFinished.Play();


                Hatch.SetActive(data.HasBeenHammered);
                Wall.SetActive(!data.HasBeenHammered);


                string materialA = Mod.MaterialIdsToNames[data.MaterialA];
                string materialB = Mod.MaterialIdsToNames[data.MaterialB];

                Hatch.ApplyMaterialToChild("Cube", materialA, "BaseDefault", materialB);
                Hatch.ApplyMaterialToChild("Cube.001", materialA, materialB);

                Wall.GetChild("wallsection").ApplyMaterialToChild("Cube", materialA, "BaseDefault", materialB);
                Wall.GetChild("wallsection").ApplyMaterialToChild("Cube.001", materialA, materialB);
                //TODO: if still lags with big room save the mesh renderer
                //TODO: is OnRegister material still needed?

                LastData = data;
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
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube", "Wall Main", "BaseDefault", "Wall Main");//TODO: these are wrong(?)
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube.001", "Wall Main", "Wall Main");
            wall.GetChild("wallsection").ApplyMaterialToChild("Cube.002", "Wood - Default");

            wall.gameObject.transform.position += Vector3.down * 0.01f;//to prevent z fighting with outter walls eg in city

            WallReplacedView hammeredView = Prefab.AddComponent<WallReplacedView>();
            hammeredView.Wall = wall;
            hammeredView.Hatch = hatch;


            GameObject gameObject = new GameObject("SoundSourceWallBeingHammered");
            SoundSource sourceHammerProgress = gameObject.AddComponent<SoundSource>();
            gameObject.transform.ParentTo(Prefab.transform);
            sourceHammerProgress.Configure(SoundCategory.Effects, Mod.Bundle.LoadAsset<AudioClip>("hammerProgress4"));
            sourceHammerProgress.ShouldLoop = false;//TODO: why doesnt it work with looping and transition time?
            sourceHammerProgress.TransitionTime = 0f;
            sourceHammerProgress.VolumeMultiplier = 0.1f;
            hammeredView.SourceHammerProgress = sourceHammerProgress;

            GameObject gameObject2 = new GameObject("SoundSourceWallFinishHammering");
            SoundSource sourceHammerFinished = gameObject2.AddComponent<SoundSource>();
            gameObject2.transform.ParentTo(Prefab.transform);
            sourceHammerFinished.Configure(SoundCategory.Effects, Mod.Bundle.LoadAsset<AudioClip>("hammerFinished"));
            sourceHammerFinished.ShouldLoop = false;
            sourceHammerFinished.TransitionTime = 0f;
            sourceHammerFinished.VolumeMultiplier = 0.1f;
            hammeredView.SourceHammerFinished = sourceHammerFinished;
        }
    }
}