using CurseRandomizer.Enums;

namespace CurseRandomizer.Curses;

internal class PainCurse : Curse
{
    #region Properties

    public override CurseTag Tag => CurseTag.Instant;

    #endregion

    #region Control

    public override bool CanApplyCurse() => true;

    public override void ApplyCurse() => DoDamage(1);

    internal void DoDamage(int amount)
    {
        int finalDamage = (1 + Data.DespairEnhanced) * amount;

        // Pain should not be affected by overcharming hence we remove it temporarly.
        bool overcharmed = PlayerData.instance.GetBool(nameof(PlayerData.instance.overcharmed));
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), false);
        HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, finalDamage, 0);
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), overcharmed);
    }

    #endregion
}
