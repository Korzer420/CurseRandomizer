using CurseRandomizer.Helper;
using CurseRandomizer.ItemData;
using HutongGames.PlayMaker;
using ItemChanger;
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
internal class OmenCurse : Curse
{
    #region Members

    private GameObject _tracker;

    #endregion

    #region Properties

    public List<string> KilledEnemies
    {
        get
        {
            if (Data.Data == null)
                Data.Data = new List<string>() { "Inactive" };
            return Data.Data as List<string>;
        }
    }

    public int NeededEnemyKills => Math.Min(Data.CastedAmount * 5, CurseManager.UseCaps ? Cap : 50);

    public GameObject Tracker
    {
        get
        {
            if (_tracker == null)
            {
                GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo").gameObject;
                GameObject hudCanvas = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas").gameObject;
                _tracker = GameObject.Instantiate(prefab, hudCanvas.transform, true);
                _tracker.name = "Omen Tracker";
                _tracker.transform.localPosition = new(7.7818f, 0.5418f, 0);
                _tracker.transform.localScale = new(1.3824f, 1.3824f, 1.3824f);
                _tracker.GetComponent<DisplayItemAmount>().playerDataInt = _tracker.name;
                _tracker.GetComponent<DisplayItemAmount>().textObject.text = "";
                _tracker.GetComponent<DisplayItemAmount>().textObject.fontSize = 3;
                _tracker.GetComponent<DisplayItemAmount>().textObject.gameObject.name = "Counter";
                _tracker.GetComponent<SpriteRenderer>().sprite = SpriteHelper.CreateSprite("Omen");
                _tracker.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1f);
                _tracker.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0f);
                _tracker.SetActive(!KilledEnemies.Contains("Inactive"));
            }
            return _tracker;
        }
    }

    #endregion

    #region Event handler

    private int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        if (damageAmount > 0 && damageAmount < 420 && !KilledEnemies.Contains("Inactive"))
        {
            List<Curse> availableCurses = CurseManager.GetCurses().Where(x => x.Tag == Enums.CurseTag.Permanent && x.CanApplyCurse()).ToList();
            if (!availableCurses.Any())
                damageAmount = 999;
            else
            {
                CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
                string selectedCurse = availableCurses[UnityEngine.Random.Range(0, availableCurses.Count)].Name;
                module.QueueCurse(selectedCurse);

                if (KilledEnemies.Count + 10 < NeededEnemyKills)
                    KilledEnemies.AddRange(Enumerable.Range(0, 10).Select(x => x + ""));
                else if (KilledEnemies.Count + 1 != NeededEnemyKills)
                    KilledEnemies.AddRange(Enumerable.Range(0, NeededEnemyKills - KilledEnemies.Count - 1).Select(x => x + ""));
                CheckIfCurseLift();

                PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
                playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
                playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "Curse_Randomizer_Omen_Affect_" + selectedCurse;
                playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
            }
        }
        return damageAmount;
    }

    private void ModHooks_RecordKillForJournalHook(EnemyDeathEffects enemyDeathEffects, string playerDataName, string killedBoolPlayerDataLookupKey, string killCountIntPlayerDataLookupKey, string newDataBoolPlayerDataLookupKey)
    {
        if (!KilledEnemies.Contains("Inactive") && !KilledEnemies.Contains(playerDataName))
        {
            KilledEnemies.Add(playerDataName);
            CheckIfCurseLift();
        }
    }

    #endregion


    #region Methods

    private void CheckIfCurseLift()
    {
        TextMeshPro currentCounter = _tracker.GetComponent<DisplayItemAmount>().textObject;
        currentCounter.text = $"{KilledEnemies.Count} / {NeededEnemyKills}";
        if (KilledEnemies.Count < NeededEnemyKills)
            return;
        CurseRandomizer.Instance.Log("Needed enemy kills: " + NeededEnemyKills + " Killed enemies: " + KilledEnemies.Count);
        KilledEnemies.Clear();
        KilledEnemies.Add("Inactive");
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "Curse_Randomizer_Remove_Omen";
        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
        Tracker.SetActive(false);
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        ModHooks.RecordKillForJournalHook += ModHooks_RecordKillForJournalHook;
        ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;

        GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo").gameObject;
        GameObject hudCanvas = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas").gameObject;
        _tracker = GameObject.Instantiate(prefab, hudCanvas.transform, true);
        _tracker.name = "Omen Tracker";
        _tracker.transform.localPosition = new(7.7818f, 0.5418f, 0);
        _tracker.transform.localScale = new(1.3824f, 1.3824f, 1.3824f);
        _tracker.GetComponent<DisplayItemAmount>().playerDataInt = _tracker.name;
        _tracker.GetComponent<DisplayItemAmount>().textObject.text = "";
        _tracker.GetComponent<DisplayItemAmount>().textObject.fontSize = 3;
        _tracker.GetComponent<DisplayItemAmount>().textObject.gameObject.name = "Counter";
        _tracker.GetComponent<SpriteRenderer>().sprite = SpriteHelper.CreateSprite("Omen");
        _tracker.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1f);
        _tracker.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0f);
        _tracker.SetActive(!KilledEnemies.Contains("Inactive"));
        if (_tracker.activeSelf)
            CheckIfCurseLift();
    }

    public override void Unhook()
    {
        ModHooks.RecordKillForJournalHook -= ModHooks_RecordKillForJournalHook;
        ModHooks.AfterTakeDamageHook -= ModHooks_AfterTakeDamageHook;
        if (_tracker != null)
            GameObject.Destroy(_tracker);
    }

    public override void ApplyCurse()
    {
        KilledEnemies.Clear();
        Tracker.SetActive(true);
        CheckIfCurseLift();
    }

    public override int SetCap(int value)
    => Math.Max(5, Math.Min(value, 50));

    public override void ResetAdditionalData() => Data.Data = new List<string>() { "Inactive" };

    #endregion
}
