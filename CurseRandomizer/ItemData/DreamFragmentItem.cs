using CurseRandomizer.Manager;
using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer.ItemData;

internal class DreamFragmentItem : AbstractItem
{
    public override void GiveImmediate(GiveInfo info)
    {
        if (ModManager.DreamUpgrade == 0)
            (UIDef as MsgUIDef).name = new BoxedString("Dream Nail Fight Fragment");
        else
            (UIDef as MsgUIDef).name = new BoxedString("Dream Nail Piercing Fragment");
        ModManager.DreamUpgrade++;
    }
}
