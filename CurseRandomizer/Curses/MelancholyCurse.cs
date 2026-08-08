using ItemChanger;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer.Curses;

public class MelancholyCurse : Curse
{
    public override void ApplyCurse()
    {
        foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.GetPlacements())
        {
            IEnumerable<AbstractItem> curseItems = placement.Items.Where(x => x is CurseItem && x.IsObtained());
            if (curseItems.Count() == 0)
                continue;
            // Reset UI and obtained flag (and the cost in shop/dialog)
            foreach (AbstractItem item in curseItems)
            { 
                item.UIDef = (item as CurseItem).FakeUIDef;
                item.RefreshObtained();
                if (item.GetTag<CostTag>() is CostTag tag)
                    tag.Cost.Paid = false;
            }
        }
    }
}
