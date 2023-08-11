using HutongGames.PlayMaker.Actions;
using ItemChanger.Modules;
using KorzUtils.Helper;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace CurseRandomizer.Modules;

public class VesselModule : Module
{
    #region Properties

    public int SoulVessel { get; set; } = 2;

    #endregion

    #region Vessel Handler

    private void AdjustSoulAmount(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "Soul Orb Control" && self.gameObject.name == "Soul Orb" && SoulVessel < 2)
            self.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 0 ? 0.0513f : 0.02736f;
        orig(self);
    }

    private void LimitSoul(ILContext il)
    {
        try
        {
            ILCursor cursor = new(il);
            cursor.Goto(0);

            if (cursor.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<PlayerData>("GetBool")))
            {
                cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel == 1);
                if (cursor.TryGotoNext(MoveType.After,
                    x => x.MatchLdstr("soulLimited"),
                    x => x.MatchCallvirt<PlayerData>("GetBool")))
                {
                    cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel < 2);
                    if (cursor.TryGotoNext(MoveType.After,
                        x => x.MatchCall(typeof(BossSequenceController).FullName, "get_BoundSoul")))
                        cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel == 0);
                }
            }
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.LogError("An error occured while trying to modify soul limit at: " + exception.StackTrace);
        }
    }

    private void HookVesselGain(On.HeroController.orig_AddToMaxMPReserve orig, HeroController self, int amount)
    {
        try
        {
            if (amount == -1)
            {
                if (PDHelper.MPReserveMax > 0)
                    orig(self, amount);
                else
                    ChangeMaxMP(-1);
            }
            else if (SoulVessel < 2)
                ChangeMaxMP(1);
            else
                orig(self, amount);
        }
        catch (Exception)
        {
            CurseRandomizer.Instance.LogError("Couldn't update soul vessel.");
            orig(self, amount);
        }
    }

    private void FixVesselEyes(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, IntCompare self)
    {
        if (self.IsCorrectContext("Soul Orb Control", "Soul Orb", "Check Eyes"))
            self.integer2.Value = SoulVessel == 0 ? 17 : (SoulVessel == 1 ? 33 : 55);
        orig(self);
    }

    #endregion

    #region Methods

    public override void Initialize()
    {
        IL.PlayerData.AddMPCharge += LimitSoul;
        On.PlayMakerFSM.OnEnable += AdjustSoulAmount;
        On.HeroController.AddToMaxMPReserve += HookVesselGain;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += FixVesselEyes;
    }

    public override void Unload()
    {
        IL.PlayerData.AddMPCharge -= LimitSoul;
        On.PlayMakerFSM.OnEnable -= AdjustSoulAmount;
        On.HeroController.AddToMaxMPReserve -= HookVesselGain;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= FixVesselEyes;
    }

    private void ChangeMaxMP(int increase)
    {
        PlayMakerFSM fsm = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas/Soul Orb").gameObject.LocateMyFSM("Soul Orb Control");
        if (fsm == null)
        {
            CurseRandomizer.Instance.LogError("Couldn't update soul vessel. (Soul fsm not found)");
            return;
        }
        SoulVessel += increase;
        fsm.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 1 ? 0.02736f : (SoulVessel == 2 ? 0.0171f : 0.0513f);
        PDHelper.MaxMP = SoulVessel == 1 ? 66 : (SoulVessel == 2 ? 99 : 33);
    }

    #endregion
}
