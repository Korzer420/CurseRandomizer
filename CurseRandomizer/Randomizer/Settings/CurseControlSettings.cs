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
}
