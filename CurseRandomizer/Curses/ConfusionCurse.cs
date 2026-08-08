using CurseRandomizer.Components;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using InControl;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class ConfusionCurse : TemporaryCurse
{
    #region Members

    private PlayerAction[] _actions = new PlayerAction[9];

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

    public override int NeededAmount => Math.Min(Data.CastedAmount, 3);

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

    private void InputHandler_ResetAllControllerButtonBindings(On.InputHandler.orig_ResetAllControllerButtonBindings orig, InputHandler self)
    {
        if (CurrentAmount == -1)
            orig(self);
    }

    private void InputHandler_ResetDefaultControllerButtonBindings(On.InputHandler.orig_ResetDefaultControllerButtonBindings orig, InputHandler self)
    {
        if (CurrentAmount == -1)
            orig(self);
    }

    private void InputHandler_ResetDefaultKeyBindings(On.InputHandler.orig_ResetDefaultKeyBindings orig, InputHandler self)
    {
        if (CurrentAmount == -1)
            orig(self);
    }

    private void HealthManager_OnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);
        if (IsActive() && self.hp >= 200 || self.gameObject.name == "Giant Fly" || self.gameObject.name == "Giant Buzzer" || self.gameObject.name == "Mega Moss Charger")
            self.gameObject.AddComponent<ConfusionViable>();
    }

    private void HealthManager_Die(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
    {
        orig(self, attackDirection, attackType, ignoreEvasion);
        if (IsActive() && self.gameObject.GetComponent<ConfusionViable>() != null)
        {
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
        On.InputHandler.ResetDefaultKeyBindings += InputHandler_ResetDefaultKeyBindings;
        On.InputHandler.ResetDefaultControllerButtonBindings += InputHandler_ResetDefaultControllerButtonBindings;
        On.InputHandler.ResetAllControllerButtonBindings += InputHandler_ResetAllControllerButtonBindings;
        On.HealthManager.OnEnable += HealthManager_OnEnable;
        On.HealthManager.Die += HealthManager_Die;

        _actions[0] = InputHandler.Instance.inputActions.attack;
        _actions[1] = InputHandler.Instance.inputActions.cast;
        _actions[2] = InputHandler.Instance.inputActions.dash;
        _actions[3] = InputHandler.Instance.inputActions.jump;
        _actions[4] = InputHandler.Instance.inputActions.quickCast;
        _actions[5] = InputHandler.Instance.inputActions.dreamNail;
        _actions[6] = InputHandler.Instance.inputActions.superDash;

        if (CurrentAmount != -1)
            SetBindings(false);
        base.ApplyHooks();
    }

    public override void Unhook()
    {
        On.InputHandler.SendButtonBindingsToGameSettings -= InputHandler_SendButtonBindingsToGameSettings;
        On.InputHandler.SendKeyBindingsToGameSettings -= InputHandler_SendKeyBindingsToGameSettings;
        On.InputHandler.ResetDefaultKeyBindings -= InputHandler_ResetDefaultKeyBindings;
        On.InputHandler.ResetDefaultControllerButtonBindings -= InputHandler_ResetDefaultControllerButtonBindings;
        On.InputHandler.ResetAllControllerButtonBindings -= InputHandler_ResetAllControllerButtonBindings;
        On.HealthManager.OnEnable -= HealthManager_OnEnable;
        On.HealthManager.Die -= HealthManager_Die;

        SetBindings(true);
        base.Unhook();
    }

    public override void ApplyCurse()
    {
        if (!EasyLift)
            CurrentAmount = 0;
        SetBindings(false);
        base.ApplyCurse();
    }

    public override void ResetAdditionalData() => CurrentAmount = -1;

    protected override bool IsActive() => CurrentAmount != -1;

    protected override void LiftCurse()
    {
        CurrentAmount = -1;
        SetBindings(true);
        base.LiftCurse();
    }

    protected override Vector2 MoveToPosition(CurseCounterPosition position)
    {
        return position switch
        {
            CurseCounterPosition.HorizontalBlock => new(-4, 1.5f),
            CurseCounterPosition.VerticalBlock => new(-2f, 1.5f),
            CurseCounterPosition.Column => new(0f, 4.5f),
            _ => new(-12f, 0f),
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
            InputHandler.Instance.inputActions.dash = _actions[2];
            InputHandler.Instance.inputActions.jump = _actions[3];
            InputHandler.Instance.inputActions.quickCast = _actions[4];
            InputHandler.Instance.inputActions.dreamNail = _actions[5];
            InputHandler.Instance.inputActions.superDash = _actions[6];
        }
        else
        {
            int affectedAmount = 4 + DespairCurse.CastedDespair;
            List<PlayerAction> viableActions = [.. _actions.Take(affectedAmount)];

            for (int i = 0; i < affectedAmount; i++)
            {
                PlayerAction selectedAction = viableActions[UnityEngine.Random.Range(0, viableActions.Count)];
                switch (i)
                {
                    case 0:
                        InputHandler.Instance.inputActions.attack = selectedAction;
                        break;
                    case 1:
                        InputHandler.Instance.inputActions.cast = selectedAction;
                        break;
                    case 2:
                        InputHandler.Instance.inputActions.dash = selectedAction;
                        break;
                    case 3:
                        InputHandler.Instance.inputActions.jump = selectedAction;
                        break;
                    case 4:
                        InputHandler.Instance.inputActions.quickCast = selectedAction;
                        break;
                    case 5:
                        InputHandler.Instance.inputActions.dreamNail = selectedAction;
                        break;
                    case 6:
                    default:
                        InputHandler.Instance.inputActions.superDash = selectedAction;
                        break;
                }
                viableActions.Remove(selectedAction);
            }
        }
    }

    #endregion
}
