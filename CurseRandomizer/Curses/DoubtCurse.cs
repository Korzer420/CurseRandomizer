using CurseRandomizer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer.Curses;

internal class DoubtCurse : Curse
{
    public override CurseTag Tag => CurseTag.Instant;

    public override void ApplyCurse()
    {
        int totalCost = 1;
        List<int> availableCharms = new();
        for (int i = 1; i < 41; i++)
        {
            if (i == 36)
                continue;
            PlayerData.instance.SetBool("equippedCharm_" + i, false);
            if (PlayerData.instance.GetBool("gotCharm_" + i))
            {
                availableCharms.Add(i);
                totalCost += PlayerData.instance.GetInt("charmCost_" + i);
            }
        }

        // Unequip all charms
        PlayerData.instance.GetVariable<List<int>>(nameof(PlayerData.instance.equippedCharms)).RemoveAll(x => x != 36);
        HeroController.instance.CharmUpdate();
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

        // Set all to zero at first
        foreach (int charmId in availableCharms)
            PlayerData.instance.SetInt("charmCost_" + charmId, 0);

        // To balance out the cost, all charms (except the ones affected by normality), start with one notch cost. (If it is possible, at least)
        NormalityCurse normalityCurse = CurseManager.GetCurseByType(CurseType.Normality) as NormalityCurse;
        if (availableCharms.Except(normalityCurse.DisabledCharmId).Count() >= totalCost)
            foreach (int charmId in availableCharms.Except(normalityCurse.DisabledCharmId))
                PlayerData.instance.SetInt("charmCost_" + charmId, 1);
        
        int run = 0;
        do
        {
            foreach (int charmId in availableCharms)
            {
                int previousCost = PlayerData.instance.GetInt("charmCost_" + charmId);
                // Charms can never cost more than 7.
                if (previousCost == 7)
                    continue;
                int maxAdditionalCost = Math.Min(totalCost, Math.Min(7 - previousCost, 4));
                int additionalCost = UnityEngine.Random.Range(1, maxAdditionalCost);

                totalCost -= additionalCost;
                PlayerData.instance.SetInt("charmCost_" + charmId, previousCost + additionalCost);

                if (totalCost == 0)
                    break;
            }

            // This is just a safety check.
            run++;
        } 
        while (totalCost > 0 && run < 39 * 7);
    }

    public override bool CanApplyCurse()
    {
        int totalCost = 1;
        int charmAmount = 0;
        for (int i = 1; i < 41; i++)
        {
            if (i == 36)
                continue;
            if (PlayerData.instance.GetBool("gotCharm_" + i))
            {
                charmAmount++;
                totalCost += PlayerData.instance.GetInt("charmCost_" + i);
            }
        }
        return totalCost < charmAmount * 7 && (!UseCap || Data.CastedAmount < Cap);
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 40));
}
