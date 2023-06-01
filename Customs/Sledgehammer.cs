using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;
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

        public override List<IItemProperty> Properties => new()
        {
            //makes the hand pose / animation work properly, but no idea what the parameter is for
            new CEquippableTool()
            {
                CanHoldItems = true
            },

            //dont think these are neccessary?
            //new CProcessTool()
            //{
            //    Process = Refs.SledgehammerProcess.ID,
            //    Factor = 1
            //},
            //new CDurationTool()
            //{
            //    Type = DurationToolType.None,
            //    Factor = 1
            //}
            //TODO: fix now cant put down hammer with grab anymore
        };

        public override void OnRegister(GameDataObject gameDataObject)
        {
            ApplyMaterial(Prefab);
        }

        public static void ApplyMaterial(GameObject sledgehammer)
        {
            sledgehammer.ApplyMaterialToChild("Hammer", "Metal", "Wood");
        }
    }
}