using System.Collections.Generic;

namespace CurseRandomizer.SaveManagment;

public class LocalSaveData
{
    public Dictionary<string, CurseData> Data { get; set; }

    public int StartGeo { get; set; }

    public int Wallets { get; set; }

    public bool BronzeAccess { get; set; }

    public bool SilverAccess { get; set; }

    public bool GoldAccess { get; set; }

    public int DreamNailFragments { get; set; }

    public int SoulVessels { get; set; }

    public bool UseCaps { get; set; }

    public string DefaultCurse { get; set; }

    public bool WalletCursed { get; set; }

    public bool ColoCursed { get; set; }

    public bool DreamNailCursed { get; set; }

    public bool VesselCursed { get; set; }
}
