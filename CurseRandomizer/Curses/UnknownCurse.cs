using CurseRandomizer.Enums;
using HutongGames.PlayMaker;
using ItemChanger;
using ItemChanger.FsmStateActions;
using ItemChanger.UIDefs;
using KorzUtils.Data;
using KorzUtils.Helper;
using Modding;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse, which hides the mask, geo, essence, soul or map.
/// </summary>
internal class UnknownCurse : Curse
{
    #region Members

    private bool _bingoUIUsed = false;
    private GameObject _maskCover;
    private GameObject _soulCover;

    #endregion

    #region Constructors

    public UnknownCurse()
    {
        Data.AdditionalData = new List<AffectedVisual>();
    }

    #endregion

    #region Properties

    private List<AffectedVisual> Affected
    {
        get
        {
            Data ??= new();
            if (Data.AdditionalData == null)
                Data.AdditionalData = new List<AffectedVisual>();
            return Data.AdditionalData as List<AffectedVisual>;
        }
    }

    public static bool AreCursesHidden => CurseManager.GetCurse<UnknownCurse>().Affected.Contains(AffectedVisual.Items);

    #endregion

    #region Event handler

    private void GeoCounter_Update(On.GeoCounter.orig_Update orig, GeoCounter self)
    {
        orig(self);
        if (Affected.Contains(AffectedVisual.Geo))
        {
            if (!_bingoUIUsed)
                self.geoTextMesh.text = "???";
            else
            {
                int bingoIndex = 0;
                while (bingoIndex < self.geoTextMesh.text.Length && self.geoTextMesh.text[bingoIndex] != '(')
                    bingoIndex++;
                self.geoTextMesh.text = "??? " + self.geoTextMesh.text.Substring(bingoIndex);
            }
        }
    }

    private void DisplayItemAmount_OnEnable(On.DisplayItemAmount.orig_OnEnable orig, DisplayItemAmount self)
    {
        orig(self);
        if ((self.playerDataInt == "geo" && Affected.Contains(AffectedVisual.Geo)) ||
            (self.playerDataInt == "dreamOrbs" && Affected.Contains(AffectedVisual.Essence)))
            self.textObject.text = "???";
    }

    private void SetTextMeshProText_OnEnter(On.HutongGames.PlayMaker.Actions.SetTextMeshProText.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetTextMeshProText self)
    {
        if (self.IsCorrectContext("Control", "Dream Nail", "Update") && Affected.Contains(AffectedVisual.Essence))
            self.textString.Value = "???";
        orig(self);
    }

    private void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
    {
        orig(self);
        if (self.Fsm.Name == "Update Vessels" && self.Name == "Idle" && Affected.Contains(AffectedVisual.Soul))
            self.ClearTransitions();
    }

