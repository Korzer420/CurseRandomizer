using KorzUtils.Helper;
using Modding;
using System;

namespace CurseRandomizer.Curses;

internal class WeaknessCurse : Curse
{
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

    #region Event handler

    private int ModifyNailDamage(string name, int originalValue)
    {
        if (name == "nailDamage")
            originalValue = Math.Max(1, originalValue - Stacks);
        return originalValue;
    }

    private void IntOperator_OnEnter(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntOperator self)
    {
        if (self.IsCorrectContext("Shade Control", null, "Init"))
            self.integer1.Value += Stacks;
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

    public override bool CanApplyCurse() => 5 + 4 * PDHelper.NailSmithUpgrades - Stacks > 1;

    public override void ApplyCurse()
    {
        Stacks += 1 + DespairCurse.CastedDespair;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    #endregion
}
