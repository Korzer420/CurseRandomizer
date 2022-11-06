using CurseRandomizer.Helper;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using Modding;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace CurseRandomizer.Manager;

/// <summary>
/// A manager which modifies all ingame related stuff. (Besides curses)
/// </summary>
internal static class ModManager
{
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

    private static System.Collections.IEnumerator UIManager_ReturnToMainMenu(On.UIManager.orig_ReturnToMainMenu orig, UIManager self)
    {
        if (IsWalletCursed)
        {
            On.HeroController.AddGeo -= CapGeoByWallet;
            On.GeoCounter.NewSceneRefresh -= AdjustGeoColor;
            On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter -= SetMaterialColor_OnEnter;
        }
        if (IsColoCursed)
        {
            On.PlayMakerFSM.OnEnable -= BlockColoAccess;
            ModHooks.LanguageGetHook -= ShowColoPreview;
        }
        if (IsDreamNailCursed)
        {
            On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= PreventDreamBosses;
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= PreventGreyPrinceZote;
            On.PlayMakerFSM.OnEnable -= PreventWhiteDefender;
        }
        if (IsVesselCursed)
        { 
            IL.PlayerData.AddMPCharge -= LimitSoul;
            On.PlayMakerFSM.OnEnable -= AdjustSoulAmount;
            On.HeroController.AddToMaxMPReserve -= HookVesselGain;
        }
        yield return orig(self);
    }

    private static void UIManager_ContinueGame(On.UIManager.orig_ContinueGame orig, UIManager self)
    {
        orig(self);
        Hook();
    }

    private static void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        orig(self, permaDeath, bossRush);
        Hook();
    }

    private static void Hook()
    {
        if (IsWalletCursed)
        {
            On.HeroController.AddGeo += CapGeoByWallet;
            On.GeoCounter.NewSceneRefresh += AdjustGeoColor;
            On.HutongGames.PlayMaker.Actions.SetMaterialColor.OnEnter += SetMaterialColor_OnEnter;
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
        }
        if (IsVesselCursed)
        { 
            IL.PlayerData.AddMPCharge += LimitSoul;
            On.PlayMakerFSM.OnEnable += AdjustSoulAmount;
            On.HeroController.AddToMaxMPReserve += HookVesselGain;
        }
    }

    #region Wallet Handler

    private static void SetMaterialColor_OnEnter(On.HutongGames.PlayMaker.Actions.SetMaterialColor.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetMaterialColor self)
    {
        orig(self);
        if (self.IsCorrectContext("Fader", "Add Text", "Down") && self.Fsm.GameObject.transform.parent?.name == "Geo Counter")
        {
            int playerGeo = PlayerData.instance.GetInt(nameof(PlayerData.instance.geo));
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

    private static void CapGeoByWallet(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        if (WalletAmount != 4)
        {
            int currentAmount = PlayerData.instance.GetInt(nameof(PlayerData.instance.geo));
            int[] walletSizes = new int[] { 200, 500, 1000, 5000 };
            if (currentAmount + amount > walletSizes[WalletAmount])
                amount = walletSizes[WalletAmount] - currentAmount;
        }
        orig(self, amount);
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
            self.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 0 ? 0.0513f : 0.0342f;
        orig(self);
    }

    private static void LimitSoul(ILContext il)
    {
        CurseRandomizer.Instance.Log("Modify MP charge");
        try
        {
            ILCursor cursor = new(il);
            cursor.Goto(0);

            if (cursor.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<PlayerData>("GetBool")))
            {
                cursor.EmitDelegate<Func<bool, bool>>(x => x && SoulVessel == 1);
                if (cursor.TryGotoNext(MoveType.After,
                    x => x.MatchLdstr("soulLimited"),
                    x => x.MatchCallvirt<PlayerData>("GetBool")))
                {
                    cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel < 2);
                    if (cursor.TryGotoNext(MoveType.After,
                        x => x.MatchCall(typeof(BossSequenceController).FullName, "get_BoundSoul")))
                        cursor.EmitDelegate<Func<bool, bool>>(x => x || SoulVessel == 0);
                    else
                        CurseRandomizer.Instance.Log("Couldn't find getBoundSoul on BossSequence Controller");
                }
                else
                    CurseRandomizer.Instance.Log("Couldn't find second GetBool");
            }
            else
                CurseRandomizer.Instance.Log("Couldn't find first GetBool");
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.Log(exception.StackTrace);
        }
    }

    private static void HookVesselGain(On.HeroController.orig_AddToMaxMPReserve orig, HeroController self, int amount)
    {
        if (SoulVessel < 2)
        {
            SoulVessel++;
            GameManager.instance.soulVessel_fsm.FsmVariables.FindFsmFloat("Liquid Y Per MP").Value = SoulVessel == 1 ? 0.0342f : 0.0171f;
        }
        else
            orig(self, amount);
    }

    #endregion
}
