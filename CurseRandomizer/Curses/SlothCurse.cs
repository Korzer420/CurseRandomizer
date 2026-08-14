using KorzUtils.Helper;
using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;

namespace CurseRandomizer.Curses;

/// <summary>
/// Curse which increases dash, nail or spell cd.
/// </summary>
internal class SlothCurse : Curse
{
    #region Members

    private ILHook _attackHook; 

    #endregion

    #region Properties

    public int Stacks
    {
        get
        {
            if (Data.AdditionalData is null)
                Data.AdditionalData = 0;
            return Convert.ToInt32(Data.AdditionalData);
        }
        set => Data.AdditionalData = value;
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        _attackHook = new(ReflectionHelper.GetMethodInfo(typeof(HeroController), "orig_DoAttack"), HeroController_DoAttack);
        On.HeroController.Start += HeroController_Start;
        On.HeroController.CharmUpdate += HeroController_CharmUpdate;
    }

    public override void Unhook()
    {
        _attackHook.Dispose();
        _attackHook = null;
        On.HeroController.Start -= HeroController_Start;
        On.HeroController.CharmUpdate -= HeroController_CharmUpdate;
    }

    public override void ApplyCurse() => Stacks++;

    public override bool CanApplyCurse() => true;

    #endregion

    #region Event handler

    private void HeroController_DoAttack(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME_CH")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Stacks * 0.05f));
        else
            CurseRandomizer.Instance.LogError("Couldn't find attack cooldown match");

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Stacks * 0.05f));
    }

    private void HeroController_CharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        orig(self);
        if (Data.DespairEnhanced > 0)
            ReflectionHelper.SetField<HeroController, float>(HeroController.instance,
                "nailChargeTime", Data.DespairEnhanced * 0.075f + (CharmHelper.EquippedCharm(KorzUtils.Enums.CharmRef.NailmastersGlory)
                ? HeroController.instance.NAIL_CHARGE_TIME_CHARM
                : HeroController.instance.NAIL_CHARGE_TIME_DEFAULT));
    }

    private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        if (Data.DespairEnhanced > 0)
            ReflectionHelper.SetField<HeroController, float>(HeroController.instance,
                "nailChargeTime", Data.DespairEnhanced * 0.075f + (CharmHelper.EquippedCharm(KorzUtils.Enums.CharmRef.NailmastersGlory)
                ? HeroController.instance.NAIL_CHARGE_TIME_CHARM
                : HeroController.instance.NAIL_CHARGE_TIME_DEFAULT));
    }

    #endregion
}
