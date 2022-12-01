using HutongGames.PlayMaker;
using System;
using System.Collections;
using UnityEngine;

namespace CurseRandomizer;

/// <summary>
/// A debuff effect, which is applied to the player via <see cref="CurseItem"/>.
/// </summary>
public abstract class Curse
{
    public const string TextColor = "#c034eb";

    #region Constructor

    /// <summary>
    /// Creates a curse and add it automatically to the list.
    /// <para>Additional curses are automatically marked as <see cref="CurseType.Custom"/>.</para>
    /// </summary>
    public Curse() => CurseManager.AddCurse(this);

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of the curse.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The type of the curse.
    /// </summary>
    public CurseType Type { get; internal set; } = CurseType.Custom;

    /// <summary>
    /// A numeric limit which should be used (if possible) to allow the player to change how hard a curse can affect them.
    /// <para/>Use this property in <see cref="CanApplyCurse"/> and/or <see cref="ApplyCurse"/>.
    /// </summary>
    public int Cap { get; set; }

    /// <summary>
    /// Gets the value that indicates if the <see cref="Cap"/> should be used.
    /// </summary>
    public bool UseCap => CurseManager.UseCaps;

    #endregion

    /// <summary>
    /// Function to evaluate if the curse can be applied. If not, the default curse will be casted instead.
    /// </summary>
    public virtual bool CanApplyCurse() => true;

    /// <summary>
    /// The logic for actually applying the curse. This will be called, once the HeroController has gained control again.
    /// </summary>
    public abstract void ApplyCurse();

    /// <summary>
    /// Parses save data into the save data. Can later be read by <see cref="LoadData(object)"/>.
    /// </summary>
    /// <returns></returns>
    public virtual object ParseData() => null;

    /// <summary>
    /// Allows to load save data into this curse.
    /// </summary>
    /// <param name="data">The data which got stored by <see cref="ParseData"/>.</param>
    public virtual void LoadData(object data) { }

    /// <summary>
    /// Resets all data for the curse.
    /// <para>Called when the player starts a new game file.</para>
    /// </summary>
    public virtual void ResetData() { }
}
