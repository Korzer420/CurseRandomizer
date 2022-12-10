using CurseRandomizer.Enums;
using CurseRandomizer.Helper;
using CurseRandomizer.ItemData;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Placements;
using Modding;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static RandomizerMod.Settings.MiscSettings;
using static CurseRandomizer.ItemData.WalletItem;
using RandomizerMod.RC;
using ItemChanger.Tags;

namespace CurseRandomizer.Manager;

/// <summary>
/// A manager which modifies all ingame related stuff. (Besides curses)
/// </summary>
internal static class ModManager
{
    #region Members

    private static List<AbstractPlacement> _placementsToAdd = new();

    #endregion

    static ModManager()
    {
        On.UIManager.StartNewGame += UIManager_StartNewGame;
        On.UIManager.ContinueGame += UIManager_ContinueGame;
        On.UIManager.ReturnToMainMenu += UIManager_ReturnToMainMenu;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the current wallet size.
    /// </summary>
    public static int WalletAmount { get; set; } = 4;

    /// <summary>
    /// Gets or sets the start geo given by the randomizer. This is used for the wallet items.
    /// </summary>
    public static int StartGeo { get; set; }

    public static bool CanAccessBronze { get; set; } = true;

    public static bool CanAccessSilver { get; set; } = true;

    public static bool CanAccessGold { get; set; } = true;

    public static int DreamUpgrade { get; set; } = 2;

    public static int SoulVessel { get; set; } = 2;

    public static bool IsWalletCursed { get; set; }

    public static bool IsDreamNailCursed { get; set; }

    public static bool IsVesselCursed { get; set; }

    public static bool IsColoCursed { get; set; }

    #endregion

    private static IEnumerator UIManager_ReturnToMainMenu(On.UIManager.orig_ReturnToMainMenu orig, UIManager self)
    {
        if (RandomizerMod.RandomizerMod.IsRandoSave)
            if (IsWalletCursed)
            {
                On.HeroController.AddGeo -= CapGeoByWallet;
                On.HeroController.AddGeoQuietly -= CapGeoByWallet;
                On.HeroController.AddGeoToCounter -= CapGeoByWallet;
                On.GeoCounter.NewSceneRefresh -= AdjustGeoColor;
                On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter -= SetMaterialColor_OnEnter;
                ModHooks.LanguageGetHook -= AddWalletDescription;
            }
        if (IsColoCursed)
        {
            On.PlayMakerFSM.OnEnable -= BlockColoAccess;
            ModHooks.LanguageGetHook -= ShowColoPreview;
            ModHooks.SetPlayerBoolHook -= ActivatePasses;
        }
        if (IsDreamNailCursed)
        {
            On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= PreventDreamBosses;
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= PreventGreyPrinceZote;
            On.PlayMakerFSM.OnEnable -= PreventWhiteDefender;
            ModHooks.LanguageGetHook -= ShowDreamNailDescription;
        }
        if (IsVesselCursed)
        {
            IL.PlayerData.AddMPCharge -= LimitSoul;
            On.PlayMakerFSM.OnEnable -= AdjustSoulAmount;
            On.HeroController.AddToMaxMPReserve -= HookVesselGain;
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= FixVesselEyes;
        }

        foreach (Curse curse in CurseManager.GetCurses())
            curse.Unhook();
        yield return orig(self);
    }

    private static void UIManager_ContinueGame(On.UIManager.orig_ContinueGame orig, UIManager self)
    {
        orig(self);
        if (RandomizerMod.RandomizerMod.IsRandoSave)
            Hook();
    }

    private static void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        orig(self, permaDeath, bossRush);
        if (!RandomizerMod.RandomizerMod.IsRandoSave)
            return;
        foreach (Curse curse in CurseManager.GetCurses())
            curse.ResetData();
        Hook();
        // Fix for the shop placement wrap.
        if (IsWalletCursed)
        {
            AddShopDefaults();
            Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
            if (placements.ContainsKey(Sly_Key_Cheap))
                (placements[Sly_Key_Cheap] as ShopPlacement).requiredPlayerDataBool = nameof(PlayerData.instance.gaveSlykey);
            if (placements.ContainsKey(Sly_Key_Medium))
                (placements[Sly_Key_Medium] as ShopPlacement).requiredPlayerDataBool = nameof(PlayerData.instance.gaveSlykey);
            if (placements.ContainsKey(Sly_Key_Expensive))
                (placements[Sly_Key_Expensive] as ShopPlacement).requiredPlayerDataBool = nameof(PlayerData.instance.gaveSlykey);
            if (placements.ContainsKey(Sly_Key_Extreme_Valuable))
                (placements[Sly_Key_Extreme_Valuable] as ShopPlacement).requiredPlayerDataBool = nameof(PlayerData.instance.gaveSlykey);
        }
    }

