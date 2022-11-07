using CurseRandomizer.Helper;
using CurseRandomizer.Manager;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class StupidityCurse : Curse
{
    public StupidityCurse()
    {
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
        On.HutongGames.PlayMaker.Actions.SendMessage.OnEnter += SendMessage_OnEnter;
    }

    public int Stacks { get; set; }

    private void SendMessage_OnEnter(On.HutongGames.PlayMaker.Actions.SendMessage.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SendMessage self)
    {
        if (self.functionCall?.FunctionName == "TakeMP" && self.Fsm.Name == "Spell Control")
        {
            int baseValue = self.functionCall.IntParameter.Value;
            self.functionCall.IntParameter.Value = Mathf.Min(99, baseValue + Stacks);
            orig(self);
            self.functionCall.IntParameter.Value = baseValue;
        }
        else
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
        }
        else
            orig(self);
    }

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 99;
        if (ModManager.SoulVessel == 0)
            cap = 33;
        else if (ModManager.SoulVessel == 1)
            cap = Mathf.Min(cap, 66);
        return 33 + Stacks < cap;
    }

    public override void ApplyCurse() => Stacks++;

    public override void LoadData(object data) => Stacks = (int)data;

    public override object ParseData() => Stacks;

    public override void ResetData() => Stacks = 0;
}
