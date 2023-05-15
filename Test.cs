using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static KitchenSledgehammer.HatchHammered;

namespace KitchenSledgehammer
{
    [HarmonyPatch]
    public static class Test
    {
        //[HarmonyPatch(typeof(Process), "CompletionCallback")]
        //[HarmonyPrefix]
        //static bool CompletionCallback_Prefix(ref int ___processId, object context, bool wasSignaled)
        //{
        //    if (___processId == Refs.SledgehammerProcess.ID)
        //    {
        //        Debug.Log("FIN");
        //        return false;
        //    }
        //    return true;//run original
        //}
        //static void CompletionCallback_Prefix(object context, bool wasSignaled)
        //{
        //    Debug.Log("FIN");
        //    //return true;//run original
        //}
        //[HarmonyPatch(typeof(Process), "CloseMainWindow")]
        //[HarmonyPrefix]
        //static bool CloseMainWindow_Prefix(ref bool __result)//why cant patch any process method??
        //{
        //    Debug.Log("A");
        //    return true;
        //    //return true;//run original
        //}
        //[HarmonyPatch(typeof(Process), "OnExited")]
        //[HarmonyPrefix]
        //static bool OnExited_Prefix()//why cant patch any process method?? oh its the wrong process class? so what else to patch? or how to access this from my custom process? ApplyItemProcesses CCompletedProcess
        //{
        //    Debug.Log("A");
        //    return true;
        //    //return true;//run original
        //}

        //[HarmonyPatch(typeof(ClearProcessComplete), "OnUpdate")]
        //[HarmonyPrefix]
        //static bool OnUpdate_Prefix()
        //{
        //    //if (___processId == Refs.SledgehammerProcess.ID)
        //    //{
        //    Debug.Log("FIN");//happens, but what process?
        //    //    return false;
        //    //}
        //    return true;//run original
        //}

        //[HarmonyPatch(typeof(FinishCarriedProcesses), "OnUpdate")]
        //[HarmonyPrefix]
        //static bool OnUpdate_Prefix()
        //{
        //    //if (___processId == Refs.SledgehammerProcess.ID)
        //    //{
        //    Debug.Log("FIN");//happens, but what process?
        //    //    return false;
        //    //}
        //    return true;//run original
        //}

        //[HarmonyPatch(typeof(DestroyAfterDuration), "OnUpdate")]
        //[HarmonyPrefix]
        //static bool OnUpdate_Prefix()
        //{
        //    //if (___processId == Refs.SledgehammerProcess.ID)
        //    //{
        //    Debug.Log("FIN");//happens, but what process?
        //    //    return false;
        //    //}
        //    return true;//run original
        //}

        //dont work: CApplyingProcess


        //[HarmonyPatch(typeof(InteractionSystem), "OnUpdate")]
        //[HarmonyPrefix]
        //static bool OnUpdate_Prefix()
        //{
        //    Debug.Log("CANCEL");
        //    return false;//run original
        //}
        //[HarmonyPatch(typeof(InteractionSystem), "ShouldAct")]
        //[HarmonyPrefix]
        //static bool ShouldAct_Prefix(ref bool __result, InteractionData interaction_data) //prevents hammering wall with hands during day
        //{
        //    if (interaction_data.Interactor.Index == Refs.Sledgehammer.ID)
        //    {
        //        Debug.Log("CANCEL");
        //        __result = false;
        //        return false;//run original
        //    }

        //    __result = true;
        //    return true;//run original
        //}

        //[HarmonyPatch(typeof(HighlightInteraction), "IsPossible")]
        //[HarmonyPostfix]
        //static bool IsPossible_Postfix(ref bool __result, InteractionData data)
        //{
        //    if (data.Interactor.Index == Refs.Sledgehammer.ID)
        //    {
        //        Debug.Log("CANCEL");
        //        __result = false;
        //        return false;//run original
        //    }

