using CurseRandomizer.Enums;
using HutongGames.PlayMaker;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Drawing.Design;
using UnityEngine;

namespace CurseRandomizer;

/// <summary>
/// A debuff effect, which is applied to the player via <see cref="CurseItem"/>.
/// </summary>
public abstract class Curse
{
    public const string TextColor = "#c034eb";

    #region Members

    private CurseData _data;

    #endregion

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
    /// Gets or sets data about the curse. Includes an object to be stored in the save data.
    /// </summary>
    public CurseData Data
    {
        get => _data ??= new CurseData();
        set => _data = value;
    }

    /// <summary>
    /// Gets the cap. (This property exists to not break compability.)
    /// </summary>
    public int Cap => Data.Cap;

    /// <summary>
    /// Gets the value that indicates if the <see cref="Cap"/> should be used.
    /// </summary>
    public bool UseCap => CurseManager.UseCaps;

    /// <summary>
    /// Gets the tag of the curse. Default is permanent.
    /// </summary>
    public virtual CurseTag Tag => CurseTag.Permanent;

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
    /// Allows to load save data into this curse.
    /// </summary>
    internal void LoadData(CurseData data) 
    {
        if (data != null)
        {
            // To prevent null references, if the data is not set, we take the default.
            data.Data ??= Data.Data;
            Data = data;
        }
    }

    internal void ResetData()
    {
        Data.CastedAmount = 0;
        ResetAdditionalData();
    }

    /// <summary>
    /// Can be used to reset the data stored in <see cref="CurseData.Data"/>. The <see cref="CurseData.CastedAmount"/> will be reset automatically.
    /// <para>Called when the player starts a new game file.</para>
    /// </summary>
    public virtual void ResetAdditionalData() { }

    /// <summary>
    /// Is called, when the cap value of the curse changes in the menu. Use this to establish the boundary of your cap.
    /// </summary>
    /// <param name="value">The value the player has entered.</param>
    /// <returns>The value the cap should actually be set to.</returns>
    public abstract int SetCap(int value);

    /// <summary>
    /// Add needed hooks for your curse to work.
    /// <para>Is called, when the player entered a save file</para>
    /// </summary>
    public virtual void ApplyHooks() { }

    /// <summary>
    /// Remove the hooks set in <see cref="ApplyHooks"/>.
    /// </summary>
    public virtual void Unhook() { }
}