    private static void Hook()
    {
        if (IsWalletCursed)
        {
            On.HeroController.AddGeo += CapGeoByWallet;
            On.HeroController.AddGeoQuietly += CapGeoByWallet;
            On.HeroController.AddGeoToCounter += CapGeoByWallet;
            On.GeoCounter.NewSceneRefresh += AdjustGeoColor;
            On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter += SetMaterialColor_OnEnter;
            ModHooks.LanguageGetHook += AddWalletDescription;

        }
        if (IsColoCursed)
        {
            On.PlayMakerFSM.OnEnable += BlockColoAccess;
            ModHooks.LanguageGetHook += ShowColoPreview;
            ModHooks.SetPlayerBoolHook += ActivatePasses;
        }
        if (IsDreamNailCursed)
        {
            On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PreventDreamBosses;
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += PreventGreyPrinceZote;
            On.PlayMakerFSM.OnEnable += PreventWhiteDefender;
            ModHooks.LanguageGetHook += ShowDreamNailDescription;
        }
        if (IsVesselCursed)
        {
            IL.PlayerData.AddMPCharge += LimitSoul;
            On.PlayMakerFSM.OnEnable += AdjustSoulAmount;
            On.HeroController.AddToMaxMPReserve += HookVesselGain;
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += FixVesselEyes;
        }
        foreach (Curse curse in CurseManager.GetCurses())
            curse.ApplyHooks();
        CurseManager.Handler.StartCoroutine(WaitForHC());
    }

    private static string ShowDreamNailDescription(string key, string sheetTitle, string orig)
    {
        if (key == "INV_DESC_DREAMNAIL_A" || key == "INV_DESC_DREAMNAIL_B")
        {
            orig += "\r\n";
            if (DreamUpgrade == 0)
                orig += "It's not strong enough yet to fight the strong warriors from the past.";
            else if (DreamUpgrade == 1)
                orig += "With the fight fragment you can challenge the restless spirits of the strong warriors. " +
                    "But it isn't strong enough yet to pierce through the stronger remaining dreams.";
            else
                orig += "With both fragments assembled no restless spirit can resist the challenge.";
        }
        return orig;
    }

    #region Wallet Handler

    private static void SetMaterialColor_OnEnter(On.HutongGames.PlayMaker.Actions.SetMaterialColor.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetMaterialColor self)
    {
        orig(self);
        if (self.IsCorrectContext("Fader", "Add Text", "Down") && self.Fsm.GameObject.transform.parent?.name == "Geo Counter")
        {
            int playerGeo = PlayerData.instance.GetInt(nameof(PlayerData.instance.geo));
            if (((List<AffectedVisual>)CurseManager.GetCurseByType(CurseType.Unknown).Data.Data).Contains(AffectedVisual.Geo))
                HeroController.instance.geoCounter.geoTextMesh.text = "???";
            else
                switch (WalletAmount)
                {
                    case 0 when playerGeo == 200:
                    case 1 when playerGeo == 500:
                    case 2 when playerGeo == 1000:
                    case 3 when playerGeo == 5000:
                    case 4 when playerGeo == 9999999:
                        HeroController.instance.geoCounter.geoTextMesh.text = "<color=#68ff57>" + HeroController.instance.geoCounter.geoTextMesh.text + "</color>";
                        break;
                }
        }
    }

    private static void AdjustGeoColor(On.GeoCounter.orig_NewSceneRefresh orig, GeoCounter self)
    {
        orig(self);
        int playerGeo = PlayerData.instance.GetInt(nameof(PlayerData.instance.geo));
        switch (WalletAmount)
        {
            case 0 when playerGeo == 200:
            case 1 when playerGeo == 500:
            case 2 when playerGeo == 1000:
            case 3 when playerGeo == 5000:
            case 4 when playerGeo == 9999999:
                self.geoTextMesh.text = "<color=#68ff57>" + self.geoTextMesh.text + "</color>";
                break;
        }
    }

