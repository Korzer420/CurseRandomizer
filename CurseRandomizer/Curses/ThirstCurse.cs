using Modding;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ThirstCurse : Curse
{
    #region Members

    private int _stepCounter = 0;

    #endregion

    #region Properties

    public int Stacks
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = 0;
            return Convert.ToInt32(Data.AdditionalData);
        }
        set => Data.AdditionalData = value;
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    { 
        ModHooks.SoulGainHook += ModHooks_SoulGainHook;
        _stepCounter = 0;
    }

    public override void Unhook() => ModHooks.SoulGainHook -= ModHooks_SoulGainHook;

    public override bool CanApplyCurse() => Stacks < 10;

    public override void ApplyCurse() => Stacks++;

    #endregion

    #region Event handler

    private int ModHooks_SoulGainHook(int soulGain)
    {
        soulGain = Math.Max(1, soulGain - Stacks);
        _stepCounter++;
        if (Data.DespairEnhanced > 0 && Math.Max(2, 21 - Data.DespairEnhanced) < _stepCounter)
        {
            _stepCounter = 0;
            return 0;
        }
        return soulGain;
    }

    #endregion
}