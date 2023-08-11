using System.Collections.Generic;

namespace CurseRandomizer.SaveManagment;

public class LocalSaveData
{
    public Dictionary<string, CurseData> Data { get; set; }

    public bool UseCaps { get; set; }

    public string DefaultCurse { get; set; }

    public bool OmenMode { get; set; }
}
