using CurseRandomizer.Components;
using CurseRandomizer.Enums;
using CurseRandomizer.Helper;
using CurseRandomizer.ItemData;
using MapChanger;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class MazeCurse : TemporaryCurse
{
    #region Members

    private string _toScene = null;

    #endregion

    #region Properties

    public Dictionary<string, string> KnownScenes
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = new Dictionary<string, string>() { { "Counter", "0" }, { "Inactive", "Inactive" } };
            return Data.AdditionalData as Dictionary<string, string>;
        }
    }

    public override int CurrentAmount
    {
        get => Convert.ToInt32(KnownScenes["Counter"]);
        set { }
    }

    public override int NeededAmount => Math.Min(UseCap ? Cap : 10, Data.CastedAmount);

    #endregion

    #region Event handler

    private void GameManager_BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
    {
        if (!KnownScenes.ContainsKey(info.SceneName) && GameManager.instance != null && GameManager.instance.IsGameplayScene()
            && !info.SceneName.Contains("Dream"))
            KnownScenes.Add(info.SceneName, info.EntryGateName);

        if (!KnownScenes.ContainsKey("Inactive") && !string.IsNullOrEmpty(info.EntryGateName) 
            && (info.EntryGateName.StartsWith("left") || info.EntryGateName.StartsWith("right")
            || info.EntryGateName.StartsWith("top") || info.EntryGateName.StartsWith("bot"))
            && KnownScenes.Count > 3 && UnityEngine.Random.Range(0, 100) <= 7)
        {
            List<string> viableScenes = KnownScenes.Select(x => x.Key).Where(x => x != "Counter" && x != "Inactive" && x != info.SceneName).ToList();
            if (viableScenes.Any())
            {
                info.SceneName = viableScenes[UnityEngine.Random.Range(0, viableScenes.Count)];
                _toScene = info.SceneName;
            }
            else
                _toScene = null;
        }
        else
            _toScene = null;
        orig(self, info);
    }

    private void GameManager_EnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
    {
        if (_toScene != null)
        {
            if (GameObject.Find(self.entryGateName) == null)
                self.entryGateName = GameObject.FindObjectOfType<TransitionPoint>().name;
            CurseManager.Handler.StartCoroutine(WaitForControl());
            _toScene = null;
        }
        orig(self, additiveGateSearch);
    }

    private void HealthManager_OnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);
        if (self.hp >= 200)
            self.gameObject.AddComponent<MazeViable>();
    }

    private void HealthManager_Die(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
    {
        orig(self, attackDirection, attackType, ignoreEvasion);
        if (self.gameObject.GetComponent<MazeViable>() != null && !KnownScenes.ContainsKey("Inactive"))
        {
            KnownScenes["Counter"] = (Convert.ToInt32(KnownScenes["Counter"]) + 1).ToString();
            UpdateProgression();
        }
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.GameManager.BeginSceneTransition += GameManager_BeginSceneTransition;
        On.GameManager.EnterHero += GameManager_EnterHero;
        On.HealthManager.OnEnable += HealthManager_OnEnable;
        On.HealthManager.Die += HealthManager_Die;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.GameManager.BeginSceneTransition -= GameManager_BeginSceneTransition;
        On.GameManager.EnterHero -= GameManager_EnterHero;
        On.HealthManager.OnEnable -= HealthManager_OnEnable;
        On.HealthManager.Die -= HealthManager_Die;
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        KnownScenes.Remove("Inactive");
        KnownScenes["Counter"] = "0";
        base.ApplyCurse();
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(10, value));

    protected override bool IsActive() => !KnownScenes.ContainsKey("Inactive");

    public override void ResetAdditionalData()
    {
        Data.AdditionalData = new Dictionary<string, string>() { { "Counter", "0" }, { "Inactive", "Inactive" } };
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.Top => new(5f, 5.14f),
            CurseCounterPosition.Left => new(-14f, -5f),
            CurseCounterPosition.Right => new(11f, -5f),
            CurseCounterPosition.Bot => new(5f, -6f),
            CurseCounterPosition.Sides => new(11f, -2f),
            _ => new(5f, -8f),
        };
    }

    #endregion

    #region Methods

    private IEnumerator WaitForControl()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput);
        DisplayMessage("Teleported");
    }

    protected override void LiftCurse()
    {
        base.LiftCurse();
        KnownScenes.Add("Inactive", "Inactive");
        KnownScenes["Counter"] = "0";
    }

    #endregion
}
