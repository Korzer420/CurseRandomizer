using CurseRandomizer.Enums;
using System;

namespace CurseRandomizer.Curses;

internal class PainCurse : Curse
{
    #region Properties

    public override CurseTag Tag => CurseTag.Instant;

    #endregion

    #region Control

    public override bool CanApplyCurse()
    {
        if (UseCap)
            return PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > Cap;
        return true;
    }

    public override void ApplyCurse() => DoDamage(1);

    internal static void DoDamage(int amount)
    {
        int finalDamage = 0;

        for (int i = 0; i < amount; i++)
        {
            int rolled = UnityEngine.Random.Range(0, 10);
            if (rolled < 6)
                finalDamage++;
            else if (rolled < 9)
                finalDamage += 2;
            else
                finalDamage += 3;
        }

        // Pain should not be affected by overcharming hence we remove it temporarly.
        bool overcharmed = PlayerData.instance.GetBool(nameof(PlayerData.instance.overcharmed));
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), false);
        HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, finalDamage, 0);
        PlayerData.instance.SetBool(nameof(PlayerData.instance.overcharmed), overcharmed);
    }

    public override int SetCap(int value) => Math.Max(0, Math.Min(value, 8)); 

    #endregion
}
