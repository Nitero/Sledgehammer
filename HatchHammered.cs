using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class HatchHammered : CustomAppliance
    {
        public override string UniqueNameID => "HatchHammered";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("HatchHammered");
        public override PriceTier PriceTier => PriceTier.ExtremelyExpensive;
        public override RarityTier RarityTier => RarityTier.Common;
        public override bool IsPurchasable => false;
        public override ShoppingTags ShoppingTags => ShoppingTags.None;

        public override List<(Locale, ApplianceInfo)> InfoList => new()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Hammered Hatch", "There once was a wall here", new()
            {
                new Appliance.Section
                {
                    Title = "Blunt",
                    Description = "You can reach and pass things over this like with a hatch"
                }
            }, new()))
        };

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
            var parent = Prefab.GetChild("Hibachi Table");

            //Helper.SetupThinCounter(Prefab);
            //Helper.SetupThinCounterLimitedItem(Prefab, GetPrefab("HatchHammered"), false);
;
            parent.ApplyMaterialToChild("Base", "Piano Black");
            parent.ApplyMaterialToChild("Top", "Wood");
        }
    }
}