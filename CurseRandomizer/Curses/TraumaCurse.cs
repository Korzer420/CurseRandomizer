using CurseRandomizer.Components;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using ItemChanger.Extensions;
using KorzUtils.Helper;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class TraumaCurse : TemporaryCurse
{
    public override int CurrentAmount
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = -1;
            return Convert.ToInt16(Data.AdditionalData);
        }
        set => Data.AdditionalData = value;
    }

    public override int NeededAmount => 300;

    public override void ApplyCurse()
    {
        base.ApplyCurse();
        CurrentAmount = 0;
        HeroController.instance.gameObject.GetOrAddComponent<TraumaCounter>();
    }

    protected override bool IsActive() => CurrentAmount != -1;

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(8, -1.5f),
            CurseCounterPosition.VerticalBlock => new(-2f, -3f),
            CurseCounterPosition.Column => new(0f, -6f),
            _ => new(16f, 0f),
        };
    }

    protected override void LiftCurse()
    {
        base.LiftCurse();
        CurrentAmount = -1;
    }
}
