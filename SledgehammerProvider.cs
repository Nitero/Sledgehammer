using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class SledgehammerProvider : CustomAppliance
    {
        public override string UniqueNameID => "SledgehammerProvider";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("SledgehammerProvider");
        public override PriceTier PriceTier => PriceTier.ExtremelyExpensive;
        public override RarityTier RarityTier => RarityTier.Common;//TODO: Rare
        public override bool IsPurchasable => true;
        //public override bool IsPurchasableAsUpgrade => true;
        public override ShoppingTags ShoppingTags => ShoppingTags.Misc;

        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Sledgehammer", "Can't touch this", new()
            {
                new Appliance.Section
                {
                    Title = "Blunt",
                    Description = "Use this to tear down walls into hatches"
                }
            }, new()))
        };

        public override List<IApplianceProperty> Properties => new()
        {
            KitchenPropertiesUtils.GetCItemProvider(Refs.Sledgehammer.ID, 1, 1, false, false, false, false, false, false, false)
        };

        public override void OnRegister(GameDataObject gameDataObject)
        {
            var parent = Prefab.GetChild("GameObject");
            var inkJar = parent.GetChild("Ink Jar");

            Helper.SetupThinCounter(Prefab);
            Helper.SetupThinCounterLimitedItem(Prefab, Helper.GetPrefab("Sledgehammer"), false);

            parent.ApplyMaterialToChild("Sledgehammer/Cube.003", "Plastic - White", "Plastic - Grey");
            inkJar.ApplyMaterialToChild("Jar", "Oven Glass");
            inkJar.ApplyMaterialToChild("Ink", "Piano Black");
        }
    }
}