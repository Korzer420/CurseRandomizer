using CurseRandomizer.Enums;
using KorzUtils.Helper;
using CurseRandomizer.ItemData;
using GlobalEnums;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse which limits the vision range until you traverse 10 different rooms.
/// The vision and needed rooms will increase with the amount this curse is casted.
/// </summary>
internal class DarknessCurse : TemporaryCurse
{
    #region Members

    private GatePosition _enteredTransition;

    private string _currentScene;

    #endregion

    #region Properties

    public List<string> PassedScenes
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = new List<string>() { "Inactive" };
            return Data.AdditionalData as List<string>;
        }
    }

    public override int NeededAmount => Math.Min(Data.CastedAmount * 5, CurseManager.UseCaps ? Cap : 50);

    public override int CurrentAmount { get => PassedScenes.Count; set { } }

    #endregion

    #region Event handler

    private void SetVector3XYZ_DoSetVector3XYZ(On.HutongGames.PlayMaker.Actions.SetVector3XYZ.orig_DoSetVector3XYZ orig, HutongGames.PlayMaker.Actions.SetVector3XYZ self)
    {
        orig(self);
        if (self.IsCorrectContext("Darkness Control", "Vignette", null) && !PassedScenes.Contains("Inactive"))
        {
            int cap = CurseManager.UseCaps ? Cap : 5;
            Vector3 normalScale = new(self.x.Value, self.y.Value);
            float darknessFactor = 1 - ((Math.Min(cap, Data.CastedAmount) + 1) * 0.15f);
            self.vector3Variable.Value = new(Math.Max(0.4f, normalScale.x * darknessFactor), Math.Max(0.4f, normalScale.y * darknessFactor), Math.Max(0.4f, normalScale.z * darknessFactor));
        }
    }

    private void HeroController_LeaveScene(On.HeroController.orig_LeaveScene orig, HeroController self, GlobalEnums.GatePosition? gate)
    {
        orig(self, gate);
        CurseManager.Handler.StartCoroutine(WaitForEnter(gate));
    }

    #endregion

    private IEnumerator WaitForEnter(GatePosition? gate)
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput);

        if (gate.HasValue && !PassedScenes.Contains("Inactive"))
        {
            if ((int)gate.Value < 4 && (int)_enteredTransition < 4 && gate.Value != _enteredTransition
                && !PassedScenes.Contains(_currentScene))
            {
                if (!DespairCurse.DespairActive)
                    PassedScenes.Add(_currentScene);
                UpdateProgression();
            }
            switch (gate.Value)
            {
                case GatePosition.top:
                    _enteredTransition = GatePosition.bottom;
                    break;
                case GatePosition.right:
                    _enteredTransition = GatePosition.left;
                    break;
                case GatePosition.left:
                    _enteredTransition = GatePosition.right;
                    break;
                case GatePosition.bottom:
                    _enteredTransition = GatePosition.top;
                    break;
                default:
                    _enteredTransition = GatePosition.unknown;
                    break;
            }
        }
        else
            _enteredTransition = GatePosition.unknown;
        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    protected override void LiftCurse()
    {
        try
        {
            base.LiftCurse();
            PassedScenes.Clear();
            PassedScenes.Add("Inactive");

            HeroController.instance.vignetteFSM.SendEvent("SCENE RESET");
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.LogError("An error occured while trying to lift the darkness curse: " + exception.ToString());
        }
    }

    #region Control

    public override void ApplyHooks()
    {
        On.HutongGames.PlayMaker.Actions.SetVector3XYZ.DoSetVector3XYZ += SetVector3XYZ_DoSetVector3XYZ;
        On.HeroController.LeaveScene += HeroController_LeaveScene;
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.HutongGames.PlayMaker.Actions.SetVector3XYZ.DoSetVector3XYZ -= SetVector3XYZ_DoSetVector3XYZ;
        On.HeroController.LeaveScene -= HeroController_LeaveScene;
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        PassedScenes.Clear();
        base.ApplyCurse();
        HeroController.instance.vignetteFSM.SendEvent("SCENE RESET");
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 5));

    public override void ResetAdditionalData()
    {
        PassedScenes.Clear();
        PassedScenes.Add("Inactive");
    }

    protected override bool IsActive() => !PassedScenes.Contains("Inactive");

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.Bot => new(-5f, -8f),
            CurseCounterPosition.Right => new(11, 2),
            CurseCounterPosition.Left or CurseCounterPosition.Sides => new(-14f, 2f),
            _ => new(-5f, 7.14f),
        };
    }

    #endregion
}
