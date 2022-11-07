using Benchwarp;

namespace CurseRandomizer.Curses;

internal class DisorientationCurse : Curse
{
    public override void ApplyCurse() => ChangeScene.WarpToRespawn();
}
