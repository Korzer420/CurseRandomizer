using CurseRandomizer.Manager;
using CurseRandomizer.Modules;
using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer.ItemData;

public class DreamFragmentItem : AbstractItem
{
    protected override void OnLoad() => ItemChangerMod.Modules.GetOrAdd<DreamNailModule>().AddFragment(this);
    
    public override void GiveImmediate(GiveInfo info)
    {
        DreamNailModule module = ItemChangerMod.Modules.GetOrAdd<DreamNailModule>();
        if (module.DreamUpgrade == 0)
            (UIDef as MsgUIDef).name = new BoxedString("Dream Nail Fight Fragment");
        else
            (UIDef as MsgUIDef).name = new BoxedString("Dream Nail Piercing Fragment");
    }
}
