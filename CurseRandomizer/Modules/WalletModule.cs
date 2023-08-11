using CurseRandomizer.Curses;
using CurseRandomizer.Enums;
using ItemChanger;
using ItemChanger.Modules;
using KorzUtils.Helper;
using Modding;
using System.Collections.Generic;

namespace CurseRandomizer.Modules;

public class WalletModule : Module
{
    #region Properties

    public int WalletAmount { get; set; }

    public int[] Capacities { get; set; }

    #endregion

    #region Eventhandler

    private void SetMaterialColor_OnEnter(On.HutongGames.PlayMaker.Actions.SetMaterialColor.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetMaterialColor self)
    {
        orig(self);
        if (self.IsCorrectContext("Fader", "Add Text", "Down") && self.Fsm.GameObject.transform.parent?.name == "Geo Counter")
        {
            if (ItemChangerMod.Modules.Get<CurseModule>() is CurseModule module && ((List<AffectedVisual>)CurseManager.GetCurse<UnknownCurse>().Data.AdditionalData).Contains(AffectedVisual.Geo))
                HeroController.instance.geoCounter.geoTextMesh.text = "???";
            else if (WalletAmount != Capacities.Length)
                if (PDHelper.Geo >= Capacities[WalletAmount])
                    HeroController.instance.geoCounter.geoTextMesh.text = "<color=#68ff57>" + HeroController.instance.geoCounter.geoTextMesh.text + "</color>";
        }
    }

    private void AdjustGeoColor(On.GeoCounter.orig_NewSceneRefresh orig, GeoCounter self)
    {
        orig(self);
        if (WalletAmount != Capacities.Length)
            if (PDHelper.Geo >= Capacities[WalletAmount])
                self.geoTextMesh.text = "<color=#68ff57>" + self.geoTextMesh.text + "</color>";
    }

    /// <summary>
    /// For most circumstances, this will handle the geo.
    /// </summary>
    private void CapGeoByWallet(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }

    /// <summary>
    /// Caps silent geo added.
    /// </summary>
    private void CapGeoByWallet(On.HeroController.orig_AddGeoToCounter orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }

    /// <summary>
    /// Caps counter from geo.
    /// </summary>
    private void CapGeoByWallet(On.HeroController.orig_AddGeoQuietly orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }

    private string AddWalletDescription(string key, string sheetTitle, string orig)
    {
        if (key == "INV_DESC_GEO")
        {
            int cap = Capacities.Length == WalletAmount
                ? 999999
                : Capacities[WalletAmount];
            orig += "\r\nWith your current wallet, you can hold up to " + cap + " geo.";
        }
        return orig;
    }

    private int DetermineGeoAmount(int amount)
    {
        if (Capacities.Length != WalletAmount)
            if (PDHelper.Geo + amount > Capacities[WalletAmount])
                amount = Capacities[WalletAmount] - PDHelper.Geo;
        return amount;
    }

    private int ModHooks_GetPlayerIntHook(string name, int orig) => name == "wallets" ? WalletAmount : orig;

    #endregion

    #region Methods

    public override void Initialize()
    {
        On.HeroController.AddGeo += CapGeoByWallet;
        On.HeroController.AddGeoQuietly += CapGeoByWallet;
        On.HeroController.AddGeoToCounter += CapGeoByWallet;
        On.GeoCounter.NewSceneRefresh += AdjustGeoColor;
        On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter += SetMaterialColor_OnEnter;
        ModHooks.LanguageGetHook += AddWalletDescription;
        ModHooks.GetPlayerIntHook += ModHooks_GetPlayerIntHook;
    }

    public override void Unload()
    {
        On.HeroController.AddGeo -= CapGeoByWallet;
        On.HeroController.AddGeoQuietly -= CapGeoByWallet;
        On.HeroController.AddGeoToCounter -= CapGeoByWallet;
        On.GeoCounter.NewSceneRefresh -= AdjustGeoColor;
        On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter -= SetMaterialColor_OnEnter;
        ModHooks.LanguageGetHook -= AddWalletDescription;
        ModHooks.GetPlayerIntHook -= ModHooks_GetPlayerIntHook;
    }

    #endregion
}
