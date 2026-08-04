using System;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse that makes other curses worse
/// </summary>
internal class DespairCurse : Curse
{
    #region Properties

    public static int CastedDespair
    { 
        get
        {
            DespairCurse despair = CurseManager.GetCurse<DespairCurse>();
            if (despair.Data.Active)
                return despair.Data.CastedAmount;
            else
                return 0;
        }
    }

    #endregion

    #region Control

    public override bool CanApplyCurse() => true;

    public override void ApplyCurse() { }

    #endregion
}
