using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse which lowers the nail range.
/// </summary>
internal class DiminishCurse : Curse 
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

    #region Control

    public override void ApplyHooks() => On.NailSlash.StartSlash += NailSlash_StartSlash;

    public override void Unhook() => On.NailSlash.StartSlash -= NailSlash_StartSlash;

    public override void ApplyCurse() => Stacks = Math.Min(16, Stacks + 1 + Data.DespairEnhanced);

    public override bool CanApplyCurse() => Stacks < 16;

    #endregion

    #region Event handler

    private void NailSlash_StartSlash(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
        self.transform.localScale -= new Vector3(0.05f * Stacks, 0.05f * Stacks);
    }

    #endregion
}
