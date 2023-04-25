using CurseRandomizer.Curses;
using CurseRandomizer.Enums;
using CurseRandomizer.ItemData;
using CurseRandomizer.Manager;
using CurseRandomizer.Randomizer.Settings;
using CurseRandomizer.SaveManagment;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace CurseRandomizer;

public class CurseRandomizer : Mod, IGlobalSettings<GlobalSaveData>, ILocalSettings<LocalSaveData>, IMenuMod
{
    private RandoSettings _settings;

    public CurseRandomizer()
    {
        Instance = this;
    }

    public static CurseRandomizer Instance { get; set; }

    public override string GetVersion() => /*Since this doesn't work SOMEHOW Assembly.GetExecutingAssembly().GetName().Version.ToString()*/ "4.0.0.0";

    public RandoSettings Settings => _settings ??= new();

    public bool ToggleButtonInsideMenu => false;

    public override void Initialize()
    {
        RandoManager.HookRando();
        CurseManager.Initialize();
    }

    #region Save Data control

    public void OnLoadGlobal(GlobalSaveData randoSettings)
    {
        _settings = randoSettings.Settings;
        TemporaryCurse.Position = randoSettings.CounterPosition;
        MidasCurse.Colorless = randoSettings.ColorBlindHelp;
        TemporaryCurse.Scale = Math.Max(0.1f, randoSettings.TrackerScaling);
        TemporaryCurse.TrackerPosition = randoSettings.TrackerPosition;
        TemporaryCurse.AdjustTracker();
        TemporaryCurse.EasyLift = randoSettings.EasyCurseLift;
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

    public GlobalSaveData OnSaveGlobal() =>
        new()
        {
            Settings = Settings,
            CounterPosition = TemporaryCurse.Position,
            ColorBlindHelp = MidasCurse.Colorless,
            TrackerPosition = TemporaryCurse.TrackerPosition,
            TrackerScaling = TemporaryCurse.Scale,
            EasyCurseLift = TemporaryCurse.EasyLift
        };

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
        List<IMenuMod.MenuEntry> options = new()
        {
            new IMenuMod.MenuEntry("Counter Position", Enum.GetNames(typeof(CurseCounterPosition)), "Determines the formation in which the curse counter appear.",
            index =>
            {
                TemporaryCurse.Position = (CurseCounterPosition)index;
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                    foreach (TemporaryCurse curse in CurseManager.GetCurses().Where(x => x is TemporaryCurse))
                        curse.RepositionTracker();
            },
            () => (int)TemporaryCurse.Position),
            new("Move Counter", new string[] {"Up", "Up"}, "Moves all counters slightly up.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                {
                    TemporaryCurse.TrackerPosition += new UnityEngine.Vector3(0f, 0.1f);
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new("Move Counter", new string[] {"Down", "Down"}, "Moves all counters slightly down.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                {
                    TemporaryCurse.TrackerPosition += new UnityEngine.Vector3(0f, -0.1f);
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new("Move Counter", new string[] {"Left", "Left" }, "Moves all counters slightly left.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                {
                    TemporaryCurse.TrackerPosition += new UnityEngine.Vector3(-0.1f, 0f);
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new("Move Counter", new string[] {"Right", "Right" }, "Moves all counters slightly right.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                {
                    TemporaryCurse.TrackerPosition += new UnityEngine.Vector3(0.1f, 0f);
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new("Counter Size", new string[] {"Up", "Up" }, "Makes all counters slightly larger.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
                {
                    TemporaryCurse.Scale += 0.1f;
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new("Counter Size", new string[] {"Down", "Down" }, "Makes all counters slightly smaller.", clicked =>
            {
                if (GameManager.instance != null && GameManager.instance.IsGameplayScene() && TemporaryCurse.Scale > 0.1f)
                {
                    TemporaryCurse.Scale -= 0.1f;
                    TemporaryCurse.AdjustTracker();
                }
            },
            () => 0),
            new ("Colorless Indicator", new string[]{"Disabled", "Enabled"}, "If enabled, the Midas curse will display a textbox.",
            index => MidasCurse.Colorless = index == 1,
            () => MidasCurse.Colorless ? 1 : 0),
            new ("Easy curse lift", new string[] {"Disabled", "Enabled"}, "If enabled, temporary curses will not fully reset, if recasted.",
            index => TemporaryCurse.EasyLift = index == 1,
            () => TemporaryCurse.EasyLift ? 1 : 0)
        };
        //foreach (Curse curse in CurseManager.GetCurses())
        //    options.Add(new($"Ignore {curse.Name}", new string[] { "False", "True" }, $"If true, {curse.Name} doesn't affect you",
        //        index => curse.Data.Ignored = index == 1,
        //        () => curse.Data.Ignored ? 1 : 0));
        return options;
    }

    public void SyncMenu(bool reset = true)
    {
        MenuScreen screen = ModHooks.BuiltModMenuScreens[this];
        if (screen != null)
        {
            if (reset)
                foreach (Curse curse in CurseManager.GetCurses())
                    curse.Data.Ignored = false;
            foreach (MenuOptionHorizontal option in screen.GetComponentsInChildren<MenuOptionHorizontal>(true))
                option.menuSetting.RefreshValueFromGameSettings();
        }
    }

    #endregion
}