    private void SetBoolValue_OnEnter(On.HutongGames.PlayMaker.Actions.SetBoolValue.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetBoolValue self)
    {
        orig(self);
        if (self.IsCorrectContext("Soul Orb Control", "Soul Orb", "Idle") && Affected.Contains(AffectedVisual.Soul))
            self.State.ClearTransitions();
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "Fury" && self.gameObject.name == "Charm Effects")
        {
            self.AddState(new FsmState(self.Fsm)
            {
                Name = "Check for Unknown",
                Actions = new FsmStateAction[]
                {
                new Lambda(() => self.SendEvent(Affected.Contains(AffectedVisual.Health) ? "HIDDEN FURY" : "FINISHED"))
                }
            });
            self.GetState("Get Ref").AdjustTransition("FINISHED", "Check for Unknown");

            // Skip all visual and sound effects. We only set the damage.
            FsmStateAction[] actions = self.GetState("Activate").Actions.Skip(8).Take(9).ToArray();
            self.AddState(new FsmState(self.Fsm)
            {
                Name = "Hidden Fury",
                Actions = actions,
                Transitions = self.GetState("Activate").Transitions
            });

            self.GetState("Check for Unknown").AddTransition("HIDDEN FURY", "Hidden Fury");
            self.GetState("Check for Unknown").AddTransition("FINISHED", "Activate");
        }
        else if (self.FsmName == "health_display" && self.gameObject.name.StartsWith("Health "))
        {
            self.AddState(new FsmState(self.Fsm)
            {
                Name = "Fake Hiveblood",
                Actions = new FsmStateAction[]
                {
                new Lambda(() =>
                {
                    if (Affected.Contains(AffectedVisual.Health) && self.FsmVariables.FindFsmInt("Health Number").Value == PlayerData.instance.GetInt("health"))
                        self.transform.parent.gameObject.LocateMyFSM("Hive Health Regen").SendEvent("DAMAGE TAKEN");
                })
                }
            });
            self.GetState("Inactive").AddTransition("HERO DAMAGED", "Fake Hiveblood");
            self.GetState("Fake Hiveblood").AddTransition("FINISHED", "Inactive");
        }
        else if (self.FsmName == "blue_health_display" && self.gameObject.name != "Joni Health" && Affected.Contains(AffectedVisual.Health))
            self.GetState("Init").AdjustTransition("FINISHED", "Destroy Self");
        //else if (self.FsmName == "UI Charms")
        //{
        //    self.AddState(new FsmState(self.Fsm)
        //    {
        //        Name = "Blinded?",
        //        Actions = new FsmStateAction[]
        //        {
        //            new Lambda(() =>
        //            {
        //                if (Affected.Contains(AffectedVisual.Charms))
        //                    self.SendEvent("FINISHED");
        //                else
        //                    self.SendEvent("OVERCHARMED");
        //            })
        //        }
        //    });
        //    self.GetState("Overcharmed?").AdjustTransition("OVERCHARM", "Blinded?");
        //    self.GetState("Blinded?").AddTransition("FINISHED", "Not overcharmed");
        //    self.GetState("Blinded?").AddTransition("OVERCHARMED", "Over Notches 2");

        //    self.AddState(new FsmState(self.Fsm)
        //    {
        //        Name = "Blinded? 2",
        //        Actions = new FsmStateAction[]
        //        {
        //            new Lambda(() =>
        //            {
        //                if (Affected.Contains(AffectedVisual.Charms))
        //                {
        //                    PlayerData.instance.SetBool(nameof(PlayerData.overcharmed), true);
        //                    self.SendEvent("OVERCHARMED");
        //                }
        //            })
        //        }
        //    });
        //    self.GetState("Blinded? 2").AddTransition("OVERCHARMED", "Activate UI");
        //    self.GetState("Blinded? 2").AddTransition("FINISHED", "OC Set");
        //    self.GetState("Overcharm Check").AdjustTransition("OVERCHARM", "Blinded? 2");

        //    self.AddState(new FsmState(self.Fsm)
        //    {
        //        Name = "Blinded? 3",
        //        Actions = new FsmStateAction[]
        //        {
        //            new Lambda(() =>
        //            {
        //                if (Affected.Contains(AffectedVisual.Charms))
        //                    self.SendEvent("FINISHED");
        //                else
        //                    self.SendEvent("OVERCHARM");
        //            })
        //        }
        //    });
        //    self.GetState("End Overcharm?").AdjustTransition("OVERCHARM", "Blinded? 3");
        //    self.GetState("Blinded? 3").AddTransition("FINISHED", "Tween Down");
        //    self.GetState("Blinded? 3").AddTransition("OVERCHARM", "Remain Overcharmed");

