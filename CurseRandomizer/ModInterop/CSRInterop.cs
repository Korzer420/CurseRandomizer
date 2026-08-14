using ConnectionSettingsRando;

namespace CurseRandomizer.ModInterop;

/// <summary>
/// Interop class for Connection Settings Randomizer.
/// </summary>
internal static class CSRInterop
{
    internal static void Hook() => CSR.Register(CurseRandomizer.Instance.GetName(),
            () => CurseRandomizer.Instance.Settings,
            settings => SettingsRandomizer.CopyTo(settings, CurseRandomizer.Instance.Settings));
}
