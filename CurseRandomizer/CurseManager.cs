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
        new PainCurse() { Name = "Pain", Type = CurseType.Pain},
        new GreedCurse() { Name = "Greed"},
        new EmptinessCurse() { Name = "Emptiness", Type = CurseType.Emptiness},
        new ThirstCurse() { Name = "Thirst", Type = CurseType.Thirst},
        new WeaknessCurse() { Name = "Weakness", Type = CurseType.Weakness },
        new DisorientationCurse() { Name = "Disorientation", Type = CurseType.Disorientation },
        new LostCurse() { Name = "Lost", Type = CurseType.Lost },
        new NormalityCurse() { Name = "Normality", Type = CurseType.Normality },
        new StupidityCurse() { Name = "Stupidity", Type = CurseType.Stupidity },
        new AmnesiaCurse() {Name = "Amnesia", Type = CurseType.Amnesia},
        new DarknessCurse() {Name = "Darkness", Type = CurseType.Darkness },
        new DiminishCurse() {Name = "Diminish", Type = CurseType.Diminish },
        new SlothCurse() {Name = "Sloth", Type = CurseType.Sloth },
        new UnknownCurse() {Name = "Unknown", Type = CurseType.Unknown },
        new OmenCurse() {Name = "Omen", Type = CurseType.Omen },
        new DoubtCurse() { Name = "Doubt", Type = CurseType.Doubt },
        new ConfusionCurse() { Name = "Confusion", Type = CurseType.Confusion },
        new RegretCurse() { Name = "Regret", Type = CurseType.Regret },
        new MazeCurse() { Name = "Maze", Type = CurseType.Maze },
        new DespairCurse() { Name = "Despair", Type = CurseType.Despair }
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
