using CurseRandomizer.Modules;
using ItemChanger;
using KorzUtils.Helper;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class StupidityCurse : Curse
{
    #region Properties

    public static int SpellCost => 33 + CurseManager.GetCurse<StupidityCurse>().Stacks;

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
        int cap = 99;
        VesselModule vesselModule = ItemChangerMod.Modules.GetOrAdd<VesselModule>();
        if (vesselModule.SoulVessel == 0)
            return false;
        else if (vesselModule.SoulVessel == 1)
            cap = 66;
        return SpellCost + Stacks + 3 <= cap;
    }

    public override void ApplyCurse() => Stacks += 3;

    #endregion

    #region Event handler

    private void SendMessage_OnEnter(On.HutongGames.PlayMaker.Actions.SendMessage.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SendMessage self)
    {
        if (self.functionCall?.FunctionName == "TakeMP" && self.Fsm.Name == "Spell Control")
        {
            int baseValue = self.functionCall.IntParameter.Value;
            self.functionCall.IntParameter.Value = Mathf.Min(99, baseValue + Stacks);
            if (UnityEngine.Random.Range(0, 20) < Data.DespairEnhanced)
                self.functionCall.IntParameter.Value = 99;
            orig(self);
            self.functionCall.IntParameter.Value = baseValue;
            return;
        }

        if ((self.IsCorrectContext("Spell Control", "Knight", "Focus Heal") || self.IsCorrectContext("Spell Control", "Knight", "Focus Heal 2")) && Stacks > 0)
        {
            if (UnityEngine.Random.Range(0, 20) < Data.DespairEnhanced)
                HeroController.instance.ClearMP();
            else
                HeroController.instance.TakeMP(Stacks);
        }
        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", "Can Cast? QC") || self.IsCorrectContext("Spell Control", "Knight", "Can Cast?"))
        {
            int baseValue = self.integer2.Value;
            self.integer2.Value = Mathf.Min(99, baseValue + Stacks);
            orig(self);
            self.integer2.Value = baseValue;
            return;
        }

        if (self.IsCorrectContext("Spell Control", "Knight", "Can Focus?"))
            self.integer2.Value += Stacks;
        orig(self);
    }

    #endregion
}
