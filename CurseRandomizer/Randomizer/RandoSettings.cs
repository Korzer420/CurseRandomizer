using MenuChanger.Attributes;

namespace CurseRandomizer;

public class RandoSettings
{
    #region General Settings

    /// <summary>
    /// Gets or sets the flag, that indicates if this connection is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    public bool CursedWallet { get; set; }

    public bool CursedDreamNail { get; set; }

    public bool UseCurses { get; set; }

    public bool CursedVessel { get; set; }

    public bool CursedColo { get; set; }

    #endregion

    #region Curse Settings

    public bool PerfectMimics { get; set; }

    public bool CapEffects { get; set; }

    public CurseType DefaultCurse { get; set; }

    public RequestMethod CurseMethod { get; set; }

    public Amount CurseAmount { get; set; }

    [MenuRange(0, 100)]
    public int CurseItems { get; set; }

    #region Curses

    public bool PainCurse { get; set; }

    public bool GreedCurse { get; set; }

    public bool NormalityCurse { get; set; }

    public bool LoseCurse { get; set; }

    public bool EmptinessCurse { get; set; }

    public bool DisorientationCurse { get; set; }

    public bool WeaknessCurse { get; set; }

    public bool StupidityCurse { get; set; }

    public bool ThirstCurse { get; set; }

    public bool CustomCurses { get; set; }

    #endregion

    #region AvailablePools

    public bool MaskShards { get; set; } = true;

    public bool VesselFragments { get; set; } = true;

    public bool PaleOre { get; set; } = true;

    public bool Notches { get; set; } = true;

    public bool Geo { get; set; } = true;

    public bool Relics { get; set; } = true;

    public bool Totems { get; set; } = true;

    public bool Rocks { get; set; } = true;

    public bool Custom { get; set; } = true;

    #endregion

    #region Caps

    [MenuRange(0,8)]
    public int PainCap { get; set; }

    [MenuRange(0, 5000)]
    public int GreedCap { get; set; }

    [MenuRange(0, 30)]
    public int NormalityCap { get; set; }

    [MenuRange(0, 11)]
    public int LoseCap { get; set; }

    [MenuRange(1, 8)]
    public int EmptynessCap { get; set; } = 1;

    [MenuRange(1, 20)]
    public int WeaknessCap { get; set; } = 1;

    [MenuRange(33, 99)]
    public int StupidityCap { get; set; } = 33;

    [MenuRange(1, 11)]
    public int ThirstCap { get; set; } = 11;

    #endregion

    #endregion
}
