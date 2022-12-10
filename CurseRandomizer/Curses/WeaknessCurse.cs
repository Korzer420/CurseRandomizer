using CurseRandomizer.Helper;
using Modding;
using System;

namespace CurseRandomizer.Curses;

internal class WeaknessCurse : Curse
{
    #region Event handler

    private int ModifyNailDamage(string name, int originalValue)
    {
        if (name == "nailDamage")
            originalValue = Math.Max(1, originalValue - Data.CastedAmount);
        return originalValue;
    }

    private void IntOperator_OnEnter(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntOperator self)
    {
        if (self.IsCorrectContext("Shade Control", null, "Init"))
            self.integer1.Value += Data.CastedAmount;
        orig(self);
    }

    #endregion

    #region Control

    public override void ApplyHooks() 
    { 
        ModHooks.GetPlayerIntHook += ModifyNailDamage;
        On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += IntOperator_OnEnter;
    }

    public override void Unhook() 
    { 
        ModHooks.GetPlayerIntHook -= ModifyNailDamage;
        On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter -= IntOperator_OnEnter;
    }

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 1;
        return 5 + 4 * PlayerData.instance.GetInt(nameof(PlayerData.instance.nailSmithUpgrades)) - Data.CastedAmount > cap;
    }

    public override void ApplyCurse() => PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 20));

    #endregion
}
