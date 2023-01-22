using CurseRandomizer.Helper;
using CurseRandomizer.Manager;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class StupidityCurse : Curse
{
    #region Event handler

    private void SendMessage_OnEnter(On.HutongGames.PlayMaker.Actions.SendMessage.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SendMessage self)
    {
        if (self.functionCall?.FunctionName == "TakeMP" && self.Fsm.Name == "Spell Control")
        {
            int baseValue = self.functionCall.IntParameter.Value;
            self.functionCall.IntParameter.Value = Mathf.Min(99, baseValue + Data.CastedAmount * 3);
            orig(self);
            self.functionCall.IntParameter.Value = baseValue;
            return;
        }

        if ((self.IsCorrectContext("Spell Control", "Knight", "Focus Heal") || self.IsCorrectContext("Spell Control", "Knight", "Focus Heal 2")) && Data.CastedAmount > 0)
            HeroController.instance.TakeMP(Data.CastedAmount * 3);
        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", "Can Cast? QC") || self.IsCorrectContext("Spell Control", "Knight", "Can Cast?"))
        {
            int baseValue = self.integer2.Value;
            self.integer2.Value = Mathf.Min(99, baseValue + Data.CastedAmount * 3);
            orig(self);
            self.integer2.Value = baseValue;
            return;
        }

        if (self.IsCorrectContext("Spell Control", "Knight", "Can Focus?"))
            self.integer2.Value += Data.CastedAmount * 3;
            orig(self);
    } 

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
        On.HutongGames.PlayMaker.Actions.SendMessage.OnEnter += SendMessage_OnEnter;

    }

    public override void Unhook()
    {
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= IntCompare_OnEnter;
        On.HutongGames.PlayMaker.Actions.SendMessage.OnEnter -= SendMessage_OnEnter;
    }

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 99;
        if (ModManager.SoulVessel == 0)
            return false;
        else if (ModManager.SoulVessel == 1)
            cap = Mathf.Min(cap, 66);
        return 33 + Data.CastedAmount * 3 < cap;
    }

    public override void ApplyCurse() { }

    public override int SetCap(int value) => Math.Max(36, Math.Min(value, 99)); 

    #endregion
}
