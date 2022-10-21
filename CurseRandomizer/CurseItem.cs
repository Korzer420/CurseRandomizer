using ItemChanger;
using System;

namespace CurseRandomizer;

internal class CurseItem : AbstractItem
{
    public CurseType CurseType { get; set; }

    public override void GiveImmediate(GiveInfo info)
    {
        Curse curse = CurseManager.GetCurseOfType(CurseType);
        if (curse.CanCastCurse())
            curse.ApplyCurse.Invoke();
        else if (CurseManager.DefaultCurse.CanCastCurse())
            CurseManager.DefaultCurse.CanCastCurse();
        else
            CurseManager.GetCurseOfType(CurseType.Desorientation).ApplyCurse();
    }
}
