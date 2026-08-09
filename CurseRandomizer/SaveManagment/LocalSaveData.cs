using System.Collections.Generic;

namespace CurseRandomizer.SaveManagment;

public class LocalSaveData
{
    public Dictionary<string, CurseData> Data { get; set; }

    public bool OmenMode { get; set; }
}
