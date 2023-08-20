using CurseRandomizer.Curses;
using CurseRandomizer.ItemData;
using CurseRandomizer.Modules;
using ItemChanger;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer;

internal class DespairTracker
{
    #region Members

    private int _geoDesperation = 0;
    private int _roomDesperation = 0;
    private int _deathDesperation = 0;
    private int _spellDamageDesperation = 0;
    private int _dealtSpellDamage = 0;
    private int _spendGeo = 0;
    private List<string> _killedEnemies = new();
    private List<string> _passedScenes = new();
    private Coroutine _coroutine;

    #endregion

    #region Properties

    public bool Active { get; set; }

    public int GeoDesperation
    {
        get => _geoDesperation;
        set
        {
            _geoDesperation = Math.Min(10, value);
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }
    }

    public int RoomDesperation
    {
        get => _roomDesperation;
        set
        {
            _roomDesperation = Math.Min(10, value);
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }
    }

    public int DeathDesperation
    {
        get => _deathDesperation;
        set
        {
            _deathDesperation = Math.Min(15, value);
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }
    }

    public int KillDesperation { get; set; }

    public int CurseDesperation { get; set; }

    public int SpellDesperation
    {
        get => _spellDamageDesperation;
        set
        {
            _spellDamageDesperation = Math.Min(20, value);
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }
    }

    public int Ticks { get; set; }

    #endregion

    #region Event handler

    private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        orig(self, hitInstance);
        if (hitInstance.AttackType == AttackTypes.Spell)
        {
            _dealtSpellDamage += hitInstance.DamageDealt;
            if (_dealtSpellDamage >= 100)
            {
                SpellDesperation += _dealtSpellDamage / 100;
                _dealtSpellDamage = 0;
            }
        }
    }

    private void HeroController_TakeGeo(On.HeroController.orig_TakeGeo orig, HeroController self, int amount)
    {
        orig(self, amount);
        _spendGeo += amount;
        if (_spendGeo >= 500)
        {
            GeoDesperation += _spendGeo / 500;
            _spendGeo = 0;
        }
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        _passedScenes.Add(arg1.name);
        if (_passedScenes.Count > 50)
            _passedScenes.RemoveAt(0);
        int sceneCount = 0;
        foreach (string scene in _passedScenes)
        {
            if (scene == arg1.name)
            {
                sceneCount++;
                if (sceneCount > 5)
                    return;
            }
        }
        if (sceneCount == 5)
            RoomDesperation++;
    }

    private void GetPlayerDataInt_OnEnter(On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.orig_OnEnter orig, HutongGames.PlayMaker.Actions.GetPlayerDataInt self)
    {
        orig(self);
        if (self.IsCorrectContext("Hero Death Anim", null, "Remove Geo"))
        {
            DeathDesperation += 3;
            _spendGeo -= self.storeValue.Value;
            DespairCurse despairCurse = CurseManager.GetCurse<DespairCurse>();
            if (despairCurse.CurrentAmount >= despairCurse.NeededAmount)
            {
                despairCurse.RemoveCurse();
                Reset(true);
                StopListening();
            }
        }
    }

    private void ModHooks_RecordKillForJournalHook(EnemyDeathEffects enemyDeathEffects, string playerDataName, string killedBoolPlayerDataLookupKey, string killCountIntPlayerDataLookupKey, string newDataBoolPlayerDataLookupKey)
    {
        if (!_killedEnemies.Contains(playerDataName))
        {
            _killedEnemies.Add(playerDataName);
            KillDesperation++;
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }
    }

    #endregion

    #region Methods

    internal void StartListening()
    {
        On.HealthManager.TakeDamage += HealthManager_TakeDamage;
        On.HeroController.TakeGeo += HeroController_TakeGeo;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += GetPlayerDataInt_OnEnter;
        ModHooks.RecordKillForJournalHook += ModHooks_RecordKillForJournalHook;
        _coroutine = CurseManager.Handler.StartCoroutine(Cursed());
        Active = true;
    }

    internal void StopListening()
    {
        On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
        On.HeroController.TakeGeo -= HeroController_TakeGeo;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter -= GetPlayerDataInt_OnEnter;
        ModHooks.RecordKillForJournalHook -= ModHooks_RecordKillForJournalHook;
        if (_coroutine is not null)
            CurseManager.Handler.StopCoroutine(_coroutine);
    }

    private IEnumerator Cursed()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            yield return new WaitUntil(() => GameManager.instance != null && !GameManager.instance.IsGamePaused());
            Ticks++;
            if (Ticks % 100 == 0)
            {
                try
                {
                    List<string> availableCurses = CurseManager.GetCurses().Where(x => x.Type != CurseType.Despair && x.Data.Active && x.CanApplyCurse()).Select(x => x.Name).ToList();
                    if (availableCurses.Any())
                    {
                        string selectedCurse = availableCurses[UnityEngine.Random.Range(0, availableCurses.Count)];
                        CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
                        module.QueueCurse(selectedCurse);
                        GameHelper.DisplayMessage($"Your hopelessness formed <color={Curse.TextColor}>{selectedCurse}</color>!");
                    }
                    else
                    {
                        GameHelper.DisplayMessage("Surrender to despair...");
                        HeroController.instance.TakeDamage(null, GlobalEnums.CollisionSide.other, 999, 1);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }

    internal void Reset(bool lifting = false)
    {
        if (!TemporaryCurse.EasyLift || lifting)
        {
            _deathDesperation = 0;
            _geoDesperation = 0;
            _roomDesperation = 0;
            _spellDamageDesperation = 0;
            KillDesperation = 0;
            CurseDesperation = 0;
            _passedScenes.Clear();
            _killedEnemies.Clear();
            Ticks = 0;
        }
        Active = false;
    }

    #endregion
}
