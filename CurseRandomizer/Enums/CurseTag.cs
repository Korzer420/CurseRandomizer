namespace CurseRandomizer.Enums;

/// <summary>
/// Contains information, how a curse affects the player. In the base mod, this is only used for the Omen curse.
/// </summary>
public enum CurseTag
{
    /// <summary>
    /// On pickup, the curse does something but it doesn't affect anything beyond that.
    /// </summary>
    Instant,

    /// <summary>
    /// The curse applies an effect which vanishes after certain goals are reached.
    /// </summary>
    Temporarly,

    /// <summary>
    /// The curse applies an effect which holds till the end of the game.
    /// </summary>
    Permanent,
}
