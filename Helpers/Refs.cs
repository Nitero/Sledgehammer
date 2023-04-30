using KitchenData;
using KitchenLib.References;

namespace KitchenSledgehammer
{
    internal class Refs
    {
        // Items
        public static Item Sledgehammer => Helper.Find<Item, Sledgehammer>();

        // Processes
        public static Process Research => Helper.Find<Process>(ProcessReferences.Upgrade);//TODO: replace with new custom process
        public static Process SledgehammerProcess => Helper.Find<Process, SledgehammerProcess>();

        // Appliances
        public static Appliance SledgehammerProvider => Helper.Find<Appliance, SledgehammerProvider>();
        public static Appliance HatchHammered => Helper.Find<Appliance, HatchHammered>();
    }
}