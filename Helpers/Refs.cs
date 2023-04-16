using KitchenData;

namespace KitchenSledgehammer
{
    internal class Refs
    {
        // Items
        //public static Item Sledgehammer => Find<Item, Sledgehammer>();

        // Appliances
        //public static Appliance SledgehammerProvider => Find<Appliance, SledgehammerProvider>();
        public static Appliance HatchHammered => Helper.Find<Appliance, HatchHammered>();
    }
}