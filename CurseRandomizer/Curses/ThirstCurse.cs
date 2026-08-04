using Modding;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ThirstCurse : Curse
{
    #region Members

    private int _stepCounter = 0;

    #endregion

    #region Event handler

    private int ModHooks_SoulGainHook(int soulGain)
    {
        _stepCounter++;
        if (11 - Data.CastedAmount < _stepCounter)
        {
            _stepCounter = 0;
            return 0;
        }
        if (DespairCurse.CastedDespair > 0)
            return Mathf.Max(1, soulGain - DespairCurse.CastedDespair);
        return soulGain;
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    { 
        ModHooks.SoulGainHook += ModHooks_SoulGainHook;
        _stepCounter = 0;
    }

    public override void Unhook() => ModHooks.SoulGainHook -= ModHooks_SoulGainHook;

    public override bool CanApplyCurse() => Data.CastedAmount < 10;

    public override void ApplyCurse() { }

    #endregion
}