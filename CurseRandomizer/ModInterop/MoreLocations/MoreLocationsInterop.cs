using MoreLocations.Rando;

namespace CurseRandomizer.ModInterop.MoreLocations;

internal static class MoreLocationsInterop
{
	#region Methods

	internal static void Hook() 
		=> ConnectionInterop.AddRandoCostProviderToJunkShop(BargainsEnabled, CreateProvider);

	private static bool BargainsEnabled() => CurseRandomizer.Instance.Settings.GeneralSettings.Enabled
		&& CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses && CurseRandomizer.Instance.Settings.CurseControlSettings.Bargains;

	private static CurseCostProvider CreateProvider() => new();

    #endregion
}
