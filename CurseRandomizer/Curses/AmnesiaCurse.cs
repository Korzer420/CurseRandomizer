using ItemChanger.FsmStateActions;
using KorzUtils.Helper;
using System;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// Curse which lowers spell damage by 10%. Has a 20% chance to remove spell upgrades instead.
/// </summary>
internal class AmnesiaCurse : Curse
{
    #region Properties

    public int Stacks
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = 0;
            return Convert.ToInt32(Data.AdditionalData);
        }
        set => Data.AdditionalData = value;
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        On.HutongGames.PlayMaker.Actions.FloatCompare.OnEnter += FloatCompare_OnEnter;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool.OnEnter += FlingObjectsFromGlobalPool_OnEnter;
    }

    public override void Unhook()
    {
        On.HutongGames.PlayMaker.Actions.FloatCompare.OnEnter -= FloatCompare_OnEnter;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool.OnEnter += FlingObjectsFromGlobalPool_OnEnter;
    }

    public override void ApplyCurse() => Stacks++;

    public override bool CanApplyCurse() => Stacks < 9;

    #endregion

    #region Private Methods

    private bool CheckForDespair() => UnityEngine.Random.Range(0, 21) < Mathf.Min(19, Data.DespairEnhanced);

    #endregion

    #region Event handler

    private void FloatCompare_OnEnter(On.HutongGames.PlayMaker.Actions.FloatCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.FloatCompare self)
    {
        if (self.IsCorrectContext("Fireball Control", null, "Init") && Stacks > 0)
            self.Fsm.GameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = CheckForDespair()
                ? 1
                : Convert.ToInt16(Math.Round(self.Fsm.GameObject
                    .LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value * (1 - Stacks * 0.1f), 0, MidpointRounding.AwayFromZero));
        orig(self);
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        // Modify shriek and dive.
        if (self.FsmName == "Set Damage" && self.GetState("Amnesia Penalty") is null)
        {
            self.AddState(new HutongGames.PlayMaker.FsmState(self.Fsm)
            {
                Name = "Amnesia Penalty",
                Actions =
                [
                    new Lambda(() =>
                    {
                        self.gameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = CheckForDespair()
                            ? 1
                            : Convert.ToInt32(Math.Round(self.gameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value * (1 - (Stacks * 0.1f))
                              , MidpointRounding.AwayFromZero));
                    })
                ]
            });
            self.GetState("Set Damage").AdjustTransition("FINISHED", "Amnesia Penalty");
            self.GetState("Amnesia Penalty").AddTransition("FINISHED", "Finished");
        }
        orig(self);
    }

    private void FlingObjectsFromGlobalPool_OnEnter(On.HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool.orig_OnEnter orig, HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool self)
    {
        if (self.IsCorrectContext("Fireball Cast", null, "Flukes"))
        {
            if (CheckForDespair())
            {
                self.spawnMax.Value = 1;
                self.spawnMin.Value = 1;
            }
            else
            {
                int normalAmount = self.Fsm.GameObject.name.Contains("2")
                    ? 16
                    : 9;
                self.spawnMax.Value = Mathf.Max(normalAmount - Stacks, 1);
                self.spawnMin.Value = Mathf.Max(normalAmount - Stacks, 1);
            }

        }
        orig(self);
    }

    #endregion
}
