using CurseRandomizer.Enums;
using CurseRandomizer.Helper;
using HutongGames.PlayMaker;
using ItemChanger;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// A curse which limits the vision range until you traverse 10 different rooms.
/// The vision and needed rooms will increase with the amount this curse is casted.
/// </summary>
internal class DarknessCurse : Curse
{
    #region Members

    private string _enteredTransition;

    private string _currentScene;

    #endregion

    #region Constructors

    public DarknessCurse()
    {
        Data.Data = new List<string>() { "Inactive" };
    }

    #endregion

    #region Properties

    public override CurseTag Tag => CurseTag.Temporarly;

    public List<string> PassedScenes
    { 
        get
        {
            if (Data.Data == null)
                Data.Data = new List<string>() { "Inactive" };
            return Data.Data as List<string>;
        }
    }

    #endregion

    #region Event handler

    private void TransitionPoint_OnTriggerEnter2D(On.TransitionPoint.orig_OnTriggerEnter2D orig, TransitionPoint self, Collider2D movingObj)
    {
        if (!string.IsNullOrEmpty(_enteredTransition) && !string.IsNullOrEmpty(_currentScene) && !PassedScenes.Contains("Inactive")
            && !PassedScenes.Contains(_currentScene))
        {
            if ((_enteredTransition.StartsWith("left") && self.entryPoint.StartsWith("right"))
            || (_enteredTransition.StartsWith("right") && self.entryPoint.StartsWith("left"))
            || (_enteredTransition.StartsWith("top") && self.entryPoint.StartsWith("bot"))
            || (_enteredTransition.StartsWith("bot") && self.entryPoint.StartsWith("top")))
            {
                PassedScenes.Add(_currentScene);
                if (PassedScenes.Count >= 10)
                {
                    PassedScenes.Clear();
                    PassedScenes.Add("Inactive");
                    CurseManager.Handler.StartCoroutine(DisplayCurseLift());
                }
            }
        }
        orig(self, movingObj);
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        if (!PassedScenes.Contains("Inactive"))
        {
            _currentScene = arg1.name;
            CurseManager.Handler.StartCoroutine(WaitForEnter());
        }
        else
            _currentScene = null;
        
    }

    private void SetVector3XYZ_DoSetVector3XYZ(On.HutongGames.PlayMaker.Actions.SetVector3XYZ.orig_DoSetVector3XYZ orig, HutongGames.PlayMaker.Actions.SetVector3XYZ self)
    {
        orig(self);
        if (self.IsCorrectContext("Darkness Control", "Vignette", null) && !PassedScenes.Contains("Inactive"))
        {
            int cap = CurseManager.UseCaps ? Cap : 6;
            Vector3 normalScale = new(self.x.Value, self.y.Value);
            float darknessFactor = 1 - /*Math.Min(cap, Data.CastedAmount)*/ 6 * 0.15f;
            self.vector3Variable.Value = new(Math.Max(0.4f,normalScale.x * darknessFactor), Math.Max(0.4f, normalScale.y * darknessFactor), Math.Max(0.4f, normalScale.z * darknessFactor));
        }
    }

    #endregion

    private IEnumerator WaitForEnter()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput);
        _enteredTransition = HeroController.instance.GetEntryGateName();
    }

    private IEnumerator DisplayCurseLift()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput);
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "Curse_Randomizer_Remove_Darkness";
        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
    }

    #region Control

    public override void ApplyHooks()
    {
        On.HutongGames.PlayMaker.Actions.SetVector3XYZ.DoSetVector3XYZ += SetVector3XYZ_DoSetVector3XYZ;
        On.TransitionPoint.OnTriggerEnter2D += TransitionPoint_OnTriggerEnter2D;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public override void Unhook()
    {
        On.HutongGames.PlayMaker.Actions.SetVector3XYZ.DoSetVector3XYZ -= SetVector3XYZ_DoSetVector3XYZ;
        On.TransitionPoint.OnTriggerEnter2D -= TransitionPoint_OnTriggerEnter2D;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public override void ApplyCurse() => PassedScenes.Clear();

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 6));

    public override void ResetAdditionalData()
    {
        PassedScenes.Clear();
        PassedScenes.Add("Inactive");
    }

    #endregion
}
