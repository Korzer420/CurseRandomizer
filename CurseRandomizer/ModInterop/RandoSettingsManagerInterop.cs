using CurseRandomizer.Randomizer;
using CurseRandomizer.Randomizer.Settings;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;

namespace CurseRandomizer.ModInterop;

internal static class RandoSettingsManagerInterop
{
	#region Methods

	internal static void Hook()
	{
        RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<RandoSettings>(CurseRandomizer.Instance,
        RandomizerMenu.Instance.UpdateMenuSettings,
        () => CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? CurseRandomizer.Instance.Settings : null));
    }

	#endregion
}
