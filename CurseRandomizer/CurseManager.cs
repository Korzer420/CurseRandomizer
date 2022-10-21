using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer;

public static class CurseManager
{
    #region Members

    private static List<Curse> _curses;

    private static CurseHandler _handler;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the curse, which should be casted if the other ones don't work.
    /// <para>If this default curse cannot be applied as well, the desorientation curse will be casted instead.</para>
    /// </summary>
    public static Curse DefaultCurse { get; set; } 

    #endregion

    internal static void Initialize()
    {
        _curses = new()
        {
            new()
            {
                Name = "Pain",
                Type = CurseType.Pain,
                Cap = 1,
                CanApplyCurse = (curse) =>
                {
                    return CurseRandomizer.Instance.Settings.CapEffects
                    ? PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > curse.Cap
                    : PlayerData.instance.GetInt(nameof(PlayerData.instance.health)) > 0;
                },
                ApplyCurse = (curse) => HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, 1, 0)
            },
            new()
            {
                Name = "Greed",
                Type = CurseType.Greed,
                Cap = 500,
                CanApplyCurse = (curse) => PlayerData.instance.GetInt("geo") > 1,
                ApplyCurse = (curse) =>
                {
                    int geoToTake = CurseRandomizer.Instance.Settings.CapEffects
                    ? Mathf.Min(PlayerData.instance.GetInt("geo") / 2, curse.Cap)
                    : PlayerData.instance.GetInt("geo") / 2;
                    HeroController.instance.TakeGeo(geoToTake);
                }
            },
            new()
            {
                Name = "Normality",
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
                },
                ApplyCurse = (curse) =>
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
                    availableCharms = availableCharms.Except((List<int>)curse.Data).ToList();

                    int rolledCharm = availableCharms[UnityEngine.Random.Range(0, availableCharms.Count())];
                    PlayerData.instance.SetBool("equipped_Charm"+rolledCharm, false);
                    (curse.Data as List<int>).Add(rolledCharm);
                    PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
                }
            },
            new()
            {
                Name = "Lose",
                Type = CurseType.Lose,
                Cap = 2,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 0;
                    for (int i = 1; i < 5; i++)
                        if (PlayerData.instance.GetInt($"trinket{i}") > cap)
                            return true;
                    return PlayerData.instance.GetInt("charmSlots") > cap;
                },
                ApplyCurse = (curse) =>
                {
                    List<string> viableSlots = new();
                    int cap = CurseRandomizer.Instance.Settings.CapEffects
                    ? curse.Cap
                    : 0;

                    for (int i = 1; i < 5; i++)
                        if (PlayerData.instance.GetInt($"trinket{i}") > cap)
                            viableSlots.Add($"trinket{i}");
                    if (PlayerData.instance.GetInt("charmSlots") > cap)
                        viableSlots.Add("charmSlots");

                    string rolledConsumable = viableSlots[UnityEngine.Random.Range(0, viableSlots.Count)];
                    PlayerData.instance.DecrementInt(rolledConsumable);
                    if (rolledConsumable == "charmSlots")
                        HeroController.instance.CharmUpdate();
                }
            },
            new()
            {
                Name = "Emptyness",
                Type = CurseType.Emptyness,
                Cap = 3,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return PlayerData.instance.GetInt("maxHealth") > cap;
                },
                ApplyCurse = (curse) => HeroController.instance.AddToMaxHealth(-1)
            },
            new()
            {
                Name = "Desorientation",
                Type = CurseType.Desorientation,
                CanApplyCurse = (curse) => true,
                ApplyCurse = (curse) => GameManager.instance.StartCoroutine(HeroController.instance.Respawn())
            },
            new()
            {
                Name = "Weakness",
                Type = CurseType.Weakness,
                Cap = 3,
                Data = 0,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return 5 + 4 * PlayerData.instance.GetInt(nameof(PlayerData.instance.nailSmithUpgrades)) - (int)curse.Data > cap;
                },
                ApplyCurse = (curse) =>
                {
                    curse.Data = Convert.ToInt32(curse.Data) - 1;
                    PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
                }
            },
            new()
            {
                Name = "Stupidity",
                Type = CurseType.Stupidity,
                Cap = 50,
                Data = 0,
                CanApplyCurse =(curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 99;
                    return 33 + (int)curse.Data < cap;
                },
                ApplyCurse = (curse) => curse.Data = (int)curse.Data + 1
            },
            new()
            {
                Name = "Thirst",
                Type = CurseType.Thirst,
                Cap = 4,
                Data = 0,
                CanApplyCurse = (curse) =>
                {
                    int cap = CurseRandomizer.Instance.Settings.CapEffects ? curse.Cap : 1;
                    return 11 - (int)curse.Data > cap;
                },
                ApplyCurse = (curse) => curse.Data = (int)curse.Data + 1
            }
        };
        GameObject coroutineHolder = new("Curse Randomizer Handler");
        GameObject.DontDestroyOnLoad(coroutineHolder);
        _handler = coroutineHolder.AddComponent<CurseHandler>();
    }

    /// <summary>
    /// Gets the curse by name.
    /// </summary>
    public static Curse GetCurseByName(string name) => _curses.FirstOrDefault(x => x.Name == name);

    /// <summary>
    /// Get one of the core curses, should not be used with custom curses.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static Curse GetCurseByType(CurseType type) => _curses.FirstOrDefault(x => x.Type == type);

    /// <summary>
    /// Adds a curse to the list of the available curses.
    /// <para/>Adding a curse this way will only be used, if the player activates "custom curses".
    /// </summary>
    /// <param name="curse"></param>
    public static void AddCurse(Curse curse) => _curses.Add(curse);

    /// <summary>
    /// Start a coroutine, can be used to delay curse activation.
    /// </summary>
    internal static Coroutine StartRoutine(Action<Curse> action, Curse curse) => _handler.StartCoroutine(DelayCurse(action, curse));

    /// <summary>
    /// Wait for the HeroController to accept inputs, and then activate the curse.
    /// </summary>
    private static IEnumerator DelayCurse(Action<Curse> action, Curse curse)
    {
        yield return new WaitUntil(() => HeroController.instance.acceptingInput);
        action.Invoke(curse);
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "Curse_Randomizer_Fool";
        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
    }
}
