using CurseRandomizer.Enums;
using CurseRandomizer.Randomizer.Settings;
using System.Collections.Generic;

namespace CurseRandomizer.SaveManagment;

public class GlobalSaveData
{
    public CurseCounterPosition CounterPosition { get; set; }

    public RandoSettings Settings { get; set; }

    public Dictionary<string, bool> IgnoredCurses { get; set; }
}
