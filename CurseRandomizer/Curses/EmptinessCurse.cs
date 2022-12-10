using CurseRandomizer.Helper;
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
        return PlayerData.instance.GetInt(nameof(PlayerData.instance.maxHealthBase)) > cap;
    }

    public override void ApplyCurse() 
    { 
        HeroController.instance.AddToMaxHealth(-1);
        if (!PlayerData.instance.GetBool(nameof(PlayerData.instance.equippedCharm_27)))
        {
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

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 8));
}