using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse which lowers the nail range.
/// </summary>
internal class DiminishCurse : Curse 
{
    #region Event handler

    private void NailSlash_StartSlash(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
        self.transform.localScale -= new Vector3(0.1f * Data.CastedAmount, 0.1f * Data.CastedAmount);
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.NailSlash.StartSlash += NailSlash_StartSlash;
    }

    public override void Unhook()
    {
        On.NailSlash.StartSlash -= NailSlash_StartSlash;
    }

    public override void ApplyCurse() { }

    public override bool CanApplyCurse()
    {
        int cap = CurseManager.UseCaps ? Data.Cap : 8;
        return Data.CastedAmount < cap;
    }

    public override int SetCap(int value)
    => Math.Max(1, Math.Min(value, 8)); 

    #endregion
}
