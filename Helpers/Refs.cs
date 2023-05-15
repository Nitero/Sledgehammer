using KitchenData;
using KitchenLib.References;

namespace KitchenSledgehammer
{
    internal class Refs
    {
        // Items
        public static Item Sledgehammer => Helper.Find<Item, Sledgehammer>();

        // Processes
        public static Process SledgehammerProcess => Helper.Find<Process, SledgehammerProcess>();

        // Appliances
        public static Appliance SledgehammerProvider => Helper.Find<Appliance, SledgehammerProvider>();
        public static Appliance WallReplaced => Helper.Find<Appliance, WallReplaced>();
    }
}