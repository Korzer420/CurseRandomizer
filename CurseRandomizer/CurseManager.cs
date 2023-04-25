using CurseRandomizer.Curses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer;

public static class CurseManager
{
    #region Members

    private static List<Curse> _curses = new()
    {
        new PainCurse() { Name = "Pain", Type = CurseType.Pain, Data = new CurseData() { Cap = 1 }},
        new GreedCurse() { Name = "Greed", Type = CurseType.Greed, Data = new CurseData() { Cap = 5000 } },
        new EmptinessCurse() { Name = "Emptiness", Type = CurseType.Emptiness, Data = new CurseData() { Cap = 3 } },
        new ThirstCurse() { Name = "Thirst", Type = CurseType.Thirst, Data = new CurseData() { Cap = 5 } },
        new WeaknessCurse() { Name = "Weakness", Type = CurseType.Weakness, Data = new CurseData() { Cap = 3 } },
        new DisorientationCurse() { Name = "Disorientation", Type = CurseType.Disorientation },
        new LostCurse() { Name = "Lost", Type = CurseType.Lost, Data = new CurseData() { Cap = 2 } },
        new NormalityCurse() { Name = "Normality", Type = CurseType.Normality, Data = new CurseData() { Cap = 5 } },
        new StupidityCurse() { Name = "Stupidity", Type = CurseType.Stupidity, Data = new CurseData() { Cap = 50 } },
        new AmnesiaCurse() {Name = "Amnesia", Type = CurseType.Amnesia, Data = new() { Cap = 5 } },
        new DarknessCurse() {Name = "Darkness", Type = CurseType.Darkness, Data = new() { Cap = 3 } },
        new DiminishCurse() {Name = "Diminish", Type = CurseType.Diminish, Data = new() { Cap = 3 } },
        new SlothCurse() {Name = "Sloth", Type = CurseType.Sloth, Data = new() { Cap = 5 } },
        new UnknownCurse() {Name = "Unknown", Type = CurseType.Unknown, Data = new() { Cap = 3 } },
        new OmenCurse() {Name = "Omen", Type = CurseType.Omen, Data = new() { Cap = 5 } },
        new DoubtCurse() { Name = "Doubt", Type = CurseType.Doubt, Data = new() {Cap = 5} },
        new ConfusionCurse() { Name = "Confusion", Type = CurseType.Confusion, Data = new() {Cap = 15} },
        new RegretCurse() { Name = "Regret", Type = CurseType.Regret, Data = new() {Cap = 5} },
        new MazeCurse() { Name = "Maze", Type = CurseType.Maze, Data = new() {Cap = 2} },
        new DespairCurse() { Name = "Despair", Type = CurseType.Despair, Data = new() {Cap = 4} },
        new MidasCurse() { Name = "Midas", Type = CurseType.Midas, Data = new() { Cap = 10 } }
    };

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the curse, which should be casted if the other ones don't work.
    /// <para>If this default curse cannot be applied as well, the desorientation curse will be casted instead.</para>
    /// </summary>
    public static Curse DefaultCurse { get; set; }

    /// <summary>
    /// Gets or sets if curses should use their caps.
    /// </summary>
    public static bool UseCaps { get; set; }

    internal static CurseHandler Handler { get; set; }

    #endregion

    internal static void Initialize()
    {
        GameObject coroutineHolder = new("Curse Randomizer Handler");
        GameObject.DontDestroyOnLoad(coroutineHolder);
        Handler = coroutineHolder.AddComponent<CurseHandler>();
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
    internal static T GetCurse<T>() where T : Curse => _curses.FirstOrDefault(x => x is T) as T;

    internal static List<Curse> GetCurses() => _curses;

    /// <summary>
    /// Adds a curse to the list of the available curses.
    /// <para/>Adding a curse this way will only be used, if the player activates "custom curses".
    /// </summary>
    /// <param name="curse"></param>
    internal static void AddCurse(Curse curse)
    {
        if (_curses != null && !_curses.Contains(curse))
            _curses.Add(curse);
    }

    internal static void ParseSaveData(Dictionary<string, CurseData> curseData)
    {
        if (curseData == null)
            return;
        CurseRandomizer.Instance.LogDebug("Load data for curses.");
        foreach (string curseName in curseData.Keys)
            try
            {
                if (GetCurseByName(curseName) is Curse curse)
                    curse.LoadData(curseData[curseName]);
            }
            catch (System.Exception exception)
            {
                CurseRandomizer.Instance.LogError(exception.Message + " StackTrace: " + exception.StackTrace);
            }
    }
}
