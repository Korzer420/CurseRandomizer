using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Curses;

/// <summary>
/// Curse which increases dash, nail or spell cd.
/// </summary>
internal class SlothCurse : Curse
{
    private ILHook _attackHook;

    #region Event handler

    private void HeroController_DoAttack(MonoMod.Cil.ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME_CH")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Data.CastedAmount * 0.1f));
        else
            CurseRandomizer.Instance.LogError("Couldn't find attack cooldown match");

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("ATTACK_COOLDOWN_TIME")))
            cursor.EmitDelegate<Func<float, float>>(x => x + (Data.CastedAmount * 0.1f));
    }

    private void HeroController_HeroDash(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("DASH_COOLDOWN_CH")))
            cursor.EmitDelegate<Func<float, float>>(x => x + Data.CastedAmount * 0.1f);
        else
            CurseRandomizer.Instance.LogError("Couldn't find dash cooldown ch logic");

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("DASH_COOLDOWN")))
            cursor.EmitDelegate<Func<float, float>>(x => x + Data.CastedAmount * 0.1f);
        else
            CurseRandomizer.Instance.LogError("Couldn't find dash cooldown logic");
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        _attackHook = new(ReflectionHelper.GetMethodInfo(typeof(HeroController), "orig_DoAttack"), HeroController_DoAttack);
        IL.HeroController.HeroDash += HeroController_HeroDash;
    }

    public override void Unhook()
    {
        _attackHook.Dispose();
        _attackHook = null;
        IL.HeroController.HeroDash -= HeroController_HeroDash;
    }

    public override void ApplyCurse() { }

    public override bool CanApplyCurse() => Data.CastedAmount < (CurseManager.UseCaps ? Cap : 30);

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 30));

    #endregion
}
