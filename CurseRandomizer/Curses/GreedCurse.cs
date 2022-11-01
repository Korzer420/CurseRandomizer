using UnityEngine;

namespace CurseRandomizer.Curses;

internal class GreedCurse : Curse
{
    public override bool CanApplyCurse() => PlayerData.instance.GetInt("geo") > 1;

    public override void ApplyCurse()
    {
        int geoToTake = UseCap
            ? Mathf.Min(Cap, PlayerData.instance.GetInt("geo") / 2)
            : PlayerData.instance.GetInt("geo") / 2;
        HeroController.instance.TakeGeo(geoToTake);
    }
}