    /// <summary>
    /// For most circumstances, this will handle the geo.
    /// </summary>
    private static void CapGeoByWallet(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }

    /// <summary>
    /// Caps silent geo added.
    /// </summary>
    private static void CapGeoByWallet(On.HeroController.orig_AddGeoToCounter orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }

    /// <summary>
    /// Caps counter from geo.
    /// </summary>
    private static void CapGeoByWallet(On.HeroController.orig_AddGeoQuietly orig, HeroController self, int amount)
    {
        amount = DetermineGeoAmount(amount);
        orig(self, amount);
    }


    private static string AddWalletDescription(string key, string sheetTitle, string orig)
    {
        if (key == "INV_DESC_GEO" && WalletAmount != 4)
        {
            int cap = WalletAmount switch
            {
                1 => 500,
                2 => 1000,
                3 => 5000,
                _ => 200
            };
            orig += "\r\nWith your current wallet, you can hold up to " + cap + " geo in it.";
        }
        return orig;
    }

    private static int DetermineGeoAmount(int amount)
    {
        if (WalletAmount != 4)
        {
            int currentAmount = PlayerData.instance.GetInt(nameof(PlayerData.instance.geo));
            int[] walletSizes = new int[] { 200, 500, 1000, 5000 };
            if (currentAmount + amount > walletSizes[WalletAmount])
                amount = walletSizes[WalletAmount] - currentAmount;
        }
        return amount;
    }

    #endregion

    #region Colo Handler

    private static void BlockColoAccess(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        try
        {
            if (self.FsmName == "Conversation Control" && self.gameObject.name.EndsWith("Trial Board"))
            {
                PlayMakerFSM referenceBoard = GameObject.Find("Gold Trial Board").LocateMyFSM("Conversation Control");

                string board = self.gameObject.name.StartsWith("Bronze")
                    ? "Bronze"
                    : (self.gameObject.name.StartsWith("Silver")
                    ? "Silver"
                    : "Gold");
                CallMethodProper actionReference = referenceBoard.GetState("Unpaid 2").GetFirstActionOfType<CallMethodProper>();
                self.AddState(new HutongGames.PlayMaker.FsmState(self.Fsm)
                {
                    Name = "Has Ticket?",
                    Actions = new HutongGames.PlayMaker.FsmStateAction[]
                    {
                        new Lambda(() =>
                        {
                            if (board == "Bronze")
                                self.SendEvent(CanAccessBronze ? "OPEN" : "FINISHED");
                            else if (board == "Silver")
                                self.SendEvent(CanAccessSilver ? "OPEN" : "FINISHED");
                            else
                                self.SendEvent(CanAccessGold ? "OPEN" : "FINISHED");
                        })
                    }
                });
                self.AddState(new HutongGames.PlayMaker.FsmState(self.Fsm)
                {
                    Name = "Box Up",
                    Actions = referenceBoard.GetState("Box Up 2").Actions
                });
                self.AddState(new HutongGames.PlayMaker.FsmState(self.Fsm)
                {
                    Name = "Unworthy",
                    Actions = new HutongGames.PlayMaker.FsmStateAction[]
                    {
                        new Lambda(() =>
                        {
                            actionReference.gameObject.GameObject.Value.GetComponent<DialogueBox>().StartConversation($"Unworthy ({board})", "Minor_NPC");
                        })
                    }
                });


                // Set up transition
                self.GetState("State Check").AdjustTransition("OPEN", "Has Ticket?");
                self.GetState("Has Ticket?").AddTransition("OPEN", "Box Up YN");
                self.GetState("Has Ticket?").AddTransition("FINISHED", "Box Up");
                self.GetState("Box Up").AddTransition("FINISHED", "Unworthy");
                self.GetState("Unworthy").AddTransition("CONVO_FINISH", "Anim End");
            }
        }
        catch (System.Exception exception)
        {
            CurseRandomizer.Instance.LogError("Couldn't modify colo trial boards: " + exception.StackTrace);
        }
        orig(self);
    }

