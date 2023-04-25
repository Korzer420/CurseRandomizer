using CurseRandomizer.Enums;
using CurseRandomizer.Randomizer.Settings;
using UnityEngine;

namespace CurseRandomizer.SaveManagment;

public class GlobalSaveData
{
    public CurseCounterPosition CounterPosition { get; set; }

    public RandoSettings Settings { get; set; }

    public bool EasyCurseLift { get; set; }

    public bool ColorBlindHelp { get; set; }

    public Vector3 TrackerPosition { get; set; }

    public float TrackerScaling { get; set; }
}
