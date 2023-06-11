using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Preferences;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class SledgehammerMenu<T> : KLMenu<T>
    {
        public SledgehammerMenu(Transform container, ModuleList moduleList) : base(container, moduleList)
        {

        }

        public override void Setup(int player_id)
        {
            AddLabel("Sledgehammer");
            AddInfo("Changing these settings only takes effect upon game restart.");

            New<SpacerElement>();

            AddLabel("Sledgehammer Rarity");
            AddSelect<int>(SledgehammerRarity);
            SledgehammerRarity.OnChanged += delegate (object _, int result)
            {
                PreferenceInt preferenceInt = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_RARITY);
                preferenceInt.Set(result);
                Mod.PrefManager.Save();
            };

            New<SpacerElement>();

            AddLabel("Sledgehammer Price");
            AddSelect<int>(SledgehammerPrice);
            SledgehammerPrice.OnChanged += delegate (object _, int result)
            {
                PreferenceInt preferenceInt = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_PRICE);
                preferenceInt.Set(result);
                Mod.PrefManager.Save();
            };

            New<SpacerElement>();

            AddLabel("Hammering Duration");
            AddSelect<int>(SledgehammerDuration);
            SledgehammerDuration.OnChanged += delegate (object _, int result)
            {
                PreferenceInt preferenceInt = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_DURATION);
                preferenceInt.Set(result);
                Mod.PrefManager.Save();
            };

            New<SpacerElement>();

            AddLabel("Consequence After Usage");
            AddSelect<int>(SledgehammerConsequence);
            SledgehammerConsequence.OnChanged += delegate (object _, int result)
            {
                PreferenceInt preferenceInt = Mod.PrefManager.GetPreference<PreferenceInt>(Mod.SLEDGEHAMMER_CONSEQUENCE);
                preferenceInt.Set(result);
                Mod.PrefManager.Save();
            };

            New<SpacerElement>(true);
            New<SpacerElement>(true);
            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate (int i)
            {
                this.RequestPreviousMenu();
            }, 0, 1f, 0.2f);
        }

        private Option<int> SledgehammerRarity = new Option<int>(
            new List<int> { 0, 1, 2 },
            (int)Mod.PrefManager.Get<PreferenceInt>(Mod.SLEDGEHAMMER_RARITY),
            new List<string> { "Common", "Uncommon", "Rare" }
            );

        private Option<int> SledgehammerPrice = new Option<int>(
            new List<int> { 0, 5, 20, 40, 60, 100, 250, 1250 },
            (int)Mod.PrefManager.Get<PreferenceInt>(Mod.SLEDGEHAMMER_PRICE),
            new List<string> { "0", "5", "20", "40", "60", "100", "250", "1250" }
            );

        private Option<int> SledgehammerDuration = new Option<int>(
            new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 },
            (int)Mod.PrefManager.Get<PreferenceInt>(Mod.SLEDGEHAMMER_DURATION),
            new List<string> { "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60" }
            );

        private Option<int> SledgehammerConsequence = new Option<int>(
            new List<int> { 0, 1, 2 },
            (int)Mod.PrefManager.Get<PreferenceInt>(Mod.SLEDGEHAMMER_CONSEQUENCE),
            new List<string> { "Nothing", "Destroy Hammer", "Destroy Hammer & Provider" }
            );
    }
}