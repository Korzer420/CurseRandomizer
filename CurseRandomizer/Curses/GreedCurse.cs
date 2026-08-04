using CurseRandomizer.Enums;
using KorzUtils.Helper;
using System;

namespace CurseRandomizer.Curses;

internal class GreedCurse : Curse
{
    public override CurseTag Tag => CurseTag.Instant;

    public override bool CanApplyCurse() => PDHelper.Geo > 1;

    public override void ApplyCurse()
    {
        int geoToTake = (int)Math.Round(PDHelper.Geo * (0.3f + (Math.Min(0.7f, DespairCurse.CastedDespair * 0.1f))));
        HeroController.instance.TakeGeo(geoToTake);
    }
}
