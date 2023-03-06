using KorzUtils.Helper;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

/// <summary>
/// Curse which lowers spell damage by 10%. Has a 20% chance to remove spell upgrades instead.
/// </summary>
internal class AmnesiaCurse : Curse
{
    #region Constructors

    public AmnesiaCurse()
    {
        // Data is stacks for the damage debuff.
        Data.AdditionalData = 0;
    }

    #endregion

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

    #region Event handler

    private void FloatCompare_OnEnter(On.HutongGames.PlayMaker.Actions.FloatCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.FloatCompare self)
    {
        if (self.IsCorrectContext("Fireball Control", null, "Init") && Stacks > 0)
            self.Fsm.GameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value =
               Convert.ToInt16(Math.Round(self.Fsm.GameObject
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
                Actions = new HutongGames.PlayMaker.FsmStateAction[]
                {
                    new Lambda(() =>
                    {
                        self.gameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value =
                        Convert.ToInt32(Math.Round(self.gameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value * (1 - (Stacks * 0.1f))
                        , MidpointRounding.AwayFromZero));
                    })
                }
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
            int normalAmount = self.Fsm.GameObject.name.Contains("2") ? 16 : 9;
            self.spawnMax.Value = Mathf.Max(normalAmount - Stacks, 1);
            self.spawnMin.Value = Mathf.Max(normalAmount - Stacks, 1);
        }
        orig(self);
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

    public override void ApplyCurse()
    {
        int cap = CurseManager.UseCaps ? Data.Cap : 9;
        if (cap == Stacks)
        {
            if ((Data.Cap > 4 || !CurseManager.UseCaps) && (PlayerData.instance.GetInt(nameof(PlayerData.instance.quakeLevel)) > 1
                || PlayerData.instance.GetInt(nameof(PlayerData.instance.screamLevel)) > 1
                || PlayerData.instance.GetInt(nameof(PlayerData.instance.fireballLevel)) > 1))
            {
                List<string> availableSpells = new();
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.fireballLevel)) > 1)
                    availableSpells.Add("fireballLevel");
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.quakeLevel)) > 1)
                    availableSpells.Add("quakeLevel");
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.screamLevel)) > 1)
                    availableSpells.Add("screamLevel");

                if (!availableSpells.Any())
                    CurseRandomizer.Instance.LogError("Couldn't find a spell to downgrade. This curse shouldn't be allowed to be casted. Report this to the mod developer please.");
                else
                    PlayerData.instance.DecrementInt(availableSpells[UnityEngine.Random.Range(0, availableSpells.Count)]);
            }
            else
                CurseRandomizer.Instance.LogError("Couldn't find a spell to downgrade. This curse shouldn't be allowed to be casted. Report this to the mod developer please.");
        }
        else
        {
            List<string> availableSpells = new();
            if ((Data.Cap > 4 || !CurseManager.UseCaps) && (PlayerData.instance.GetInt(nameof(PlayerData.instance.quakeLevel)) > 1
                || PlayerData.instance.GetInt(nameof(PlayerData.instance.screamLevel)) > 1
                || PlayerData.instance.GetInt(nameof(PlayerData.instance.fireballLevel)) > 1))
            {
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.fireballLevel)) > 1)
                    availableSpells.Add("fireballLevel");
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.quakeLevel)) > 1)
                    availableSpells.Add("quakeLevel");
                if (PlayerData.instance.GetInt(nameof(PlayerData.instance.screamLevel)) > 1)
                    availableSpells.Add("screamLevel");
            }

            // 20% chance for a spell upgrade to be taken.
            if (availableSpells.Any() && UnityEngine.Random.Range(0, 5) == 0)
                PlayerData.instance.DecrementInt(availableSpells[UnityEngine.Random.Range(0, availableSpells.Count)]);
            else
                Stacks++;
        }
    }

    public override bool CanApplyCurse()
    {
        // Upgrades can only be taken if cap is above 4.
        if ((Data.Cap > 4 || !CurseManager.UseCaps) && (PlayerData.instance.GetInt(nameof(PlayerData.instance.quakeLevel)) > 1
            || PlayerData.instance.GetInt(nameof(PlayerData.instance.screamLevel)) > 1
            || PlayerData.instance.GetInt(nameof(PlayerData.instance.fireballLevel)) > 1))
            return true;

        int cap = CurseManager.UseCaps ? Data.Cap : 9;
        return Stacks < cap;
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 9)); 

    #endregion
}
