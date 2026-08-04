using CurseRandomizer.Enums;
using KorzUtils.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class LostCurse : Curse
{
    #region Event Handler

    /// <summary>
    /// Fix the shade health.
    /// </summary>
    private void GetPlayerDataInt_OnEnter(On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.orig_OnEnter orig, HutongGames.PlayMaker.Actions.GetPlayerDataInt self)
    {
        orig(self);
        if (self.IsCorrectContext("Shade Control", null, "Init") && self.intName.Value == "shadeHealth")
            self.storeValue.Value += Data.CastedAmount;
    }

    #endregion

    #region Control

    public override CurseTag Tag => CurseTag.Instant;

    public override void ApplyHooks()
    {
        base.ApplyHooks();
        On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += GetPlayerDataInt_OnEnter;
    }

    public override void Unhook()
    {
        base.Unhook();
        On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter -= GetPlayerDataInt_OnEnter;
    }

    public override bool CanApplyCurse() => PDHelper.CharmSlots > 1 
        || (PDHelper.MPReserveMax + PDHelper.MaxMP) / StupidityCurse.SpellCost > 1 || PDHelper.MaxHealthBase > 1;

    public override void ApplyCurse()
    {
        List<string> viableSlots = [];
        

        for (int i = 0; i < 1 + DespairCurse.CastedDespair / 2; i++)
        {
            viableSlots.Clear();
            if (PDHelper.CharmSlots > 1)
                viableSlots.Add("charmSlots");
            if (PDHelper.MaxHealthBase > 1)
                viableSlots.Add("masks");
            int maxMp = PDHelper.MPReserveMax + PDHelper.MaxMP;
            if (maxMp / StupidityCurse.SpellCost > 1)
                viableSlots.Add("vessels");
            if (viableSlots.Count == 0)
                break;
            int rolledConsumable = Random.Range(0, 3);
            if (rolledConsumable == 0)
            {
                PlayerData.instance.DecrementInt(nameof(PlayerData.charmSlots));
                // Unequip all charms
                PlayerData.instance.GetVariable<List<int>>(nameof(PlayerData.instance.equippedCharms)).RemoveAll(x =>
                {
                    if (x == 36)
                        return false;
                    PlayerData.instance.SetBool("equippedCharm_" + x, false);
                    return true;
                });
                HeroController.instance.CharmUpdate();
                PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
                GameHelper.DisplayMessage("FOOL! (You lost a charm notch)");
            }
            else
            {
                if (rolledConsumable == 1)
                {
                    HeroController.instance.AddToMaxHealth(-1);
                    GameHelper.DisplayMessage("FOOL! (You lost a mask)");
                }
                else
                {
                    HeroController.instance.AddToMaxMPReserve(-1);
                    GameHelper.DisplayMessage("FOOL! (You lost a vessel)");
                }

                // To force the UI to update to amount of masks.
                if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                    GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                else
                {
                    GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                    GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                }
            }
        }
    }

    #endregion
}
