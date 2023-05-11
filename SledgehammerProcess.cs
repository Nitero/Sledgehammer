using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class SledgehammerProcess : CustomProcess//CustomItemProcess CustomApplianceProcess CustomProcess
    {
        // The unique internal name of this process
        public override string UniqueNameID => "Hammer";

        // The "default" appliance of this process (e.g., counter for "chop", hob for "cook")
        // When a dish requires this process, this is the appliance that will spawn at the beginning of a run
        public override GameDataObject BasicEnablingAppliance => Refs.HatchHammered;

        // Whether or not the process can be obfuscated, such as through the "Blindfolded Chefs" card. This is normally set to `true`
        public override bool CanObfuscateProgress => true;


        // The localization information for this process. This must be set for at least one language. 
        //public override LocalisationObject<ProcessInfo> Info
        //{
        //    get
        //    {
        //        var info = new LocalisationObject<ProcessInfo>();

        //        info.Add(Locale.English, LocalisationUtils.CreateProcessInfo("Hammer", "<sprite name=\"cogpngwhitescaled\">"));
        //        return info;
        //    }
        //}
    }


    public class SledgehammerItemProcess : CustomItemProcess
    {
        //public override Process Process => 
        //public override Item Result { get; protected set; }
        //public override float Duration { get; protected set; }
        //public override bool IsBad { get; protected set; }
        //public override bool RequiresWrapper { get; protected set; }
    }

    public class SledgehammerApplianceProcess : CustomApplianceProccess
    {
        public override Process Process => Refs.SledgehammerProcess;
        public override bool IsAutomatic => false;
        public override float Speed => 1f;
        public override ProcessValidity Validity => ProcessValidity.Generic;
    }
}
