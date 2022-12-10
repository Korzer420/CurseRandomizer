using Modding;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ThirstCurse : Curse
{
    #region Event handler

    private int ModHooks_SoulGainHook(int soulGain) => Mathf.Max(1, soulGain - Data.CastedAmount);

    #endregion

    #region Control

    public override void ApplyHooks() => ModHooks.SoulGainHook += ModHooks_SoulGainHook;

    public override void Unhook() => ModHooks.SoulGainHook -= ModHooks_SoulGainHook;

    public override bool CanApplyCurse() => 11 - Data.CastedAmount > (UseCap ? Cap : 1);

    public override void ApplyCurse() { }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 10)); 

    #endregion
}