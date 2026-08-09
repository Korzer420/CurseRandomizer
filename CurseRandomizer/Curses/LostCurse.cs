using CurseRandomizer.Enums;
using ItemChanger;
using KorzUtils.Helper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Curses;

internal class LostCurse : Curse
{
    
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

    public override bool CanApplyCurse() => GetAvailablePools().Any(x => x);

    public override void ApplyCurse()
    {
        List<bool> slots = GetAvailablePools();
        List<int> availableSlotNumbers = [];
        for (int i = 0; i < slots.Count; i++)
            if (slots[i])
                availableSlotNumbers.Add(i);
        int selectedSlot = availableSlotNumbers[UnityEngine.Random.Range(0, availableSlotNumbers.Count)];
        AbstractItem itemToShuffle = null;
        string message = "";
        if (selectedSlot == 0)
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
            itemToShuffle = Finder.GetItem(ItemNames.Charm_Notch);
            message = "a charm notch";
        }
        else if (selectedSlot == 1)
        {
            HeroController.instance.AddToMaxHealth(-1);
            itemToShuffle = Finder.GetItem(ItemNames.Full_Mask);
            message = "a mask";
            // To force the UI to update to amount of masks.
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else if (selectedSlot == 2)
        {
            HeroController.instance.AddToMaxMPReserve(-1);
            itemToShuffle = Finder.GetItem(ItemNames.Full_Soul_Vessel);
            message = "a vessel";
            // To force the UI to update to amount of masks.
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else if (selectedSlot == 3)
        {
            PDHelper.FireballLevel = 1;
            itemToShuffle = Finder.GetItem(ItemNames.Shade_Soul);
            message = "shade soul";
        }
        else if (selectedSlot == 4)
        {
            PDHelper.QuakeLevel = 1;
            itemToShuffle = Finder.GetItem(ItemNames.Descending_Dark);
            message = "descending dark";
        }
        else
        {
            PDHelper.ScreamLevel = 1;
            itemToShuffle = Finder.GetItem(ItemNames.Abyss_Shriek);
            message = "abyss shriek";
        }

        if (Random.Range(1, 101) <= Data.DespairEnhanced * 5)
            message += " (forever)";
        else
        {
            List<AbstractPlacement> viablePlacements = [];
            foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.GetPlacements())
            {
                if (!placement.AllObtained())
                    continue;
                viablePlacements.Add(placement);
            }
            if (viablePlacements.Count == 0)
                message += " (forever)";
            else
            {
                AbstractPlacement selectedPlacement = viablePlacements[Random.Range(0, viablePlacements.Count)];
                selectedPlacement.Add(itemToShuffle);
            }
        }
        GameHelper.DisplayMessage($"You lost {message}");
    }

    #endregion

    #region Private Methods
    
    private List<bool> GetAvailablePools()
    {
        return [PDHelper.CharmSlots > 1,
        PDHelper.MaxHealthBase > 1,
        (PDHelper.MPReserveMax + PDHelper.MaxMP) / StupidityCurse.SpellCost > 1,
        PDHelper.FireballLevel > 1,
        PDHelper.QuakeLevel > 1,
        PDHelper.ScreamLevel > 1
        ];
    } 

    #endregion

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
}
