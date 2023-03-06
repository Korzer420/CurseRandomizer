using System;

namespace CurseRandomizer;

/// <summary>
/// Contains data which curses can use ingame and in the menu.
/// </summary>
[Serializable]
public class CurseData
{
    /// <summary>
    /// Gets or sets if this curse is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the amount this curse was casted.
    /// </summary>
    public int CastedAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the cap of the curse, to determine if it can be casted.
    /// </summary>
    public int Cap { get; set; }

    /// <summary>
    /// Gets or sets an object, in which the curse can store additional data.
    /// </summary>
    public object AdditionalData { get; set; }

    public bool Ignored { get; set; }
}
