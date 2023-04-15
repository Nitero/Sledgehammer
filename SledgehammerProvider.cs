using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using UnityEngine;

namespace KitchenSledgehammer
{
    /*public class SledgehammerProvider : CustomAppliance
    {
        public override string UniqueNameID => "Researching Sledgehammer Provider";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Researching Sledgehammer Provider");
        public override PriceTier PriceTier => PriceTier.VeryExpensive;
        public override RarityTier RarityTier => RarityTier.Uncommon;
        public override bool IsPurchasable => true;
        public override ShoppingTags ShoppingTags => ShoppingTags.Misc;

        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Researching Sledgehammer", "Makes you seem smarter", new()
            {
                new Appliance.Section
                {
                    Title = "Scholarly",
                    Description = "Hold this to <sprite name=\"upgrade\" color=#A8FF1E> 2x faster"
                }
            }, new()))
        };

        public override List<Process> RequiresProcessForShop => new()
        {
            Refs.Research
        };

        public override List<IApplianceProperty> Properties => new()
        {

            GetCItemProvider(Refs.ResearchingSledgehammer.ID, 1, 1, false, false, false, false, false, false, false)
        };

        public override void OnRegister(GameDataObject gameDataObject)
        {
            var parent = Prefab.GetChild("GameObject");
            var inkJar = parent.GetChild("Ink Jar");

            SetupThinCounter(Prefab);
            SetupThinCounterLimitedItem(Prefab, GetPrefab("Researching Sledgehammer"), false);

            parent.ApplyMaterialToChild("Researching Sledgehammer/Sledgehammer/Cube.003", "Plastic - White", "Plastic - Grey");
            inkJar.ApplyMaterialToChild("Jar", "Oven Glass");
            inkJar.ApplyMaterialToChild("Ink", "Piano Black");
        }
    }*/
}