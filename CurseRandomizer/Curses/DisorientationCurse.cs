using Benchwarp;
using CurseRandomizer.Enums;

namespace CurseRandomizer.Curses;

internal class DisorientationCurse : Curse
{
    public override CurseTag Tag => CurseTag.Instant;
    
    public override void ApplyCurse() => ChangeScene.WarpToRespawn();

    public override int SetCap(int value) => 0;
}