        //    __result = true;
        //    return true;//run original
        //}

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(InteractionSystem), "IsPossible")]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        //static bool BaseMethodDummy(HighlightInteraction instance, ref InteractionData data) { return false; }

        //[HarmonyPatch(typeof(HighlightInteraction), "IsPossible")]
        ////[HarmonyPostfix]
        //static void IsPossible_Postfix(HighlightInteraction __instance, ref bool __result, ref InteractionData data)
        //{
        //    var b = BaseMethodDummy(__instance, ref data);
        //    Debug.Log("TTTTTTEST " + b);
        //    //return BaseMethodDummy(__instance, ref data);
        //}

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(InteractionSystem), "IsPossible")]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        //static bool IsPossibleDummy(ApplianceInteractionSystem instance, ref InteractionData data) { return false; }

        //[HarmonyPatch(typeof(ApplianceInteractionSystem), "IsPossible")]
        //[HarmonyPostfix]
        //static void IsPossible_Postfix(ApplianceInteractionSystem __instance, ref bool __result, ref InteractionData data)
        //{
        //    var b = IsPossibleDummy(__instance, ref data);
        //    Debug.Log("TTTTTTEST " + b);
        //    //return BaseMethodDummy(__instance, ref data);
        //}



        //[HarmonyPatch(typeof(MakePing), "Perform")]
        //[HarmonyPostfix]
        //public static void Perform_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("ping Perform");
        //}
        //[HarmonyPatch(typeof(MakePing), "IsPossible")]
        //[HarmonyPostfix]
        //public static void IsPossibleA_Postfix(ref bool __result, ref InteractionData data)
        //{
        //    Debug.Log("ping IsPossible");
        //    __result = false;
        //}

        //[HarmonyPatch(typeof(HighlightInteraction), "IsPossible")]
        //[HarmonyPostfix]
        //public static void IsPossibleB_Postfix(ref bool __result, ref InteractionData data)
        //{
        //    Debug.Log("HighlightInteraction IsPossible");
        //    __result = false;
        //}

        //[HarmonyPatch(typeof(TakeFromHolder), "Perform")]
        //[HarmonyPostfix]
        //static void A_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("A");
        //}
        //[HarmonyPatch(typeof(TakeFromProvider), "Perform")]
        //[HarmonyPostfix]
        //static void B_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("B");
        //}
        //[HarmonyPatch(typeof(TakeFromToolUser), "Perform")]
        //[HarmonyPostfix]
        //static void C_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("C");
        //}
        //[HarmonyPatch(typeof(InteractRotatePush), "Perform")]
        //[HarmonyPostfix]
        //static void D_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("D");
        //}
        //[HarmonyPatch(typeof(PickUpAndDropAppliance), "Perform")]
        //[HarmonyPostfix]
        //static void E_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("E");
        //}
        //[HarmonyPatch(typeof(PickUpAppliance), "Perform")]
        //[HarmonyPostfix]
        //static void F_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("F");
        //}
        //[HarmonyPatch(typeof(PerformDirectProviderInsertion), "Perform")]
        //[HarmonyPostfix]
        //static void G_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("G");
        //}
        //[HarmonyPatch(typeof(TriggerActivation), "Perform")]
        //[HarmonyPostfix]
        //static void H_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("H");
        //}
        //[HarmonyPatch(typeof(PseudoInteractProcess), "Perform")]
        //[HarmonyPostfix]
        //static void I_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("I");
        //}
        //[HarmonyPatch(typeof(ApplianceInteractionSystem), "Perform")]
        //[HarmonyPostfix]
        //static void J_Postfix(ref InteractionData data)
        //{
        //    Debug.Log("J");
        //}
    }



    //public class SetInteractableSystem : DaySystem, IModSystem
    //{
    //    private EntityQuery ReplacedWallsQuery;

    //    protected override void Initialise()
    //    {
    //        base.Initialise();
    //        ReplacedWallsQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallHasBeenReplaced)));
    //    }

    //    protected override void OnUpdate()
    //    {
    //        Debug.Log("DAY");
    //        NativeArray<Entity> replacedWalls = ReplacedWallsQuery.ToEntityArray(Allocator.TempJob);
    //        foreach (Entity replacedWall in replacedWalls)
    //        {
    //            if (EntityManager.RequireComponent<CIsInteractive>(replacedWall, out CIsInteractive cIsInteractive))
    //                continue;
    //            EntityManager.AddComponent<CIsInteractive>(replacedWall);
    //            break;
    //        }
    //        replacedWalls.Dispose();
    //    }
    //}
    //public class SetNonInteractableSystem : NightSystem, IModSystem
    //{
    //    private EntityQuery ReplacedWallsQuery;

    //    protected override void Initialise()
    //    {
    //        base.Initialise();
    //        ReplacedWallsQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallHasBeenReplaced)));
    //    }

    //    protected override void OnUpdate()
    //    {
    //        Debug.Log("NIGHT");
    //        NativeArray<Entity> replacedWalls = ReplacedWallsQuery.ToEntityArray(Allocator.TempJob);
    //        foreach (Entity replacedWall in replacedWalls)
    //        {
    //            if (!EntityManager.RequireComponent<CIsInteractive>(replacedWall, out CIsInteractive cIsInteractive))
    //                continue;
    //            EntityManager.RemoveComponent<CIsInteractive>(replacedWall);
    //            break;
    //        }
    //        replacedWalls.Dispose();
    //    }
    //}



    [HarmonyPatch]
    public static class CanReachOverHammeredWallPatch
    {
        [HarmonyPatch(typeof(GenericSystemBase), "CanReach")]
        [HarmonyPrefix]
        static bool CanReach_Prefix(ref bool __result, Vector3 from, Vector3 to, bool do_not_swap = false)
        {
            if (SReplaceWalls.Instance.IsReplacedWallHammeredBetween(from, to))//TODO: instead if the wall wasnt hammered yet and I have the proper tool let me target it, but if it is already hammered can reach over it
            {
                __result = true;
                return false;
            }
            return true;//run original
        }
    }

    public class HammerWallSystem : GenericSystemBase, IModSystem
    {
        private EntityQuery ReplacedWallsQuery;

        protected override void Initialise()
        {
            base.Initialise();
            ReplacedWallsQuery = GetEntityQuery(new QueryHelper().All(typeof(CWallHasBeenReplaced)));
        }

        protected override void OnUpdate()
        {
            //Debug.Log(GameInfo.CurrentScene);
            //if (GameInfo.CurrentScene == SceneType.Kitchen)//TODO: then doesnt work on franchise, maybe other stuff too?
            //    return;

            NativeArray<Entity> replacedWalls = ReplacedWallsQuery.ToEntityArray(Allocator.TempJob);
            foreach (Entity replacedWall in replacedWalls)
            {
                if (!EntityManager.RequireComponent<CWallHasBeenReplaced>(replacedWall, out CWallHasBeenReplaced cReplacedWall))
                    continue;
                if (!EntityManager.RequireComponent<CTakesDuration>(replacedWall, out CTakesDuration cTakesDuration))
                    continue;

                if (cTakesDuration.Remaining <= 0f && cTakesDuration.Active)//TODO: find a better way to check if completed
                {
                    Debug.Log("Took: " + cTakesDuration.Remaining);
                    //Debug.Log("REMAIN: " + cTakesDuration.Remaining);
                    //EntityManager.RemoveComponent<CIsInteractive>(replacedWall);
                    //EntityManager.RemoveComponent<CTakesDuration>(replacedWall);

                    EntityManager.AddComponent<CIsInactive>(replacedWall);
                    //EntityManager.RemoveComponent<CWallHasBeenReplaced>(replacedWall);
                    cReplacedWall.HasBeenHammered = true;
                    EntityManager.SetComponentData(replacedWall, cReplacedWall);

                    cTakesDuration.IsLocked = true;
                    cTakesDuration.Active = false;
                    EntityManager.SetComponentData(replacedWall, cTakesDuration);
                }
            }
            replacedWalls.Dispose();
        }
    }



    [UpdateInGroup(typeof(DurationLocks))]
    public class LockDurationAtNight : RestaurantSystem
    {
        EntityQuery WallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            WallQuery = GetEntityQuery(typeof(CTakesDuration), typeof(CWallHasBeenReplaced));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = WallQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CTakesDuration> durations = WallQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CTakesDuration duration = durations[i];

                if (Has<SIsNightTime>())
                {
                    duration.IsLocked = true;
                    Set(entity, duration);
                }
            }
        }
    }



    [UpdateBefore(typeof(ItemTransferGroup))]
    //public class HammerInteractionSystem : ItemInteractionSystem, IModSystem// ItemInteractionSystem works, but still can progress duration with hand //OR ApplianceInteractionSystem? nvm just at night
    public class HammerInteractionSystem : InteractionSystem, IModSystem
    {
        private CTakesDuration duration;
        //private CProgressIndicator duration;
        //private CDisplayDuration duration;
        private CWallHasBeenReplaced wallReplaced;
        //private CAttemptingInteraction attemptInteraction;
        private EntityQuery SledgehammerQuery;
        protected override bool RequireHold => true;
        protected override bool RequirePress => false;

        protected override void Initialise()
        {
            base.Initialise();
            SledgehammerQuery = GetEntityQuery(new QueryHelper().All(typeof(CItem)));
        }

        protected override bool IsPossible(ref InteractionData data)
        {
            //if (!Require(data.Target, out attemptInteraction))
            //    return false;
            if (!Require(data.Target, out wallReplaced))
                return false;
            if (wallReplaced.HasBeenHammered)
                return false;
            //if (!Require(data.Target, out duration))
            //    return false;

            NativeArray<Entity> sledgehammers = SledgehammerQuery.ToEntityArray(Allocator.TempJob);
            foreach (var sledgehammer in sledgehammers)
            {
                if (!EntityManager.RequireComponent<CItem>(sledgehammer, out CItem item))
                    continue;
                if (!EntityManager.RequireComponent<CHeldBy>(sledgehammer, out CHeldBy heldBy))
                    continue;

                if (item.ID == Refs.Sledgehammer.ID && heldBy.Holder == data.Interactor)//TODO: optimize this to no query?
                {
                    sledgehammers.Dispose();
                    //Debug.Log("POSSIBLE " + duration.Total + " / "+ duration.Remaining);
                    //duration.IsLocked = false;
                    //duration.Active = true;
                    //duration.Remaining = 0.5f;
                    //duration.Progress = 0.5f;
                    //duration.CurrentChange = 1f;
                    return true;
                }
            }
            sledgehammers.Dispose();
            Debug.Log("IMPOSSIBLE");
            //attemptInteraction.Result = 0;

            //duration.IsLocked = true;
            //duration.Active = false;
            //data.ShouldAct = false;
            //data.Attempt.Result = InteractionResult.None;
            return false;
        }

        protected override void Perform(ref InteractionData data)
        {

            if (Require(data.Target, out wallReplaced))
            {
                //duration.IsLocked = false;
                //duration.Active = true;
                wallReplaced.HasBeenHammerAttemptedToday = true;
                EntityManager.SetComponentData(data.Target, wallReplaced);
            }

            if (Require(data.Target, out duration))
            {
                Debug.Log("PERFORM " + duration.Remaining);
                duration.Manual = true;
                duration.IsLocked = false;
                //duration.Active = true;
                duration.Remaining -= 0.1f * 1f;
                duration.CurrentChange = 1f;
                EntityManager.SetComponentData(data.Target, duration);
            }

            //SReplaceWalls.Instance.Hammered(data.Target);
            //TODO: why does this only happen with no duration?
        }
    }


    [UpdateInGroup(typeof(DurationLocks))]
    public class LockDurationByTimeOfDay : RestaurantSystem
    {
        EntityQuery wallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            wallQuery = GetEntityQuery(typeof(CTakesDuration), typeof(CWallHasBeenReplaced));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = wallQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CTakesDuration> durations = wallQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
            using NativeArray<CWallHasBeenReplaced> walls = wallQuery.ToComponentDataArray<CWallHasBeenReplaced>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CTakesDuration duration = durations[i];
                CWallHasBeenReplaced wall = walls[i];

                //if (Has<SIsDayTime>() && time.LockDuringDay || Has<SIsNightTime>() && time.LockDuringNight)
                if (!wall.HasBeenHammerAttemptedToday)
                {
                    duration.IsLocked = true;
                    Set(entity, duration);
                }
            }
        }
    }


    //struct CWallLockDuration : IModComponent
    //public struct CWallLockDuration : IApplianceProperty, IAttachableProperty, IComponentData
    //{
    //}
}