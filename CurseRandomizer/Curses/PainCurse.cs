namespace CurseRandomizer.Curses;

internal class PainCurse : Curse
{
    public override bool CanApplyCurse()
    {
        if (CurseRandomizer.Instance.Settings.CapEffects)
            return PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > Cap;
        return true;
    }

    public override void ApplyCurse() => HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, 1, 0);
}
