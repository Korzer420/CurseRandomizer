using System.Collections.Generic;

namespace CurseRandomizer.Curses;

internal class LoseCurse : Curse
{
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

        string rolledConsumable = viableSlots[UnityEngine.Random.Range(0, viableSlots.Count)];
        PlayerData.instance.DecrementInt(rolledConsumable);
        if (rolledConsumable == "charmSlots")
            HeroController.instance.CharmUpdate();
    }
}
