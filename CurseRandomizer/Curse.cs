using System;

namespace CurseRandomizer;

internal class Curse
{
    public CurseType Type { get; set; }

    public int Cap { get; set; }

    public object Data { get; set; }

    /// <summary>
    /// Function to evaluate if the curse can be applied. If not, the default curse will be casted instead.
    /// </summary>
    public Func<Curse,bool> CanApplyCurse { get; init; }

    /// <summary>
    /// Applies the curse.
    /// </summary>
    public Action ApplyCurse { get; set; }

    /// <summary>
    /// Checks if the curse can be executed.
    /// </summary>
    /// <returns></returns>
    internal bool CanCastCurse() => CanApplyCurse.Invoke(this);

}
