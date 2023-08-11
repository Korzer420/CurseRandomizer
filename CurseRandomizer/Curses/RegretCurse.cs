using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using CurseRandomizer.Modules;
using ItemChanger;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class RegretCurse : TemporaryCurse
{
    #region Properties

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

    public override int NeededAmount => Math.Min(UseCap ? Cap * 300 : 3000, Data.CastedAmount * 300);

    public List<string> KilledEnemies { get; set; } = new();

    public override CurseTag Tag => CurseTag.Temporarly;

    #endregion

    #region Event handler

    private void ModHooks_RecordKillForJournalHook(EnemyDeathEffects enemyDeathEffects, string playerDataName, string killedBoolPlayerDataLookupKey, string killCountIntPlayerDataLookupKey, string newDataBoolPlayerDataLookupKey)
    {
        if (CurrentAmount != -1)
            CheckIfCurseCast(playerDataName);
    }

    private void HeroController_TakeGeo(On.HeroController.orig_TakeGeo orig, HeroController self, int amount)
    {
        orig(self, amount);
        if (CurrentAmount != -1)
        {
            if (!DespairCurse.DespairActive)
                CurrentAmount += amount;
            UpdateProgression();
        }
    }

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (name == "HasRegrets")
            return CurrentAmount != -1 || orig;
        return orig;
    }

    #endregion

    #region Control

    public override void ApplyCurse()
    {
        if (!EasyLift)
            CurrentAmount = 0;
        base.ApplyCurse();
    }

    public override void ApplyHooks()
    {
        ModHooks.RecordKillForJournalHook += ModHooks_RecordKillForJournalHook;
        On.HeroController.TakeGeo += HeroController_TakeGeo;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        ModHooks.RecordKillForJournalHook -= ModHooks_RecordKillForJournalHook;
        On.HeroController.TakeGeo -= HeroController_TakeGeo;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;

        base.Unhook();
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(10, value));

    public override void ResetAdditionalData() => Data.AdditionalData = -1;

    protected override bool IsActive() => CurrentAmount != -1;

    #endregion

    #region Methods

    private void CheckIfCurseCast(string enemyName)
    {
        int chance = 2 + (KilledEnemies.Count(x => x == enemyName) * 4);
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {
            List<Curse> availableCurses = CurseManager.GetCurses().Where(x => x.Tag == Enums.CurseTag.Instant && x.Data.Active && x.CanApplyCurse()).ToList();
            // Lost curse is the only instant curse which can't be applied if disabled.
            Curse lostCurse = CurseManager.GetCurse<LostCurse>();
            if (!lostCurse.Data.Active)
                availableCurses.Remove(lostCurse);

            if (!availableCurses.Any())
                availableCurses.Add(CurseManager.GetCurse<DisorientationCurse>());
            CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
            string selectedCurse = availableCurses[UnityEngine.Random.Range(0, availableCurses.Count)].Name;
            module.QueueCurse(selectedCurse);

            GameHelper.DisplayMessage($"The sins of <color={TextColor}>" + selectedCurse + "</color> are crawling down your spine...");
            KilledEnemies.Clear();
        }

        if (KilledEnemies.Count == 20)
            KilledEnemies.RemoveAt(19);
        KilledEnemies.Insert(0, enemyName);
    }

    protected override void LiftCurse()
    {
        CurrentAmount = -1;
        KilledEnemies.Clear();
        base.LiftCurse();
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(-4, -1.5f),
            CurseCounterPosition.VerticalBlock => new(2f, 0f),
            CurseCounterPosition.Column => new(0f, -1.5f),
            _ => new(4f, 0f),
        };
    }

    #endregion
}
