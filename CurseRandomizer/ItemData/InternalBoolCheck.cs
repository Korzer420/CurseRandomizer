using ItemChanger;

namespace CurseRandomizer.ItemData;

internal class InternalBoolCheck : IBool
{
    public int ItemNumber { get; set; }

    public bool Value
    {
        get
        {
            return ItemNumber switch
            {
                0 => CurseRandomizer.Instance.Settings.CursedWallet,
                1 => CurseRandomizer.Instance.Settings.CursedColo,
                2 => CurseRandomizer.Instance.Settings.CursedDreamNail,
                _ => false,
            };
        }
    }


    public IBool Clone() => new InternalBoolCheck() { ItemNumber = ItemNumber };
}
