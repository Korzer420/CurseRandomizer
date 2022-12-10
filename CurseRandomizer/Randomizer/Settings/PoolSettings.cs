namespace CurseRandomizer.Randomizer.Settings;

/// <summary>
/// Contains which items from which pool can be replaced by curses.
/// </summary>
public class PoolSettings 
{
    public bool MaskShards { get; set; } = true;

    public bool VesselFragments { get; set; } = true;

    public bool PaleOre { get; set; } = true;

    public bool Notches { get; set; } = true;

    public bool Geo { get; set; } = true;

    public bool Relics { get; set; } = true;

    public bool Totems { get; set; } = true;

    public bool Rocks { get; set; } = true;

    public bool Custom { get; set; } = true;
}
