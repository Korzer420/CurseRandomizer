using ItemChanger;

namespace CurseRandomizer.ItemData;

internal class InternalBoolCheck : IBool
{
    public int ItemNumber { get; set; }

    public bool Value
    {
        get
        {
            switch (ItemNumber)
            {
                case 0:
                    return CurseRandomizer.Instance.Settings.CursedWallet;
                case 1:
                    return CurseRandomizer.Instance.Settings.CursedColo;
                case 2:
                    return CurseRandomizer.Instance.Settings.CursedDreamNail;
                default:
                    return false;
            }
        }
    }


    public IBool Clone() => new InternalBoolCheck() { ItemNumber = ItemNumber };
}
