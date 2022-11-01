namespace CurseRandomizer.Curses;

internal class EmptynessCurse : Curse
{
    public override bool CanApplyCurse()
    {
        int cap = CurseRandomizer.Instance.Settings.CapEffects ? Cap : 1;
        return PlayerData.instance.GetInt("maxHealth") > cap;
    }

    public override void ApplyCurse() => HeroController.instance.AddToMaxHealth(-1);
}
