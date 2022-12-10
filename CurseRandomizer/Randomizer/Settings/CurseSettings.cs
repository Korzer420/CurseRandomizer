using MenuChanger.Attributes;

namespace CurseRandomizer.Randomizer.Settings;

/// <summary>
/// Container which is used in the menu and will be pasted onto the real curses upon starting the rando.
/// </summary>
public class CurseSettings
{
    /// <summary>
    /// Gets or sets the name of the curse.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets or sets if the curse should be used.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the cap of the curse.
    /// </summary>
    public int Cap { get; set; }

    /// <summary>
    /// Gets if the curse settings are related to the curse.
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="curse"></param>
    /// <returns></returns>
    public static bool operator == (CurseSettings settings, Curse curse) => settings?.Name == curse?.Name;

    /// <summary>
    /// Gets if the curse settings are not related to the curse.
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="curse"></param>
    /// <returns></returns>
    public static bool operator != (CurseSettings settings, Curse curse) => settings?.Name != curse?.Name;
}
