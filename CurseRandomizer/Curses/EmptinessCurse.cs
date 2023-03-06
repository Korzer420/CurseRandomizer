using KorzUtils.Helper;
using CurseRandomizer.Manager;
using System;

namespace CurseRandomizer.Curses;

internal class EmptinessCurse : Curse
{
    #region Event handler

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

    public override void ApplyHooks() => On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += GetPlayerDataInt_OnEnter;

    public override void Unhook() => On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter -= GetPlayerDataInt_OnEnter;

    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 1;
        int spellCost = 33 + CurseManager.GetCurseByType(CurseType.Stupidity).Data.CastedAmount * 3;
        int maxMp = PlayerData.instance.GetInt(nameof(PlayerData.instance.MPReserveMax)) + PlayerData.instance.GetInt(nameof(PlayerData.instance.maxMP));

        return PlayerData.instance.GetInt(nameof(PlayerData.instance.maxHealthBase)) > cap || maxMp / spellCost > cap;
    }

    public override void ApplyCurse()
    {
        int cap = UseCap ? Cap : 1;
        if (PlayerData.instance.GetInt(nameof(PlayerData.instance.maxHealthBase)) > cap)
        {
            int spellCost = 33 + CurseManager.GetCurseByType(CurseType.Stupidity).Data.CastedAmount * 3;
            int maxMp = PlayerData.instance.GetInt(nameof(PlayerData.instance.MPReserveMax)) + PlayerData.instance.GetInt(nameof(PlayerData.instance.maxMP));
            if (maxMp / spellCost > cap && UnityEngine.Random.Range(0, 10) < 3)
                HeroController.instance.AddToMaxMPReserve(-1);
            else
                HeroController.instance.AddToMaxHealth(-1);
        }
        else
            HeroController.instance.AddToMaxMPReserve(-1);

        // To force the UI to update to amount of masks.
        if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
            GameCameras.instance.hudCanvas.gameObject.SetActive(true);
        else
        {
            GameCameras.instance.hudCanvas.gameObject.SetActive(false);
            GameCameras.instance.hudCanvas.gameObject.SetActive(true);
        }
    }

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 8));
}