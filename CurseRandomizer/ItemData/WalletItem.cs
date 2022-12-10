using CurseRandomizer.Manager;
using ItemChanger;
using ItemChanger.UIDefs;

namespace CurseRandomizer.ItemData;

internal class WalletItem : AbstractItem
{
    #region Constants

    public const string Sly_Cheap = "Sly_Cheap";

    public const string Sly_Medium = "Sly_Medium";

    public const string Sly_Expensive = "Sly_Expensive";

    public const string Sly_Extreme_Valuable = "Sly_Extreme_Valuable";

    public const string Sly_Key_Cheap = "Sly_(Key)_Cheap";

    public const string Sly_Key_Medium = "Sly_(Key)_Medium";

    public const string Sly_Key_Expensive = "Sly_(Key)_Expensive";

    public const string Sly_Key_Extreme_Valuable = "Sly_(Key)_Extreme_Valuable";

    public const string Salubra_Cheap = "Salubra_Cheap";

    public const string Salubra_Medium = "Salubra_Medium";

    public const string Salubra_Expensive = "Salubra_Expensive";

    public const string Salubra_Extreme_Valuable = "Salubra_Extreme_Valuable";

    public const string Salubra_Charms_Cheap = "Salubra_(Requires_Charms)_Cheap";

    public const string Salubra_Charms_Medium = "Salubra_(Requires_Charms)_Medium";

    public const string Salubra_Charms_Expensive = "Salubra_(Requires_Charms)_Expensive";

    public const string Salubra_Charms_Extreme_Valuable = "Salubra_(Requires_Charms)_Extreme_Valuable";

    public const string Iselda_Cheap = "Iselda_Cheap";

    public const string Iselda_Medium = "Iselda_Medium";

    public const string Iselda_Expensive = "Iselda_Expensive";

    public const string Iselda_Extreme_Valuable = "Iselda_Extreme_Valuable";

    public const string Leg_Eater_Cheap = "Leg_Eater_Cheap";

    public const string Leg_Eater_Medium = "Leg_Eater_Medium";

    public const string Leg_Eater_Expensive = "Leg_Eater_Expensive";

    public const string Leg_Eater_Extreme_Valuable = "Leg_Eater_Extreme_Valuable";

    #endregion

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
