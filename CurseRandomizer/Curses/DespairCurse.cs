namespace CurseRandomizer.Curses;

/// <summary>
/// A curse that makes other curses worse.
/// <para>Each curse should, if possible try to implement a despair effect. Use <see cref="CurseData.DespairEnhanced"/> to count the times despair has been casted on a specific curse.</para>
/// </summary>
internal class DespairCurse : Curse
{
    #region Control

    public override bool CanApplyCurse() => true;

    #endregion
}
