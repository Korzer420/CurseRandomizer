using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;

namespace CurseRandomizer.Curses;

/// <summary>
/// Curse which increases dash, nail or spell cd.
/// </summary>
internal class SlothCurse : Curse
{
    private ILHook _attackHook;

    #region Properties

    public List<int> Stacks
    {
        get
        {
            if (Data.AdditionalData is null)
                Data.AdditionalData = new List<int>() { 0, 0, 0, 0 };
            return Data.AdditionalData as List<int>;
        }
    }

    #endregion

    #region Event handler

    private void HeroController_DoAttack(MonoMod.Cil.ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME_CH")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Stacks[0] * 0.1f));
        else
            CurseRandomizer.Instance.LogError("Couldn't find attack cooldown match");

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Stacks[0] * 0.1f));
    }

    private void HeroController_HeroDash(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("DASH_COOLDOWN_CH")))
            cursor.EmitDelegate<Func<float, float>>(x => x + Stacks[1] * 0.1f);
        else
            CurseRandomizer.Instance.LogError("Couldn't find dash cooldown ch logic");

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("DASH_COOLDOWN")))
            cursor.EmitDelegate<Func<float, float>>(x => x + Stacks[1] * 0.1f);
        else
            CurseRandomizer.Instance.LogError("Couldn't find dash cooldown logic");
    }

    private void CallMethodProper_OnEnter(On.HutongGames.PlayMaker.Actions.CallMethodProper.orig_OnEnter orig, CallMethodProper self)
    {
        orig(self);
        if (self.IsCorrectContext("Superdash", null, "On Ground?"))
            HeroController.instance.superDash.FsmVariables.FindFsmFloat("Charge Time").Value += Stacks[2] * 0.15f;
        // This action does exist twice in the fsm state
        else if (self.IsCorrectContext("Superdash", null, "Regain Control"))
            HeroController.instance.superDash.FsmVariables.FindFsmFloat("Charge Time").Value -= Stacks[2] * 0.0725f;
    }

    private void HeroController_CharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        orig(self);
        if (Stacks[3] > 0)
            ReflectionHelper.SetField<HeroController, float>(HeroController.instance,
                "nailChargeTime", Stacks[3] * 0.15f + (CharmHelper.EquippedCharm(KorzUtils.Enums.CharmRef.NailmastersGlory)
                ? HeroController.instance.NAIL_CHARGE_TIME_CHARM
                : HeroController.instance.NAIL_CHARGE_TIME_DEFAULT));
    }

    private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        if (Stacks[3] > 0)
            ReflectionHelper.SetField<HeroController, float>(HeroController.instance,
                "nailChargeTime", Stacks[3] * 0.15f + (CharmHelper.EquippedCharm(KorzUtils.Enums.CharmRef.NailmastersGlory)
                ? HeroController.instance.NAIL_CHARGE_TIME_CHARM
                : HeroController.instance.NAIL_CHARGE_TIME_DEFAULT));
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        _attackHook = new(ReflectionHelper.GetMethodInfo(typeof(HeroController), "orig_DoAttack"), HeroController_DoAttack);
        IL.HeroController.HeroDash += HeroController_HeroDash;
        On.HutongGames.PlayMaker.Actions.CallMethodProper.OnEnter += CallMethodProper_OnEnter;
        On.HeroController.Start += HeroController_Start;
        On.HeroController.CharmUpdate += HeroController_CharmUpdate;
    }

    public override void Unhook()
    {
        _attackHook.Dispose();
        _attackHook = null;
        IL.HeroController.HeroDash -= HeroController_HeroDash;
        On.HutongGames.PlayMaker.Actions.CallMethodProper.OnEnter -= CallMethodProper_OnEnter;
        On.HeroController.Start -= HeroController_Start;
        On.HeroController.CharmUpdate -= HeroController_CharmUpdate;
    }

    public override void ApplyCurse() 
    {
        int selected = UnityEngine.Random.Range(0, 4);
        Stacks[selected]++;
        string message = selected switch
        {
            0 => "FOOL! (You slash slower)",
            1 => "FOOL! (You dash slower)",
            2 => "FOOL! (Your crystal heart got weaker)",
            _ => "FOOL! (You charge the nail slower)",
        };
        GameHelper.DisplayMessage(message);
    }

    public override bool CanApplyCurse() => Data.CastedAmount < (CurseManager.UseCaps ? Cap : 99);

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 99));

    #endregion
}
