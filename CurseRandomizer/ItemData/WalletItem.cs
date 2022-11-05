using CurseRandomizer.Manager;
using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer.ItemData;

internal class WalletItem : AbstractItem
{
    private static int[] _walletSizes = new int[] { 500, 1000, 5000 };
    
    public override void GiveImmediate(GiveInfo info)
    {
        ModManager.WalletAmount++;
        if (ModManager.WalletAmount < 4)
            (UIDef as MsgUIDef).name = new BoxedString("Wallet (" + _walletSizes[ModManager.WalletAmount - 1] + ")");
        else
            (UIDef as MsgUIDef).name = new BoxedString("Tycoon Wallet");
        // If the player has enabled starting geo, 50% of the geo is granted (multiplied by the wallet amounts)
        if (ModManager.StartGeo > 0)
            HeroController.instance.AddGeo(System.Convert.ToInt32(ModManager.StartGeo * (0.5f * ModManager.WalletAmount)));
    }
}
