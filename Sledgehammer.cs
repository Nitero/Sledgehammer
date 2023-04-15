using KitchenData;
using KitchenLib.Customs;
using UnityEngine;

namespace KitchenSledgehammer
{
    /*public class Sledgehammer : CustomItem
    {
        public override string UniqueNameID => "Sledgehammer";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Sledgehammer");
        public override ItemCategory ItemCategory => ItemCategory.Generic;
        public override ItemStorage ItemStorageFlags => ItemStorage.None;
        public override ItemValue ItemValue => ItemValue.Small;
        public override ToolAttachPoint HoldPose => ToolAttachPoint.Hand;
        public override bool IsIndisposable => true;
        public override List<IItemProperty> Properties => new()
        {
            new CProcessTool()
            {
                Process = Refs.Research.ID,
                Factor = 2
            },
            new CEquippableTool()
            {
                CanHoldItems = true
            },
            new CDurationTool()
            {
                Type = (DurationToolType)11,
                Factor = 2
            }
        };

        public override void OnRegister(GameDataObject gameDataObject)
        {
            Prefab.ApplyMaterialToChild("Sledgehammer/Cube.003", "Plastic - White", "Plastic - Grey");
        }
    }*/
}