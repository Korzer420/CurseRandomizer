using Modding;
using UnityEngine;

namespace CurseRandomizer.Curses;

public class FrailCurse : Curse
{
    public override void ApplyCurse() { }

    public override bool CanApplyCurse() => Data.CastedAmount < 99;

    public override void ApplyHooks() => ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;

    public override void Unhook() => ModHooks.AfterTakeDamageHook -= ModHooks_AfterTakeDamageHook;

    private int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        if (Data.CastedAmount > 0)
        { 
            if (Random.Range(0, 20) < DespairCurse.CastedDespair)
                HeroController.instance.ClearMP();
            else
                HeroController.instance.TakeMP(Data.CastedAmount + 99);
        }
        return damageAmount;
    }
}
