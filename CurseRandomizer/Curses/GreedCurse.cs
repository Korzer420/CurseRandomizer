using CurseRandomizer.Enums;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class GreedCurse : Curse
{
    public override CurseTag Tag => CurseTag.Instant;

    public override bool CanApplyCurse() => PlayerData.instance.GetInt("geo") > 1;

    public override void ApplyCurse()
    {
        int geoToTake = UseCap
            ? Mathf.Min(Cap, PlayerData.instance.GetInt("geo") / 2)
            : PlayerData.instance.GetInt("geo") / 2;
        HeroController.instance.TakeGeo(geoToTake);
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 5000));
}
