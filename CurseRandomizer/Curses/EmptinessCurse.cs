using CurseRandomizer.Components;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using KorzUtils.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class EmptinessCurse : TemporaryCurse
{
    #region Properties

    public override int CurrentAmount
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = new Dictionary<string, int>();
            return ((Dictionary<string, int>)Data.AdditionalData).Values.Sum() - 1;
        }
        set { }
    }

    public override int NeededAmount => Math.Min(Data.CastedAmount, UseCap ? Cap : 20) * 300;

    #endregion

    #region Event handler

    private void HeroController_MaxHealthKeepBlue(On.HeroController.orig_MaxHealthKeepBlue orig, HeroController self)
    {
        if (!IsActive())
            orig(self);
    }

    private void HeroController_MaxHealth(On.HeroController.orig_MaxHealth orig, HeroController self)
    {
        if (!IsActive())
            orig(self);
    }

    private void HeroController_AddHealth(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
    {
        if (!IsActive())
            orig(self, amount);
    }

    private void HealthManager_OnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);
        if (IsActive())
            self.gameObject.AddComponent<EmptinessCounter>();
    }

    private void SetBoolValue_OnEnter(On.HutongGames.PlayMaker.Actions.SetBoolValue.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetBoolValue self)
    {
        if (IsActive() && (self.IsCorrectContext("Spell Control", "Knight", "Focus Heal") || self.IsCorrectContext("Spell Control", "Knight", "Focus Heal 2")))
        { 
            HeroController.instance.TakeDamage(HeroController.instance.gameObject, GlobalEnums.CollisionSide.top, 1, 1);
            HeroController.instance.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
        }
        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (IsActive() && (self.IsCorrectContext("Spell Control", "Knight", "Full HP?") || self.IsCorrectContext("Spell Control", "Knight", "Full HP? 2")) && self.integer1.Name == "HP")
            HeroController.instance.proxyFSM.SendEvent("HeroCtrl-HeroDamaged");
        orig(self);
    }

    #endregion

    #region Controls

    public override void ApplyHooks()
    {
        On.HeroController.MaxHealth += HeroController_MaxHealth;
        On.HeroController.MaxHealthKeepBlue += HeroController_MaxHealthKeepBlue;
        On.HeroController.AddHealth += HeroController_AddHealth;
        On.HealthManager.OnEnable += HealthManager_OnEnable;
        On.HutongGames.PlayMaker.Actions.SetBoolValue.OnEnter += SetBoolValue_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.HeroController.MaxHealth -= HeroController_MaxHealth;
        On.HeroController.MaxHealthKeepBlue -= HeroController_MaxHealthKeepBlue;
        On.HeroController.AddHealth -= HeroController_AddHealth;
        On.HealthManager.OnEnable -= HealthManager_OnEnable;
        On.HutongGames.PlayMaker.Actions.SetBoolValue.OnEnter -= SetBoolValue_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= IntCompare_OnEnter;
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        if (Data.AdditionalData == null)
            Data.AdditionalData = new Dictionary<string, int>();
        Dictionary<string, int> enemyData = (Dictionary<string, int>)Data.AdditionalData;
        if (!EasyLift)
            enemyData.Clear();
        enemyData.Add("Dummy", 1);
        base.ApplyCurse();
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 8));

    protected override bool IsActive() => CurrentAmount >= 0;

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(4f, 1.5f),
            CurseCounterPosition.VerticalBlock => new(-2f, 0f),
            CurseCounterPosition.Column => new(0f, 1.5f),
            _ => new(-4f, 0f),
        };
    }

    protected override void LiftCurse()
    {
        base.LiftCurse();
        HeroController.instance.AddHealth(1);
    }

    #endregion
}