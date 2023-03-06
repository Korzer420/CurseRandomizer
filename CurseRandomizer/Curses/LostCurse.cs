using CurseRandomizer.Enums;
using HutongGames.PlayMaker;
using KorzUtils.Helper;
using System;
using System.Collections.Generic;

namespace CurseRandomizer.Curses;

internal class LostCurse : Curse
{
    public override CurseTag Tag => CurseTag.Instant;

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 0;
        for (int i = 1; i < 5; i++)
            if (PlayerData.instance.GetInt($"trinket{i}") > cap)
                return true;
        return cap == 0 ? PlayerData.instance.GetInt("charmSlots") > 1 : PlayerData.instance.GetInt("charmSlots") > cap;
    }

    public override void ApplyCurse()
    {
        List<string> viableSlots = new();
        int cap = UseCap
        ? Cap
        : 0;

        for (int i = 1; i < 5; i++)
            if (PlayerData.instance.GetInt($"trinket{i}") > cap)
                viableSlots.Add($"trinket{i}");
        if (cap == 0 ? PlayerData.instance.GetInt("charmSlots") > 1 : PlayerData.instance.GetInt("charmSlots") > cap)
            viableSlots.Add("charmSlots");

        string rolledConsumable = null;
        // Charm slots have a base 20% if any relic is present since the punishment is way harder.
        if (viableSlots.Contains("charmSlots") && viableSlots.Count > 1)
            if (UnityEngine.Random.Range(0, 100) < 20)
                rolledConsumable = "charmSlots";
            else
                viableSlots.Remove("charmSlots");

        if (rolledConsumable == null)
            rolledConsumable = viableSlots[UnityEngine.Random.Range(0, viableSlots.Count)];
        
        PlayerData.instance.DecrementInt(rolledConsumable);
        if (rolledConsumable == "charmSlots")
        {
            HeroController.instance.CharmUpdate();
            GameHelper.DisplayMessage("FOOL (You lost a charm notch)");
        }
        else
            GameHelper.DisplayMessage("FOOL (You lost a relic)");
    }

    public override int SetCap(int value) => Math.Max(0, Math.Min(value, 11));
}
