
using MenuChanger.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Randomizer.Settings;

public class CurseControlSettings
{
    public bool PerfectMimics { get; set; }

    public RequestMethod CurseMethod { get; set; }

    public Amount CurseAmount { get; set; }

    [MenuRange(0, 200)]
    public int CurseItems { get; set; }

    // These properties will be only the curse page, which is why we bind them manually.
    [MenuIgnore]
    public string DefaultCurse { get; set; }

    [MenuIgnore]
    public bool CustomCurses { get; set; }

    [MenuIgnore]
    public bool CapEffects { get; set; }
}
