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

    public bool CursedDreamnail { get; set; }

    public bool UseCurses { get; set; }

    public bool CursedVessel { get; set; }

    public bool CursedColo { get; set; }

    #endregion

    #region Curse Settings

    public bool CapEffects { get; set; }

    public RequestMethod CurseMethod { get; set; }

    public Amount CurseAmount { get; set; }

    [MenuRange(0, 50)]
    public int CurseItems { get; set; }

    #region Curses

    public bool PainCurse { get; set; }

    public bool GreedCurse { get; set; }

    public bool NormalityCurse { get; set; }

    public bool LoseCurse { get; set; }

    public bool EmptynessCurse { get; set; }

    public bool DesorientationCurse { get; set; }

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

    public bool CustomPools { get; set; } = true;

    #endregion 

    #endregion
}
