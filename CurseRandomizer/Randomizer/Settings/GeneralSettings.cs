using MenuChanger.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Randomizer.Settings;

public class GeneralSettings
{
    /// <summary>
    /// Gets or sets the flag, that indicates if this connection is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    public bool CursedWallet { get; set; }

    public bool CursedDreamNail { get; set; }

    public bool UseCurses { get; set; }

    [MenuRange(0, 2)]
    public int CursedVessel { get; set; } = 0;

    public bool CursedColo { get; set; }
}
