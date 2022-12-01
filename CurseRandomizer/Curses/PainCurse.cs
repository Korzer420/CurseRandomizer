namespace CurseRandomizer.Curses;

internal class PainCurse : Curse
{
    public override bool CanApplyCurse()
    {
        if (UseCap)
            return PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > Cap;
        return true;
    }

    public override void ApplyCurse() => DoDamage(1);

    internal static void DoDamage(int amount) 
    {
        // Pain should not be affected by overcharming hence we remove it temporarly.
        bool overcharmed = PlayerData.instance.GetBool(nameof(PlayerData.instance.overcharmed));
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), false);
        HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, amount, 0);
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), overcharmed);
    }
}
