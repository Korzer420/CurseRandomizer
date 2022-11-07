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
        new PainCurse() { Name = "Pain", Type = CurseType.Pain, Cap = 1 },
        new GreedCurse() { Name = "Greed", Type = CurseType.Greed, Cap = 1000 },
        new EmptinessCurse() { Name = "Emptyness", Type = CurseType.Emptiness, Cap = 3 },
        new ThirstCurse() { Name = "Thirst", Type = CurseType.Thirst, Cap = 5 },
        new WeaknessCurse() { Name = "Weakness", Type = CurseType.Weakness, Cap = 3 },
        new DisorientationCurse() { Name = "Desorientation", Type = CurseType.Disorientation },
        new LoseCurse() { Name = "Lose", Type = CurseType.Lose, Cap = 2 },
        new NormalityCurse() { Name = "Normality", Type = CurseType.Normality, Cap = 5 },
        new StupidityCurse() { Name = "Stupidity", Type = CurseType.Stupidity, Cap = 50 }
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
    internal static Curse GetCurseByType(CurseType type) => _curses.FirstOrDefault(x => x.Type == type);

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

    internal static void ParseSaveData(Dictionary<string, object> curseData)
    {
        if (curseData == null)
            return;
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
