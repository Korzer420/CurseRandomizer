using Benchwarp;

namespace CurseRandomizer.Curses;

internal class DesorientationCurse : Curse
{
    public override void ApplyCurse() => ChangeScene.WarpToRespawn();
}
