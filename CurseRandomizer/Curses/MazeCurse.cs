using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using ItemChanger;
using ItemChanger.Placements;
using KorzUtils.Helper;
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
                Data.AdditionalData = new Dictionary<string, string>() { { "Counter", "-1" } };
            return Data.AdditionalData as Dictionary<string, string>;
        }
    }

    public override int CurrentAmount
    {
        get => Convert.ToInt32(KnownScenes["Counter"]);
        set => KnownScenes["Counter"] = value.ToString();
    }

    public override int NeededAmount => Math.Min(UseCap ? Cap : 50, Data.CastedAmount * 5);

    #endregion

    #region Event handler

    private void GameManager_BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
    {
        if (!KnownScenes.ContainsKey(info.SceneName) && GameManager.instance != null && GameManager.instance.IsGameplayScene()
            && !info.SceneName.Contains("Dream"))
            KnownScenes.Add(info.SceneName, info.EntryGateName);

        if (CurrentAmount != -1 && !string.IsNullOrEmpty(info.EntryGateName)
            && (info.EntryGateName.StartsWith("left") || info.EntryGateName.StartsWith("right")
            || info.EntryGateName.StartsWith("top") || info.EntryGateName.StartsWith("bot"))
            && KnownScenes.Count > 3 && UnityEngine.Random.Range(0, 100) <= 7)
        {
            List<string> viableScenes = KnownScenes.Select(x => x.Key).Where(x => x != "Counter" && x != info.SceneName).ToList();
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

    private void ObtainItem(ReadOnlyGiveEventArgs args)
    {
        if (CurrentAmount != -1 && args != null && ((args.OriginalState == ObtainState.Unobtained && args.Placement is not IMultiCostPlacement) || args.Item?.name == "Generosity"))
        {
            if (!DespairCurse.DespairActive)
                CurrentAmount++;
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

    public override void ApplyHooks()
    {
        On.GameManager.BeginSceneTransition += GameManager_BeginSceneTransition;
        On.GameManager.EnterHero += GameManager_EnterHero;
        AbstractItem.BeforeGiveGlobal += ObtainItem;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.GameManager.BeginSceneTransition -= GameManager_BeginSceneTransition;
        On.GameManager.EnterHero -= GameManager_EnterHero;
        AbstractItem.BeforeGiveGlobal -= ObtainItem;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        if (!EasyLift)
            KnownScenes["Counter"] = "0";
        base.ApplyCurse();
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(10, value));

    protected override bool IsActive() => CurrentAmount != -1;

    public override void ResetAdditionalData()
    {
        Data.AdditionalData = new Dictionary<string, string>() { { "Counter", "-1" } };
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(4, -1.5f),
            CurseCounterPosition.VerticalBlock => new(2f, -1.5f),
            CurseCounterPosition.Column => new(0f, -4.5f),
            _ => new(12f, 0f),
        };
    }

    #endregion

    #region Methods

    private IEnumerator WaitForControl()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput);
        GameHelper.DisplayMessage("???");
    }

    protected override void LiftCurse()
    {
        base.LiftCurse();
        KnownScenes["Counter"] = "-1";
    }

    #endregion
}
