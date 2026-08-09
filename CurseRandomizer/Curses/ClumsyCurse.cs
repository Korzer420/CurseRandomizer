using Modding;
using UnityEngine;

namespace CurseRandomizer.Curses;

public class ClumsyCurse : Curse
{
    #region Control
    
    /// <inheritdoc/>
    public override bool CanApplyCurse() => Data.CastedAmount < 99;

    /// <inheritdoc/>
    public override void ApplyHooks() => ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;

    /// <inheritdoc/>
    public override void Unhook() => ModHooks.AfterTakeDamageHook -= ModHooks_AfterTakeDamageHook;

    #endregion

    #region Event handler

    private int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        if (Data.CastedAmount > 0)
        {
            if (Random.Range(0, 20) < Data.DespairEnhanced)
                HeroController.instance.ClearMP();
            else
                HeroController.instance.TakeMP(Data.CastedAmount);
        }
        return damageAmount;
    }

    #endregion
}
