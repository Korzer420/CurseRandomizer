using MenuChanger.Attributes;

namespace CurseRandomizer.Randomizer.Settings;

public class CurseControlSettings
{
    public bool PerfectMimics { get; set; }

    public bool OmenMode { get; set; }

    public RequestMethod CurseMethod { get; set; }

    public bool Bargains { get; set; }

    public bool TakeReplaceGroup { get; set; }

    public Amount CurseAmount { get; set; }

    [MenuRange(0, 200)]
    public int CurseItems { get; set; }

    // These properties will only be in the curse page, which is why we bind them manually.
    [MenuIgnore]
    public string DefaultCurse { get; set; }

    [MenuIgnore]
    public bool CustomCurses { get; set; }

    [MenuIgnore]
    public bool CapEffects { get; set; }
}
