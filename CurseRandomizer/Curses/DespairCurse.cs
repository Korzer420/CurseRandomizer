using CurseRandomizer.Components;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using ItemChanger;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static bool DespairActive => (CurseManager.GetCurseByType(CurseType.Despair) as DespairCurse).IsActive();

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
        if (!DespairActive)
            orig(self);
    }

    private void HeroController_MaxHealth(On.HeroController.orig_MaxHealth orig, HeroController self)
    {
        if (!DespairActive)
            orig(self);
    }

    private void HeroController_AddHealth(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
    {
        if (!DespairActive)
            orig(self, amount);
    }

    private void HealthManager_OnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);
        if (DespairActive)
            self.gameObject.AddComponent<DespairCounter>();
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.HeroController.MaxHealth += HeroController_MaxHealth;
        On.HeroController.MaxHealthKeepBlue += HeroController_MaxHealthKeepBlue;
        On.HeroController.AddHealth += HeroController_AddHealth;
        On.HealthManager.OnEnable += HealthManager_OnEnable;

        base.ApplyHooks();
        if (IsActive())
            CurseManager.Handler.StartCoroutine(Drain());
    }

    public override void Unhook()
    {
        On.HeroController.MaxHealth -= HeroController_MaxHealth;
        On.HeroController.MaxHealthKeepBlue -= HeroController_MaxHealthKeepBlue;
        On.HeroController.AddHealth -= HeroController_AddHealth;
        On.HealthManager.OnEnable -= HealthManager_OnEnable;

        base.Unhook();
    }

    public override void ApplyCurse()
    {
        Dictionary<string, int> enemyData = (Dictionary<string, int>)Data.AdditionalData;
        enemyData.Clear();
        enemyData.Add("Dummy", 1);
        _soulDrain ??= CurseManager.Handler.StartCoroutine(Drain());
        foreach (HealthManager enemy in GameObject.FindObjectsOfType<HealthManager>())
            if (enemy.GetComponent<DespairCounter>() is null)
                enemy.gameObject.AddComponent<DespairCounter>();
        base.ApplyCurse();
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(20, value));

    protected override void LiftCurse()
    {
        ((Dictionary<string, int>)Data.AdditionalData).Clear();
        if (_soulDrain != null)
            CurseManager.Handler.StopCoroutine(_soulDrain);
        CurseManager.Handler.StartCoroutine(WaitForDamage());
    }

    protected override bool IsActive() => CurrentAmount >= 0;

    public override void ResetAdditionalData() => Data.AdditionalData = null;

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.Right => new(11, 4),
            CurseCounterPosition.Left or CurseCounterPosition.Sides => new(-14f, -2f),
            CurseCounterPosition.Bot => new(5f, -8f),
            _ => new(5f, 7.14f),
        };
    }

    #endregion

    #region Methods

    private IEnumerator WaitForDamage()
    {
        yield return new WaitUntil(() => (bool)_canTakeDamage.Invoke(HeroController.instance, null));
        base.LiftCurse();
        if (PlayerData.instance.GetInt(nameof(PlayerData.instance.permadeathMode)) < 2)
            HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.top, 999, 2);
    }

    private IEnumerator Drain()
    {
        yield return new WaitUntil(() => HeroController.instance != null);
        Dictionary<string, int> enemyData = (Dictionary<string, int>)Data.AdditionalData;
        while (true)
        {
            yield return new WaitForSeconds(3f);
            yield return new WaitUntil(() => GameManager.instance != null && !GameManager.instance.IsGamePaused());
            if (!enemyData.ContainsKey("Dummy"))
                yield break;
            enemyData["Dummy"]++;
            HeroController.instance.TakeMP(Convert.ToInt32(1 + Math.Min(99, enemyData["Dummy"] / 20)));
            if (enemyData["Dummy"] >= 60 && PlayerData.instance.GetInt("geo") >= (enemyData["Dummy"] - 50) / 10)
                HeroController.instance.TakeGeo((enemyData["Dummy"] - 50) / 10);
            if (enemyData["Dummy"] >= 300 && (enemyData["Dummy"] - 260) % 40 == 0)
            {
                List<string> availableCurses = CurseManager.GetCurses().Where(x => x.Type != CurseType.Despair && x.Data.Active && x.CanApplyCurse()).Select(x => x.Name).ToList();
                if (availableCurses.Any())
                {
                    string selectedCurse = availableCurses[UnityEngine.Random.Range(0, availableCurses.Count)];
                    CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
                    module.QueueCurse(selectedCurse);
                    DisplayMessage("Casted_" + selectedCurse);
                }
            }
        }
    }

    #endregion
}
