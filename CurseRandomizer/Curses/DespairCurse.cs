using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using Modding;
using System;
using System.Reflection;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class DespairCurse : TemporaryCurse
{
    #region Members

    private MethodInfo _canTakeDamage = ReflectionHelper.GetMethodInfo(typeof(HeroController), "CanTakeDamage");

    private Coroutine _soulDrain;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the flag that indicates if despair is active. Temporary curses should not be allowed to progress while this is the case.
    /// </summary>
    public static bool DespairActive => CurseManager.GetCurse<DespairCurse>().IsActive();

    public override int CurrentAmount
    {
        get => Counter.GeoDesperation + Counter.CurseDesperation + Counter.SpellDesperation + Counter.KillDesperation + Counter.DeathDesperation + Counter.RoomDesperation;
        set { }
    }

    public override int NeededAmount => Math.Min(Data.CastedAmount, UseCap ? Cap : 10) * 7;

    public DespairTracker Counter
    {
        get
        {
            if (Data.AdditionalData is null)
                Data.AdditionalData = new DespairTracker();
            return Data.AdditionalData as DespairTracker;
        }
    }

    #endregion

    #region Event handler

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        base.ApplyHooks();
        if (IsActive())
            Counter.StartListening();
    }

    public override void Unhook()
    {
        base.Unhook();
        Counter.StopListening();
    }

    public override void ApplyCurse()
    {
        base.ApplyCurse();
        if (Counter.Active)
        { 
            Counter.Reset();
            Counter.Active = true;
        }
        else
            Counter.StartListening();
    }

    internal override void UpdateProgression()
    {
        base.UpdateProgression();
        if (PlayerData.instance.GetInt(nameof(PlayerData.instance.permadeathMode)) != 0 && CurrentAmount >= NeededAmount)
        { 
            LiftCurse();
            Counter.Reset(true);
            Counter.StopListening();
        }
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(20, value));

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(0f, 0f),
            CurseCounterPosition.VerticalBlock => new(0, -3f),
            CurseCounterPosition.Column => new(0f, 0f),
            _ => new(0f, 0f),
        };
    }

    internal void RemoveCurse() => LiftCurse();

    public override void ResetAdditionalData() => Data.AdditionalData = new DespairTracker();

    protected override bool IsActive() => Counter.Active;

    #endregion
}
