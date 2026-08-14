using Modding;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

public class ClumsyCurse : Curse
{
    #region Properties

    public int Stacks
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = 0;
            return (int)Data.AdditionalData;
        }
        set => Data.AdditionalData = value;
    }

    #endregion

    #region Control

    /// <inheritdoc/>
    public override bool CanApplyCurse() => Stacks < 99;

    /// <inheritdoc/>
    public override void ApplyHooks() => ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;

    /// <inheritdoc/>
    public override void Unhook() => ModHooks.AfterTakeDamageHook -= ModHooks_AfterTakeDamageHook;

    public override void ApplyCurse() => Stacks++;

    #endregion

    #region Event handler

    private int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        if (Data.CastedAmount > 0)
        {
            if (UnityEngine.Random.Range(0, 20) < Data.DespairEnhanced)
                HeroController.instance.ClearMP();
            else
                HeroController.instance.TakeMP(Data.CastedAmount);
        }
        return damageAmount;
    }

    #endregion
}
