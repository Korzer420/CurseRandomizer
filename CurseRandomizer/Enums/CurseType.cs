namespace CurseRandomizer;

public enum CurseType
{
    /// <summary>
    /// Let the player take 1 damage.
    /// </summary>
    Pain,

    /// <summary>
    /// Takes 50% of the players geo.
    /// </summary>
    Greed,

    /// <summary>
    /// Makes a charm useless.
    /// </summary>
    Normality,

    /// <summary>
    /// Remove a relic or charm notch.
    /// </summary>
    Lost,

    /// <summary>
    /// Removes a mask from the player.
    /// </summary>
    Emptiness,

    /// <summary>
    /// Teleports the player back to their bench. The other curses default to this if they cannot be executed.
    /// </summary>
    Disorientation,

    /// <summary>
    /// Reduces the nail damage by 1.
    /// </summary>
    Weakness,

    /// <summary>
    /// Spells costs 1 more soul.
    /// </summary>
    Stupidity,

    /// <summary>
    /// Hits grant 1 less soul.
    /// </summary>
    Thirst,

    /// <summary>
    /// Weakens spell damage by 10% or removes spell upgrade.
    /// </summary>
    Amnesia,

    /// <summary>
    /// Makes the game temporarly darker. Will be stronger after multiple casts.
    /// </summary>
    Darkness,

    /// <summary>
    /// Shortens nail range.
    /// </summary>
    Diminish,

    /// <summary>
    /// Hides health, soul, geo, essence or the map.
    /// </summary>
    Unknown,

    /// <summary>
    /// Increases attack or dash cooldown.
    /// </summary>
    Sloth,

    /// <summary>
    /// Casts another permanent curse upon being hit. Vanishes after killing enough different enemies.
    /// </summary>
    Omen,

    /// <summary>
    /// Killing an enemy has a chance to apply an instant curse.
    /// </summary>
    Regret,

    /// <summary>
    /// Randomizes the key bindings until vanished.
    /// </summary>
    Confusion,

    /// <summary>
    /// Entering a room has a chance to teleport you to a known room.
    /// </summary>
    Maze,

    /// <summary>
    /// Removes all equipped charms and redistributes their cost (with 1 additional cost).
    /// </summary>
    Doubt,

    /// <summary>
    /// Prevent temporarly curses from further progression. Slowly drains soul over time and healing does nothing. Kills you on vanish.
    /// </summary>
    Despair,

    /// <summary>
    /// A custom curse implemented by another mod.
    /// </summary>
    Custom
}
