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

    private IEnumerator GodfinderIcon_Show(On.GodfinderIcon.orig_Show orig, GodfinderIcon self, float delay)
    {
        if (!KnownScenes.ContainsKey("Inactive"))
        {
            if (!DespairCurse.DespairActive)
                KnownScenes["Counter"] = (Convert.ToInt32(KnownScenes["Counter"]) + 1).ToString();
            UpdateProgression();
        }
        yield return orig(self, delay);
    }

    private string ModHooks_BeforeSceneLoadHook(string targetScene)
    {
        if (!KnownScenes.ContainsKey("Inactive") && KnownScenes.Count > 3 && UnityEngine.Random.Range(0, 20) == 0)
        {
            targetScene = KnownScenes.Select(x => x.Key).Where(x => x != "Counter" && x != "Inactive" && x != targetScene).ToArray()[UnityEngine.Random.Range(0, KnownScenes.Count)];
            _toScene = targetScene;
        }
        else
            _toScene = null;
        return targetScene;
    }

    private void GameManager_BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
    {
        if (!KnownScenes.ContainsKey(info.SceneName) && GameManager.instance != null && GameManager.instance.IsGameplayScene())
            KnownScenes.Add(info.SceneName, info.EntryGateName);
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

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        ModHooks.BeforeSceneLoadHook += ModHooks_BeforeSceneLoadHook;
        On.GodfinderIcon.Show += GodfinderIcon_Show;
        On.GameManager.BeginSceneTransition += GameManager_BeginSceneTransition;
        On.GameManager.EnterHero += GameManager_EnterHero;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        ModHooks.BeforeSceneLoadHook -= ModHooks_BeforeSceneLoadHook;
        On.GodfinderIcon.Show -= GodfinderIcon_Show;
        On.GameManager.BeginSceneTransition -= GameManager_BeginSceneTransition;
        On.GameManager.EnterHero -= GameManager_EnterHero;
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
