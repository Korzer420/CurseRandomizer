using System.Collections.Generic;

namespace CurseRandomizer.Randomizer.Settings;

public class RandoSettings
{
    public GeneralSettings GeneralSettings { get; set; } = new();

    public CurseControlSettings CurseControlSettings { get; set; } = new();

    public CurseSettings Curses { get; set; } = new();

    public PoolSettings Pools { get; set; } = new();
}
