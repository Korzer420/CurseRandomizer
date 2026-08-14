using Benchwarp;
using CurseRandomizer.Enums;

namespace CurseRandomizer.Curses;

internal class DisorientationCurse : Curse
{
    #region Properties

    public override CurseTag Tag => CurseTag.Instant; 

    #endregion

    #region Control

    public override void ApplyCurse() => ChangeScene.WarpToRespawn(); 

    #endregion
}