    private static string ShowColoPreview(string key, string sheetTitle, string orig)
    {
        if (key == "Unworthy (Bronze)")
        {
            if (ItemChanger.Internal.Ref.Settings.Placements.ContainsKey(LocationNames.Charm_Notch_Colosseum)
                && ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Charm_Notch_Colosseum].Items.Count > 0)
                orig = "You are not worthy to enter the arena yet. If you want " +
                    ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Charm_Notch_Colosseum].Items[0].GetPreviewName(ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Charm_Notch_Colosseum])
                    + " you have to be a little... richer. (Translation: Only fools with a ticket have access, so get lost.)";
            else
                orig = "You are not worthy to enter the arena yet.";
        }
        else if (key == "Unworthy (Silver)")
        {
            if (ItemChanger.Internal.Ref.Settings.Placements.ContainsKey(LocationNames.Pale_Ore_Colosseum)
                && ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Pale_Ore_Colosseum].Items.Count > 0)
                orig = "You are not worthy to enter the arena yet. If you want " +
                    ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Pale_Ore_Colosseum].Items[0].GetPreviewName(ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Pale_Ore_Colosseum])
                    + " you have to be a little... richer. (Translation: Only fools with a ticket have access, so get lost.)";
            else
                orig = "You are not worthy to enter the arena yet.";
        }
        else if (key == "Unworthy (Gold)")
            orig = "You are not worthy to enter the arena yet. Come back when you are a little... richer. (Translation: Only fools with a ticket have access, so get lost.)";

        return orig;
    }

    private static bool ActivatePasses(string name, bool orig)
    {
        if (name == "CanAccessBronze")
            CanAccessBronze = orig;
        else if (name == "CanAccessSilver")
            CanAccessSilver = orig;
        else if (name == "CanAccessGold")
            CanAccessGold = orig;
        return orig;
    }

    #endregion

    #region Dream Nail Handler

    private static void PreventDreamBosses(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, PlayerDataBoolTest self)
    {
        if ((self.IsCorrectContext("Appear", "Ghost Warrior NPC", "Init")
            || self.IsCorrectContext("Conversation Control", "Ghost Warrior NPC", "Init")) && DreamUpgrade == 0)
            self.isTrue = self.isFalse;
        else if (DreamUpgrade < 2 && self.boolName.Value == "hasDreamNail" && self.IsCorrectContext("Control", null, "Check") &&
            (self.Fsm.GameObject.name == "FK Corpse" || self.Fsm.GameObject.name == "IK Remains"
            || self.Fsm.GameObject.name == "Mage Lord Remains"))
            self.isTrue = self.isFalse;
        orig(self);
    }

    private static void PreventWhiteDefender(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (DreamUpgrade < 2 && self.FsmName == "Control" && self.gameObject.name == "Dream Enter" && GameManager.instance?.sceneName == "Waterways_15")
            self.GetState("Idle").ClearTransitions();
        orig(self);
    }

    private static void PreventGreyPrinceZote(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, IntCompare self)
    {
        if (DreamUpgrade < 2 && self.IsCorrectContext("FSM", "Dream Enter", "Check") && self.Fsm.Variables.FindFsmString("PD Int Name")?.Value == "greyPrinceDefeats")
        {
            self.equal = self.greaterThan;
            self.lessThan = self.greaterThan;
        }
        orig(self);
    }

    #endregion

    #region Vessel Handler

    private static void AdjustSoulAmount(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "Soul Orb Control" && self.gameObject.name == "Soul Orb" && SoulVessel < 2)
            self.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 0 ? 0.0513f : 0.02736f;
        orig(self);
    }

    private static void LimitSoul(ILContext il)
    {
        try
        {
            ILCursor cursor = new(il);
            cursor.Goto(0);

            if (cursor.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<PlayerData>("GetBool")))
            {
                cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel == 1);
                if (cursor.TryGotoNext(MoveType.After,
                    x => x.MatchLdstr("soulLimited"),
                    x => x.MatchCallvirt<PlayerData>("GetBool")))
                {
                    cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel < 2);
                    if (cursor.TryGotoNext(MoveType.After,
                        x => x.MatchCall(typeof(BossSequenceController).FullName, "get_BoundSoul")))
                        cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel == 0);
                }
            }
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.Log(exception.StackTrace);
        }
    }

    private static void HookVesselGain(On.HeroController.orig_AddToMaxMPReserve orig, HeroController self, int amount)
    {
        try
        {
            if (SoulVessel < 2)
            {
                SoulVessel++;
                PlayMakerFSM fsm = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas/Soul Orb").gameObject.LocateMyFSM("Soul Orb Control");
                if (fsm != null)
                {
                    fsm.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 1 ? 0.02736f : 0.0171f;
                    PlayerData.instance.SetInt(nameof(PlayerData.instance.maxMP), SoulVessel == 1 ? 66 : 99);
                }
                else
                {
                    CurseRandomizer.Instance.LogError("Couldn't update soul vessel. Fsm not found");
                    orig(self, amount);
                }
            }
            else
                orig(self, amount);
        }
        catch (Exception)
        {
            CurseRandomizer.Instance.LogError("Couldn't update soul vessel.");
            orig(self, amount);
        }
    }

    private static void FixVesselEyes(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, IntCompare self)
    {
        if (self.IsCorrectContext("Soul Orb Control", "Soul Orb", "Check Eyes"))
            self.integer2.Value = SoulVessel == 0 ? 17 : (SoulVessel == 1 ? 33 : 55);
        orig(self);
    }

    #endregion

    private static void AddShopDefaults()
    {
        _placementsToAdd.Clear();

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Charms)
        {
            // Salubra charms.
            AddDefaultItemToShop(Salubra_Cheap, ItemNames.Steady_Body, 150);
            AddDefaultItemToShop(Salubra_Medium, ItemNames.Lifeblood_Heart, 250);
            AddDefaultItemToShop(Salubra_Medium, ItemNames.Longnail, 300);
            AddDefaultItemToShop(Salubra_Medium, ItemNames.Shaman_Stone, 220);
            AddDefaultItemToShop(Salubra_Expensive, ItemNames.Quick_Focus, 800);
            AddDefaultItemToShop(Salubra_Expensive, ItemNames.Salubras_Blessing, 800, 40);

            // Iselda charms
            AddDefaultItemToShop(Iselda_Medium, ItemNames.Wayward_Compass, 220);

            // Sly charms.
            AddDefaultItemToShop(Sly_Cheap, ItemNames.Stalwart_Shell, 200);
            AddDefaultItemToShop(Sly_Medium, ItemNames.Gathering_Swarm, 300);
            AddDefaultItemToShop(Sly_Key_Medium, ItemNames.Heavy_Blow, 369);
            AddDefaultItemToShop(Sly_Key_Medium, ItemNames.Sprintmaster, 420);

            // Leg eater
            AddDefaultItemToShop(Leg_Eater_Medium, ItemNames.Fragile_Heart, 350);
            AddDefaultItemToShop(Leg_Eater_Medium, ItemNames.Fragile_Greed, 250);
            AddDefaultItemToShop(Leg_Eater_Expensive, ItemNames.Fragile_Strength, 600);

            // Repairable charms
            Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
            AbstractPlacement currentPlacement;
            AbstractItem currentItem = Finder.GetItem(ItemNames.Fragile_Heart_Repair);
            currentItem.tags ??= new();
            currentItem.AddTag(new CostTag() { Cost = new GeoCost(200)});
            currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_23) });
            if (!placements.ContainsKey(Leg_Eater_Cheap) && !_placementsToAdd.Select(x => x.Name).Contains(Leg_Eater_Cheap))
            {
                currentPlacement = Finder.GetLocation(Leg_Eater_Cheap).Wrap();
                _placementsToAdd.Add(currentPlacement);
            }
            else if (!placements.ContainsKey(Leg_Eater_Cheap))
                currentPlacement = _placementsToAdd.First(x => x.Name == Leg_Eater_Cheap);
            else
                currentPlacement = placements[Leg_Eater_Cheap];
            currentPlacement.Add(currentItem);

            // Greed
            currentItem = Finder.GetItem(ItemNames.Fragile_Greed_Repair);
            currentItem.tags ??= new();
            currentItem.AddTag(new CostTag() { Cost = new GeoCost(150) });
            currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_24) });
            currentPlacement.Add(currentItem);

            // Strength
            currentItem = Finder.GetItem(ItemNames.Fragile_Strength_Repair);
            currentItem.tags ??= new();
            currentItem.AddTag(new CostTag() { Cost = new GeoCost(350) });
            currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_25) });
            if (!placements.ContainsKey(Leg_Eater_Medium) && !_placementsToAdd.Select(x => x.Name).Contains(Leg_Eater_Medium))
            {
                currentPlacement = Finder.GetLocation(Leg_Eater_Medium).Wrap();
                _placementsToAdd.Add(currentPlacement);
            }
            else if (!placements.ContainsKey(Leg_Eater_Medium))
                currentPlacement = _placementsToAdd.First(x => x.Name == Leg_Eater_Medium);
            else
                currentPlacement = placements[Leg_Eater_Medium];
            currentPlacement.Add(currentItem);
        }

        if (RandomizerMod.RandomizerMod.RS.GenerationSettings.MiscSettings.SalubraNotches == SalubraNotchesSetting.Vanilla)
        {
            // Salubra charms.
            AddDefaultItemToShop(Salubra_Cheap, ItemNames.Charm_Notch, 150, 5);
            AddDefaultItemToShop(Salubra_Medium, ItemNames.Charm_Notch, 500, 10);
            AddDefaultItemToShop(Salubra_Expensive, ItemNames.Charm_Notch, 900, 18);
            AddDefaultItemToShop(Salubra_Extreme_Valuable, ItemNames.Charm_Notch, 1400, 25);
        }

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Keys)
        {
            AddDefaultItemToShop(Sly_Expensive, ItemNames.Simple_Key, 950);
            AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Elegant_Key, 800);
        }

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.MaskShards)
        {
            AddDefaultItemToShop(Sly_Cheap, ItemNames.Mask_Shard, 150);
            AddDefaultItemToShop(Sly_Medium, ItemNames.Mask_Shard, 500);
            AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Mask_Shard, 800);
            AddDefaultItemToShop(Sly_Key_Extreme_Valuable, ItemNames.Mask_Shard, 1500);
        }

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.VesselFragments)
        {
            AddDefaultItemToShop(Sly_Expensive, ItemNames.Vessel_Fragment, 550);
            AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Vessel_Fragment, 900);
        }

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Skills)
            AddDefaultItemToShop(Sly_Extreme_Valuable, ItemNames.Lumafly_Lantern, 1800);

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.RancidEggs)
            AddDefaultItemToShop(Sly_Cheap, ItemNames.Rancid_Egg, 69);

        if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Maps)
            AddDefaultItemToShop(Iselda_Cheap, ItemNames.Quill, 120);

        if (_placementsToAdd.Any())
            ItemChangerMod.AddPlacements(_placementsToAdd);
    }

    private static void AddDefaultItemToShop(string locationName, string itemName, int geoCost, int charmCost = -1)
    {
        Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
        AbstractPlacement currentPlacement;
        AbstractItem currentItem = Finder.GetItem(itemName);
        currentItem.tags ??= new();
        if (charmCost == -1)
            currentItem.AddTag(new CostTag() { Cost = new GeoCost(geoCost) });

        else
            currentItem.AddTag(new CostTag()
            {
                Cost = new MultiCost(
                new GeoCost(geoCost),
                new PDIntCost(charmCost, nameof(PlayerData.instance.charmsOwned), $"You need to have {charmCost} charms to buy this."))
            });

        if (!placements.ContainsKey(locationName) && !_placementsToAdd.Select(x => x.Name).Contains(locationName))
        {
            currentPlacement = Finder.GetLocation(locationName).Wrap();
            _placementsToAdd.Add(currentPlacement);
        }
        else if (!placements.ContainsKey(locationName))
            currentPlacement = _placementsToAdd.First(x => x.Name == locationName);
        else
            currentPlacement = placements[locationName];
        currentPlacement.Add(currentItem);
    }

    private static IEnumerator WaitForHC()
    {
        yield return new WaitUntil(() => HeroController.instance != null);
        CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
        if (module.CurseQueue.Any())
            CurseManager.Handler.StartCoroutine(module.WaitForControl());
    }
}
