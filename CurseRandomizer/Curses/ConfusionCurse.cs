using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using InControl;
using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ConfusionCurse : TemporaryCurse
{
    #region Members

    private PlayerAction[] _actions = new PlayerAction[8];

    #endregion

    #region Properties

    public override int CurrentAmount 
    { 
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = -1;
            return Convert.ToInt32(Data.AdditionalData);
        }
        set => Data.AdditionalData = value;
    }

    public override int NeededAmount => Math.Min(Data.CastedAmount * 5, UseCap ? Cap : 40);

    #endregion

    #region Event handler

    private void InputHandler_SendKeyBindingsToGameSettings(On.InputHandler.orig_SendKeyBindingsToGameSettings orig, InputHandler self)
    {
        if (CurrentAmount == -1)
            orig(self);
    }

    private void InputHandler_SendButtonBindingsToGameSettings(On.InputHandler.orig_SendButtonBindingsToGameSettings orig, InputHandler self)
    {
        if (CurrentAmount == -1)
            orig(self);
    }

    private void ObtainItem(ReadOnlyGiveEventArgs args)
    {
        if (CurrentAmount != -1 && args != null && args.OriginalState == ObtainState.Unobtained)
        {
            if (!DespairCurse.DespairActive)
                CurrentAmount++;
            UpdateProgression();
        }
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.InputHandler.SendButtonBindingsToGameSettings += InputHandler_SendButtonBindingsToGameSettings;
        On.InputHandler.SendKeyBindingsToGameSettings += InputHandler_SendKeyBindingsToGameSettings;
        AbstractItem.BeforeGiveGlobal += ObtainItem;
        _actions[0] = InputHandler.Instance.inputActions.attack;
        _actions[1] = InputHandler.Instance.inputActions.cast;
        _actions[2] = InputHandler.Instance.inputActions.quickCast;
        _actions[3] = InputHandler.Instance.inputActions.dreamNail;
        _actions[4] = InputHandler.Instance.inputActions.openInventory;
        _actions[5] = InputHandler.Instance.inputActions.quickMap;
        _actions[6] = InputHandler.Instance.inputActions.superDash;
        _actions[7] = InputHandler.Instance.inputActions.jump;
        if (CurrentAmount != -1)
            SetBindings(false);
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.InputHandler.SendButtonBindingsToGameSettings -= InputHandler_SendButtonBindingsToGameSettings;
        On.InputHandler.SendKeyBindingsToGameSettings -= InputHandler_SendKeyBindingsToGameSettings;
        AbstractItem.BeforeGiveGlobal -= ObtainItem;
        SetBindings(true);
        base.Unhook();
    }

    public override void ApplyCurse() 
    {
        CurrentAmount = 0;
        SetBindings(false);
        base.ApplyCurse();
    }

    public override int SetCap(int value) => Math.Max(5, Math.Min(40, value));

    public override void ResetAdditionalData() => CurrentAmount = -1;

    protected override bool IsActive() => CurrentAmount != -1;

    protected override void LiftCurse()
    {
        SetBindings(true);
        CurrentAmount = -1;
        base.LiftCurse();
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.Top => new(0f, 5.14f),
            CurseCounterPosition.Right => new(11, -3f),
            CurseCounterPosition.Left => new(-14f, -3f),
            CurseCounterPosition.TopAndBot => new(0f, -8f),
            CurseCounterPosition.Sides => new(11f, 0f),
            _ => new(0f, -6f),
        };
    }

    #endregion

    #region Methods

    private void SetBindings(bool reset)
    {
        if (reset)
        {
            InputHandler.Instance.inputActions.attack = _actions[0];
            InputHandler.Instance.inputActions.cast = _actions[1];
            InputHandler.Instance.inputActions.quickCast = _actions[2];
            InputHandler.Instance.inputActions.dreamNail = _actions[3];
            InputHandler.Instance.inputActions.openInventory = _actions[4];
            InputHandler.Instance.inputActions.quickMap = _actions[5];
            InputHandler.Instance.inputActions.superDash = _actions[6];
            InputHandler.Instance.inputActions.jump = _actions[7];
        }
        else
        {
            List<PlayerAction> viableActions = _actions.ToList();
            PlayerAction selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.attack = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.cast = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.quickCast = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.dreamNail = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.openInventory = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.quickMap = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.superDash = selectedAction;
            viableActions.Remove(selectedAction);

            selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
            InputHandler.Instance.inputActions.jump = selectedAction;
            viableActions.Remove(selectedAction);
        }
    }

    #endregion
}
