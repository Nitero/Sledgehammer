using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class Sledgehammer : CustomItem
    {
        public override string UniqueNameID => "Sledgehammer";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("Sledgehammer");
        public override ItemCategory ItemCategory => ItemCategory.Generic;
        public override ItemStorage ItemStorageFlags => ItemStorage.None;
        public override ItemValue ItemValue => ItemValue.Small;
        public override ToolAttachPoint HoldPose => ToolAttachPoint.Hand;
        public override bool IsIndisposable => true;

        public override void OnRegister(GameDataObject gameDataObject)
        {
            Prefab.ApplyMaterialToChild("Cube.003", "Plastic - White", "Plastic - Grey");
        }
    }
}