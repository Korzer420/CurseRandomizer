using Modding;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ThirstCurse : Curse
{
    public ThirstCurse() => ModHooks.SoulGainHook += ModHooks_SoulGainHook;
    
    private int ModHooks_SoulGainHook(int soulGain) => Mathf.Max(1, soulGain - Stacks);
    
    public int Stacks { get; set; }

    public override bool CanApplyCurse()
    {
        int cap = CurseRandomizer.Instance.Settings.CapEffects ? Cap : 1;
        return 11 - Stacks > cap;
    }

    public override void ApplyCurse() => Stacks++;

    public override object ParseData() => Stacks;

    public override void LoadData(object data) => Stacks = (int)data;

    public override void ResetData() => Stacks = 0;
}
