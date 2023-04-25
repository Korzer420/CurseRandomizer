using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using ItemChanger;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse which applies curses upon being hit. Killing 10 different type of enemies, let the curse vanish.
/// The curse will becoming harder to shaking of, the more it is casted.
/// </summary>
internal class OmenCurse : TemporaryCurse
{
    #region Properties

    public List<string> KilledEnemies
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = new List<string>() { "Inactive" };
            return Data.AdditionalData as List<string>;
        }
    }

    public static bool OmenMode { get; set; }

    public override int CurrentAmount
    {
        get => KilledEnemies.Count;
        set { }
    }

    public override int NeededAmount => Math.Min(Data.CastedAmount * 5, CurseManager.UseCaps ? Cap : 50);

    #endregion

    #region Event handler

    private int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        if (damageAmount > 0 && damageAmount < 420 && !KilledEnemies.Contains("Inactive"))
        {
            List<Curse> availableCurses = CurseManager.GetCurses().Where(x => x.Tag == Enums.CurseTag.Permanent && x.CanApplyCurse() && x.Data.Active).ToList();
            if (!availableCurses.Any())
                damageAmount = 999;
            else
            {
                CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
                string selectedCurse = availableCurses[UnityEngine.Random.Range(0, availableCurses.Count)].Name;
                module.QueueCurse(selectedCurse);

                // Just a filler to track how many curses where casted via Omen.
                if (OmenMode)
                    KilledEnemies.Add("A");
                else if (KilledEnemies.Count + 5 < NeededAmount)
                    KilledEnemies.AddRange(Enumerable.Range(0, 5).Select(x => x + ""));
                else if (KilledEnemies.Count + 1 != NeededAmount)
                    KilledEnemies.AddRange(Enumerable.Range(0, NeededAmount - KilledEnemies.Count - 1).Select(x => x + ""));
                UpdateProgression();
                GameHelper.DisplayMessage("The curse of " + selectedCurse + " was layed upon you.");
            }
        }
        return damageAmount;
    }

    private void ModHooks_RecordKillForJournalHook(EnemyDeathEffects enemyDeathEffects, string playerDataName, string killedBoolPlayerDataLookupKey, string killCountIntPlayerDataLookupKey, string newDataBoolPlayerDataLookupKey)
    {
        if (!KilledEnemies.Contains("Inactive") && !KilledEnemies.Contains(playerDataName) && !OmenMode)
        {
            if (!DespairCurse.DespairActive)
                KilledEnemies.Add(playerDataName);
            UpdateProgression();
        }
    }

    #endregion

    #region Control

    public override bool CanApplyCurse() => !OmenMode;

    public override void ApplyHooks()
    {
        ModHooks.RecordKillForJournalHook += ModHooks_RecordKillForJournalHook;
        ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;
        if (OmenMode)
            KilledEnemies.RemoveAll(x => x != "A");
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        ModHooks.RecordKillForJournalHook -= ModHooks_RecordKillForJournalHook;
        ModHooks.AfterTakeDamageHook -= ModHooks_AfterTakeDamageHook;
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        base.ApplyCurse();
        if (!EasyLift)
            KilledEnemies.Clear();
        else
            KilledEnemies.RemoveAll(x => x == "Inactive");
        UpdateProgression();
    }

    public override int SetCap(int value)
    => Math.Max(5, Math.Min(value, 50));

    public override void ResetAdditionalData() => Data.AdditionalData = new List<string>() { "Inactive" };

    protected override bool IsActive() => !KilledEnemies.Contains("Inactive");

    protected override void LiftCurse()
    {
        base.LiftCurse();
        KilledEnemies.Clear();
        KilledEnemies.Add("Inactive");
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(0f, -1.5f),
            CurseCounterPosition.VerticalBlock => new(-2f, -1.5f),
            CurseCounterPosition.Column => new(0f, -3f),
            _ => new(8f, 0f),
        };
    }

    internal override void UpdateProgression()
    {
        if (OmenMode)
        {
            RepositionTracker();
            TextMeshPro currentCounter = _tracker.GetComponent<DisplayItemAmount>().textObject;
            currentCounter.text = $"{CurrentAmount}";
        }
        else
            base.UpdateProgression();
    }

    #endregion
}
