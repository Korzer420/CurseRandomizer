using System;

namespace CurseRandomizer;

public class Curse
{
    /// <summary>
    /// Gets or sets the name of the curse.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The type of the curse.
    /// </summary>
    public CurseType Type { get; set; }

    /// <summary>
    /// A numeric limit which should be used (if possible) to allow the player to change how hard a curse can affect them.
    /// <para/>Use this property in <see cref="CanApplyCurse"/> and/or <see cref="ApplyCurse"/>.
    /// </summary>
    public int Cap { get; set; }

    /// <summary>
    /// Object in which the curse can use to store data (e.g containing a number, which shows how often the curse has been applied)
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Function to evaluate if the curse can be applied. If not, the default curse will be casted instead.
    /// <para>Parses itself to allow properties being used in initialization.</para>
    /// </summary>
    public Func<Curse,bool> CanApplyCurse { get; init; }

    /// <summary>
    /// Action to apply the curse.
    /// <para>This action will be delayed, until the HeroController accepts inputs again.</para>
    /// <para>Parses itself to allow properties being used in initialization.</para>
    /// </summary>
    public Action<Curse> ApplyCurse { get; init; }

    /// <summary>
    /// Checks if the curse can be executed.
    /// </summary>
    internal bool CanCastCurse() => CanApplyCurse.Invoke(this);

    /// <summary>
    /// Apply the curse.
    /// </summary>
    internal void CastCurse() => CurseManager.StartRoutine(ApplyCurse, this);
}
