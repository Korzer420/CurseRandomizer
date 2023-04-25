using CurseRandomizer.Enums;
using HutongGames.PlayMaker;
using KorzUtils.Helper;
using System;
using System.Collections.Generic;

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

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 1;
        int spellCost = 33 + CurseManager.GetCurse<StupidityCurse>().Data.CastedAmount * 3;
        int maxMp = PlayerData.instance.GetInt(nameof(PlayerData.instance.MPReserveMax)) + PlayerData.instance.GetInt(nameof(PlayerData.instance.maxMP));
        return PlayerData.instance.GetInt("charmSlots") > cap || maxMp / spellCost > cap
            || PlayerData.instance.GetInt(nameof(PlayerData.instance.maxHealthBase)) > cap;
    }

    public override void ApplyCurse()
    {
        List<string> viableSlots = new();
        int cap = UseCap
        ? Cap
        : 1;

        if (PlayerData.instance.GetInt("charmSlots") > cap)
            viableSlots.Add("charmSlots");
        if (PlayerData.instance.GetInt(nameof(PlayerData.instance.maxHealthBase)) > cap)
            viableSlots.Add("masks");
        int spellCost = 33 + CurseManager.GetCurse<StupidityCurse>().Data.CastedAmount * 3;
        int maxMp = PlayerData.instance.GetInt(nameof(PlayerData.instance.MPReserveMax)) + PlayerData.instance.GetInt(nameof(PlayerData.instance.maxMP));
        if (maxMp / spellCost > cap)
            viableSlots.Add("vessels");
        string rolledConsumable = null;

        if (rolledConsumable == "charmSlots")
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
            if (rolledConsumable == "masks")
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

    public override int SetCap(int value) => Math.Max(0, Math.Min(value, 11)); 

    #endregion
}
