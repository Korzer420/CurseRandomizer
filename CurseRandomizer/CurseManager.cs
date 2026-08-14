using CurseRandomizer.Curses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer;

public static class CurseManager
{
    #region Members

    private static List<Curse> _curses =
    [
        new AmnesiaCurse() {Name = "Amnesia", Type = CurseType.Amnesia},
        new ClumsyCurse() { Name = "Frail", Type = CurseType.Despair },
        new ConfusionCurse() { Name = "Confusion", Type = CurseType.Confusion },
        new DarknessCurse() {Name = "Darkness", Type = CurseType.Darkness },
        new DespairCurse() { Name = "Despair", Type = CurseType.Despair },
        new DiminishCurse() {Name = "Diminish", Type = CurseType.Diminish },
        new DoubtCurse() { Name = "Doubt", Type = CurseType.Doubt },
        new EmptinessCurse() { Name = "Emptiness", Type = CurseType.Emptiness},
        new GreedCurse() { Name = "Greed"},
        new LostCurse() { Name = "Lost", Type = CurseType.Lost },
        new MazeCurse() { Name = "Maze", Type = CurseType.Maze },
        new MelancholyCurse() { Name = "Melancholy", Type = CurseType.Despair },
        new NormalityCurse() { Name = "Normality", Type = CurseType.Normality },
        new OmenCurse() {Name = "Omen", Type = CurseType.Omen },
        new PainCurse() { Name = "Pain", Type = CurseType.Pain},
        new RegretCurse() { Name = "Regret", Type = CurseType.Regret },
        new SlothCurse() {Name = "Sloth", Type = CurseType.Sloth },
        new StupidityCurse() {Name = "Stupidity", Type = CurseType.Stupidity },
        new ThirstCurse() {Name = "Thirst", Type = CurseType.Thirst},
        new TraumaCurse() { Name = "Trauma", Type = CurseType.Despair },
        new UnknownCurse() {Name = "Unknown", Type = CurseType.Unknown },
        new WeaknessCurse() { Name = "Weakness", Type = CurseType.Weakness },
    ];

    #endregion

    #region Properties

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
