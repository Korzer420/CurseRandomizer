using CurseRandomizer.Enums;
using CurseRandomizer.Helper;
using HutongGames.PlayMaker;
using IL.TMPro;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using Modding;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse, which hides the mask, geo, essence, soul or map.
/// </summary>
internal class UnknownCurse : Curse
{
    #region Members

    private bool _bingoUIUsed = false;

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
            if (Data.AdditionalData == null)
                Data.AdditionalData = new List<AffectedVisual>();
            return Data.AdditionalData as List<AffectedVisual>;
        }
    }

    #endregion

    #region Event handler

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (!string.IsNullOrEmpty(name) && name.StartsWith("map") && Affected.Contains(AffectedVisual.Map))
            return false;
        return orig;
    }

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
                self.geoTextMesh.text = "??? "+self.geoTextMesh.text.Substring(bingoIndex);
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

    // ---------------------- Health Stuff ---------------------------

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
        orig(self);
    }

    private void FadeGroupDown_OnEnter(On.FadeGroupDown.orig_OnEnter orig, FadeGroupDown self)
    {
        orig(self);
        if ((self.IsCorrectContext("Vignette Control", "Low Health Vignette", "Down") || self.IsCorrectContext("Fader", "Low Health Light", "Down"))
            && Affected.Contains(AffectedVisual.Health))
            self.State.ClearTransitions();
    }

    private void HeroAnimationController_PlayIdle(MonoMod.Cil.ILContext il)
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

        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        _bingoUIUsed = ModHooks.GetMod("BingoUI") is Mod;
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

        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
    }

    public override void ApplyCurse()
    {
        List<AffectedVisual> viableVisuals = (Enum.GetValues(typeof(AffectedVisual)) as AffectedVisual[]).Except(Affected).ToList();
#if RELEASE
        AffectedVisual chosen = viableVisuals[UnityEngine.Random.Range(0, viableVisuals.Count)];
        Affected.Add(chosen);
        if (chosen == AffectedVisual.Health)
        {
            // To force the UI to update to amount of masks.
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        } 
        else if (chosen == AffectedVisual.Soul)
             HeroController.instance.AddMPCharge(200);
#endif
    }

    public override bool CanApplyCurse() => Data.CastedAmount < (CurseManager.UseCaps ? Data.Cap : 5);

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 5));

    public override void ResetAdditionalData() => Affected.Clear();

    #endregion
}
