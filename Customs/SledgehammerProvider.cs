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
        public override RarityTier RarityTier => RarityTier.Rare;
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
                    Description = "Use this to tear down a wall into a hatch"
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
            var thinBlock = Prefab.GetChild("Thin Block");
            var sledgeHammer = parent.GetChild("Sledgehammer");

            Helper.SetupThinCounter(Prefab);
            Helper.SetupThinCounterLimitedItem(Prefab, Helper.GetPrefab("Sledgehammer"), false);

            thinBlock.ApplyMaterialToChild("SledgehammerStand", "Wood - Default");

            Sledgehammer.ApplyMaterial(sledgeHammer);
        }
    }
}