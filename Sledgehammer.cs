using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
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
        //public override List<Item.ItemProcess> Processes => new List<Item.ItemProcess>
        //{
        //    new Item.ItemProcess
        //    {
        //        Duration = 2,
        //        //Process = Helper.Find<Process>(ProcessReferences.Chop),
        //        Process = Refs.SledgehammerProcess,
        //        //Result = (Item)GDOUtils.GetCustomGameDataObject<Sushi_Avocado_Fish_Rolled>().GameDataObject
        //        Result = Helper.Find<Item>(ItemReferences.SteakMedium)
        //    }
        //};

        //public override List<IItemProperty> Properties => new()
        //{
        //    new CProcessTool()
        //    {
        //        Process = Refs.SledgehammerProcess.ID,//needed?
        //        Factor = 1
        //    },
        //    new CEquippableTool()
        //    {
        //        CanHoldItems = true
        //    },
        //    //new CDurationTool()
        //    //{
        //    //    Type = (DurationToolType)11,
        //    //    Factor = 2
        //    //}
        //};

        public override void OnRegister(GameDataObject gameDataObject)
        {
            Prefab.ApplyMaterialToChild("Cube.003", "Plastic - White", "Plastic - Grey");
        }
    }
}