        //    self.AddState(new FsmState(self.Fsm)
        //    {
        //        Name = "Blinded? 4",
        //        Actions = new FsmStateAction[]
        //        {
        //            new Lambda(() =>
        //            {
        //                if (Affected.Contains(AffectedVisual.Charms))
        //                    self.SendEvent("OVER");
        //                else if (self.FsmVariables.FindFsmInt("Slots Filled").Value >= self.FsmVariables.FindFsmInt("Slots").Value)
        //                    self.SendEvent("FULL");
        //                else
        //                    self.SendEvent("NOT FULL");
        //            })
        //        }
        //    });
        //    self.GetState("Blinded? 4").AddTransition("OVER", "Activate UI");
        //    self.GetState("Blinded? 4").AddTransition("FULL", "No Open Slot");
        //    self.GetState("Blinded? 4").AddTransition("NOT FULL", "Open Slot");
        //    self.GetState("Open Slot?").AdjustTransition("FULL", "Blinded? 4");
        //    self.GetState("Open Slot?").AdjustTransition("NOT FULL", "Blinded? 4");
        //}
        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if ((self.IsCorrectContext("health_display", null, "Check Max HP") || self.IsCorrectContext("health_display", null, "Appear?"))
            && self.Fsm.GameObjectName.StartsWith("Health ") && Affected.Contains(AffectedVisual.Health))
            self.integer1.Value = 1;
        else if ((self.IsCorrectContext("Knight Damage", null, "Last HP?") || self.IsCorrectContext("Low Health FX", "Health", "Low Health?")
            || self.IsCorrectContext("Low Health FX", "Health", "HUD In HP Check") || self.IsCorrectContext("Low Health FX", "Health", "Init"))
            && Affected.Contains(AffectedVisual.Health))
            self.integer1.Value = 99;
        else if (self.IsCorrectContext("Soul Orb Control", "Soul Orb", "Init") && Affected.Contains(AffectedVisual.Soul) && self.equal?.Name == "MP IS ZERO")
            self.integer1.Value = 100;
        orig(self);
    }

    private void FadeGroupDown_OnEnter(On.FadeGroupDown.orig_OnEnter orig, FadeGroupDown self)
    {
        orig(self);
        if ((self.IsCorrectContext("Vignette Control", "Low Health Vignette", "Down") || self.IsCorrectContext("Fader", "Low Health Light", "Down"))
            && Affected.Contains(AffectedVisual.Health))
            self.State.ClearTransitions();
    }

    private void HeroAnimationController_PlayIdle(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        cursor.GotoNext(MoveType.After,
            x => x.MatchLdstr("equippedCharm_6"),
            x => x.MatchCallvirt<PlayerData>("GetBool"));
        cursor.EmitDelegate<Func<bool, bool>>(x => x || Affected.Contains(AffectedVisual.Health));
    }

    private void SetPosition_OnEnter(On.HutongGames.PlayMaker.Actions.SetPosition.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetPosition self)
    {
        if ((self.IsCorrectContext("Hive Health Regen", "Health", "Reset Timer") || self.IsCorrectContext("blue_health_display", "Joni Health", null)) && Affected.Contains(AffectedVisual.Health))
            self.y = 300f;
        orig(self);
    }
    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.HutongGames.PlayMaker.Actions.SetTextMeshProText.OnEnter += SetTextMeshProText_OnEnter;
        On.GeoCounter.Update += GeoCounter_Update;
        On.DisplayItemAmount.OnEnable += DisplayItemAmount_OnEnable;
        On.HutongGames.PlayMaker.Actions.SetBoolValue.OnEnter += SetBoolValue_OnEnter;
        On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;

        // Health
        On.FadeGroupDown.OnEnter += FadeGroupDown_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        IL.HeroAnimationController.PlayIdle += HeroAnimationController_PlayIdle;
        On.HutongGames.PlayMaker.Actions.SetPosition.OnEnter += SetPosition_OnEnter;

        _bingoUIUsed = ModHooks.GetMod("BingoUI") is Mod;

        CurseManager.Handler.StartCoroutine(CoroutineHelper.WaitForHero(() =>
        {
            if (Affected.Contains(AffectedVisual.Health))
            {
                GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo/Geo Amount").gameObject;
                Transform parent = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas");
                GameObject cover = GameObject.Instantiate(prefab, parent, true);
                cover.layer = parent.gameObject.layer;
                cover.transform.position = new(-10.285f, 6.3351f, 0.1094f);
                cover.transform.localScale = new(1.4f, 1.4f, 0.751f);
                cover.GetComponent<TMP_Text>().fontSize = 4;
                cover.GetComponent<TMP_Text>().color = new(1f, 0f, 1f);
                cover.GetComponent<TMP_Text>().text = "?";
                _maskCover = cover;
                _maskCover.SetActive(true);
                GameObject.DontDestroyOnLoad(_maskCover);
            }
            if (Affected.Contains(AffectedVisual.Soul))
            {
                GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo/Geo Amount").gameObject;
                Transform parent = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas");
                GameObject cover = GameObject.Instantiate(prefab, parent, true);
                cover.transform.position = new(-12.1442f, 6.2679f, 0.1093f);
                cover.transform.localScale = new(1.2369f, 1.2369f, 0.751f);
                cover.GetComponent<TMP_Text>().fontSize = 12;
                cover.GetComponent<TMP_Text>().color = new(1f, 0f, 1f);
                cover.GetComponent<TMP_Text>().text = "?";
                _soulCover = cover;
                _soulCover.SetActive(true);
                GameObject.DontDestroyOnLoad(_soulCover);
            }
        }));
    }

    public override void Unhook()
    {
        On.HutongGames.PlayMaker.Actions.SetTextMeshProText.OnEnter -= SetTextMeshProText_OnEnter;
        On.GeoCounter.Update -= GeoCounter_Update;
        On.DisplayItemAmount.OnEnable -= DisplayItemAmount_OnEnable;
        On.HutongGames.PlayMaker.Actions.SetBoolValue.OnEnter -= SetBoolValue_OnEnter;
        On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;

        // Health
        On.FadeGroupDown.OnEnter -= FadeGroupDown_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= IntCompare_OnEnter;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        IL.HeroAnimationController.PlayIdle -= HeroAnimationController_PlayIdle;
        On.HutongGames.PlayMaker.Actions.SetPosition.OnEnter -= SetPosition_OnEnter;

        if (_maskCover != null)
            GameObject.Destroy(_maskCover);
        if (_soulCover != null)
            GameObject.Destroy(_soulCover);
    }

    public override void ApplyCurse()
    {
        List<AffectedVisual> viableVisuals = (Enum.GetValues(typeof(AffectedVisual)) as AffectedVisual[]).Except(Affected).ToList();
        AffectedVisual chosen = viableVisuals[UnityEngine.Random.Range(0, viableVisuals.Count)];
        Affected.Add(chosen);
        GameHelper.DisplayMessage("FOOL! (You can no longer see your " + chosen + ")");
        if (chosen == AffectedVisual.Health || chosen == AffectedVisual.Soul)
        {
            GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo/Geo Amount").gameObject;
            Transform parent = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas");
            GameObject cover = GameObject.Instantiate(prefab, parent, true);
            cover.layer = parent.gameObject.layer;

            if (chosen == AffectedVisual.Soul)
            {
                cover.transform.position = new(-12.1442f, 6.2679f, 0.1093f);
                cover.transform.localScale = new(1.2369f, 1.2369f, 0.751f);
                cover.GetComponent<TMP_Text>().fontSize = 12;
                cover.GetComponent<TMP_Text>().color = new(1f, 0f, 1f);
                cover.GetComponent<TMP_Text>().text = "?";
                _soulCover = cover;
                HeroController.instance.AddMPCharge(200);
                _soulCover.SetActive(true);
                GameObject.DontDestroyOnLoad(_soulCover);
            }
            else
            {
                cover.transform.position = new(-10.285f, 6.3351f, 0.1094f);
                cover.transform.localScale = new(1.4f, 1.4f, 0.751f);
                cover.GetComponent<TMP_Text>().fontSize = 4;
                cover.GetComponent<TMP_Text>().color = new(1f, 0f, 1f);
                cover.GetComponent<TMP_Text>().text = "?";
                _maskCover = cover;
                // To force the UI to update to amount of masks.
                if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                    GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                else
                {
                    GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                    GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                }
                _maskCover.SetActive(true);
                GameObject.DontDestroyOnLoad(_maskCover);
            }
        }
        else if (chosen == AffectedVisual.Items)
        {
            foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values)
                foreach (AbstractItem item in placement.Items)
                    if (item.GetResolvedUIDef() is MsgUIDef msgUIDef)
                    {
                        msgUIDef.name = new BoxedString("???");
                        msgUIDef.shopDesc = new BoxedString("You'll need this ???, otherwise you can't continue your journey. Although ??? might be a good substitution. \n<i>You don't know some of these words.</i>");
                        msgUIDef.sprite = new CustomSprite("Fool");
                    }
        }
    }

    public override bool CanApplyCurse() => Data.CastedAmount < (CurseManager.UseCaps ? Data.Cap : 5);

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 5));

    public override void ResetAdditionalData() => Affected.Clear();

    #endregion
}
