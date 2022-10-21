using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer;

/// <summary>
/// An item, which appears as a normal item, but applies a curse.
/// </summary>
internal class CurseItem : AbstractItem
{
    public Curse Curse { get; set; }

    public override void GiveImmediate(GiveInfo info)
    {
        string curseName = Curse.Name;
        if (Curse.CanCastCurse())
            Curse.CastCurse();
        else if (CurseManager.DefaultCurse.CanCastCurse())
        {
            CurseManager.DefaultCurse.CastCurse();
            curseName = CurseManager.DefaultCurse.Name;
        }
        else
        { 
            CurseManager.GetCurseByType(CurseType.Desorientation).CastCurse();
            curseName = "Desorientation";
        }
        (UIDef as MsgUIDef).name = new BoxedString($"<color=#c034eb>{curseName}</color>");
    }
}
