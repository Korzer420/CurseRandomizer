using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer;

internal static class CurseManager
{
    private static List<Curse> _curses;

    /// <summary>
    /// Gets or sets the curse, which should be casted if the other ones don't work.
    /// </summary>
    public static Curse DefaultCurse { get; set; }

    internal static void Initialize()
    {
        _curses = new()
        {
            new()
            {
                Type = CurseType.Pain,
                Cap = 1,
                CanApplyCurse = (curse) => 
                {
                    return CurseRandomizer.Instance.Settings.CapEffects
                    ? PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > curse.Cap
                    : PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > 0;
                }
            },
            new()
            {
                Type = CurseType.Greed,
                Cap = 500,
                CanApplyCurse = (curse) =>
                {
                    return CurseRandomizer.Instance.Settings.CapEffects
                    ? PlayerData.instance.GetInt("geo") > curse.Cap
                    : PlayerData.instance.GetInt("geo") > 1;
                }
            },
            new()
            {
                Type = CurseType.Normality,
                Cap = 500,
                Data = new List<int>(),
                CanApplyCurse = (curse) =>
                {
                    List<int> availableCharms = new();
                    for (int i = 1; i < 41; i++)
                    {
                        // Skip quest charms
                        if (i == 36 || i == 10 || i == 17 || i == 23 || i == 24 || i == 25 || i == 40)
                            continue;
                        if (PlayerData.instance.GetBool($"gotCharm_{i}"))
                            availableCharms.Add(i);
                    }
                    return availableCharms.Except((List<int>)curse.Data).Any();
                }
            },
            new()
            {
                Type = CurseType.Lose,
                Cap = 2,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 0;
                    for (int i = 1; i < 5; i++)
                        if (PlayerData.instance.GetInt($"trinket{i}") > cap)
                            return true;
                    return PlayerData.instance.GetInt("charmSlots") > cap;
                }
            },
            new()
            {
                Type = CurseType.Emptyness,
                Cap = 3,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return PlayerData.instance.GetInt("maxHealth") > cap;
                }
            },
            new()
            {
                Type = CurseType.Desorientation,
                CanApplyCurse = (curse) => true
            },
            new()
            {
                Type = CurseType.Weakness,
                Cap = 3,
                Data = 0,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return 5 + 4 * PlayerData.instance.GetInt(nameof(PlayerData.instance.nailSmithUpgrades)) - (int)curse.Data > cap;
                }
                
            },
            new()
            {
                Type = CurseType.Stupidity,
                Cap = 50,
                Data = 0,
                CanApplyCurse =(curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 99;
                    return 33 + (int)curse.Data < cap;
                }
            },
            new()
            {
                Type = CurseType.Thirst,
                Cap = 4,
                Data = 0,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return 11 - (int)curse.Data > cap;
                }
            }
        };
    }

    internal static Curse GetCurseOfType(CurseType type) => _curses.FirstOrDefault(x => x.Type == type);

    public static void AddCurse(Curse curse) => _curses.Add(curse);
}
