using CurseRandomizer.Modules;
using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer.ItemData;

public class WalletItem : AbstractItem
{
    public override void GiveImmediate(GiveInfo info)
    {
        WalletModule module = ItemChangerMod.Modules.Get<WalletModule>();
        module.WalletAmount++;
        if (module.WalletAmount < module.Capacities.Length)
        {
            (UIDef as MsgUIDef).name = new BoxedString("Wallet (" + module.Capacities[module.WalletAmount] + ")");
            HeroController.instance.AddGeo(module.Capacities[module.WalletAmount]);
        }
        else
        { 
            (UIDef as MsgUIDef).name = new BoxedString("Tycoon Wallet");
            HeroController.instance.AddGeo(420);
        }
    }
}
