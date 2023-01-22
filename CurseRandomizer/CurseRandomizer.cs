using CurseRandomizer.Curses;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using CurseRandomizer.Manager;
using CurseRandomizer.Randomizer.Settings;
using CurseRandomizer.SaveManagment;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace CurseRandomizer;

public class CurseRandomizer : Mod, IGlobalSettings<GlobalSaveData>, ILocalSettings<LocalSaveData>, IMenuMod
{
    private RandoSettings _settings;

    public CurseRandomizer()
    {
        Instance = this;
    }

    public static CurseRandomizer Instance { get; set; }

    public override string GetVersion() => /*Since this doesn't work SOMEHOW Assembly.GetExecutingAssembly().GetName().Version.ToString()*/ "3.1.0.0";

    public RandoSettings Settings => _settings ??= new();

    public bool ToggleButtonInsideMenu => false;

    public override void Initialize()
    {
        ModHooks.LanguageGetHook += ModHooks_LanguageGetHook;

        RandoManager.HookRando();
        CurseManager.Initialize();
    }

    private string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key == "Curse_Randomizer_Fool_1")
            orig = "FOOL";
        else if (key == "CR_Fool_Notch_1")
            orig = "FOOL (You lost a charm notch)";
        else if (key == "CR_Fool_Relic_1")
            orig = "FOOL (You lost a relic)";
        else if (key == "Curse_Randomizer_Darkness_Vanish_1")
            orig = "The curse of darkness vanished!";
        else if (key == "Curse_Randomizer_Omen_Vanish_1")
            orig = "The omen vanished!";
        else if (key == "Curse_Randomizer_Confusion_Vanish_1")
            orig = "The confusion vanished!";
        else if (key == "Curse_Randomizer_Maze_Vanish_1")
            orig = "The curse of maze vanished";
        else if (key == "Curse_Randomizer_Regret_Vanish_1")
            orig = "You have no regrets left.";
        else if (key == "Curse_Randomizer_Despair_Vanish_1")
            orig = "There is no hope... but no time for despair either.";
        else if (key == "Curse_Randomizer_Maze_Teleported_1")
            orig = "???";
        else if (key.StartsWith("Curse_Randomizer_Omen_Casted_"))
        {
            key = key.Substring("Curse_Randomizer_Omen_Casted_".Length);
            string curseName = UnknownCurse.AreCursesHidden ? "???" : key.Split(new string[] { "_1" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            orig = $"The curse of <color={Curse.TextColor}>{curseName}</color> was layed upon you!";
        }
        else if (key.StartsWith("Curse_Randomizer_Regret_Casted_"))
        {
            key = key.Substring("Curse_Randomizer_Regret_Casted_".Length);
            string curseName = UnknownCurse.AreCursesHidden ? "???" : key.Split(new string[] { "_1" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            orig = $"The sins of <color={Curse.TextColor}>{curseName}</color> are crawling on your back!";
        }
        else if (key.StartsWith("Curse_Randomizer_Despair_Casted_"))
        {
            key = key.Substring("Curse_Randomizer_Despair_Casted_".Length);
            string curseName = UnknownCurse.AreCursesHidden ? "???" : key.Split(new string[] { "_1" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            orig = $"Your hopelessness formed <color={Curse.TextColor}>{curseName}</color>!";
        }
        return orig;
    }

    #region Save Data control

    public void OnLoadGlobal(GlobalSaveData randoSettings)
    {
        _settings = randoSettings.Settings;
        TemporaryCurse.Position = randoSettings.CounterPosition;
    }

    public void OnLoadLocal(LocalSaveData saveData)
    {
        try
        {
            LogDebug("Load local data");
            if (saveData == null)
                return;
            CurseManager.ParseSaveData(saveData.Data);
            ModManager.StartGeo = saveData.StartGeo;
            ModManager.CanAccessBronze = saveData.BronzeAccess;
            ModManager.CanAccessSilver = saveData.SilverAccess;
            ModManager.CanAccessGold = saveData.GoldAccess;
            ModManager.WalletAmount = saveData.Wallets;
            ModManager.SoulVessel = saveData.SoulVessels;
            ModManager.DreamUpgrade = saveData.DreamNailFragments;
            CurseManager.UseCaps = saveData.UseCaps;
            ModManager.IsWalletCursed = saveData.WalletCursed;
            ModManager.IsVesselCursed = saveData.VesselCursed;
            ModManager.IsColoCursed = saveData.ColoCursed;
            ModManager.IsDreamNailCursed = saveData.DreamNailCursed;
            ModManager.UseCurses = saveData.UseCurses;
            OmenCurse.OmenMode = saveData.OmenMode;
            CurseManager.DefaultCurse = CurseManager.GetCurseByName(saveData.DefaultCurse);
        }
        catch (System.Exception exception)
        {
            LogError("In LoadLocal: " + exception.StackTrace);
        }
    }

    public GlobalSaveData OnSaveGlobal() => new() { Settings = Settings, CounterPosition = TemporaryCurse.Position };

    public LocalSaveData OnSaveLocal()
    {
        LogDebug("Save local data");
        Dictionary<string, CurseData> curseData = new();
        foreach (Curse curse in CurseManager.GetCurses())
            curseData.Add(curse.Name, curse.Data);

        LocalSaveData saveData = new()
        {
            Data = curseData,
            UseCurses = ModManager.UseCurses,
            BronzeAccess = ModManager.CanAccessBronze,
            SilverAccess = ModManager.CanAccessSilver,
            GoldAccess = ModManager.CanAccessGold,
            StartGeo = ModManager.StartGeo,
            Wallets = ModManager.WalletAmount,
            SoulVessels = ModManager.SoulVessel,
            DreamNailFragments = ModManager.DreamUpgrade,
            UseCaps = CurseManager.UseCaps,
            DefaultCurse = CurseManager.DefaultCurse == null ? "Pain" : CurseManager.DefaultCurse.Name,
            ColoCursed = ModManager.IsColoCursed,
            VesselCursed = ModManager.IsVesselCursed,
            WalletCursed = ModManager.IsWalletCursed,
            DreamNailCursed = ModManager.IsDreamNailCursed,
            OmenMode = OmenCurse.OmenMode
        };
        return saveData;
    }

    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return new()
        {
            new IMenuMod.MenuEntry("Counter Position", Enum.GetNames(typeof(CurseCounterPosition)), "Determines the position where the counter appear.",
            index =>
            {
                TemporaryCurse.Position = (CurseCounterPosition)index;
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                    foreach (TemporaryCurse curse in CurseManager.GetCurses().Where(x => x is TemporaryCurse))
                        curse.RepositionTracker();
            },
            () => (int)TemporaryCurse.Position)
        };
    }

    #endregion
}
