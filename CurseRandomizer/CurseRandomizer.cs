using CurseRandomizer.Manager;
using CurseRandomizer.SaveManagment;
using Modding;

namespace CurseRandomizer;

public class CurseRandomizer : Mod, IGlobalSettings<GlobalSaveData>, ILocalSettings<LocalSaveData>
{
    private RandoSettings _settings;

    public CurseRandomizer()
    {
        Instance = this;
    }

    public static CurseRandomizer Instance { get; set; }

    public override string GetVersion() => /*Since this doesn't work SOMEHOW Assembly.GetExecutingAssembly().GetName().Version.ToString()*/ "0.2.0.0";

    public RandoSettings Settings => _settings ??= new();

    public override void Initialize()
    {
        ModHooks.LanguageGetHook += ModHooks_LanguageGetHook;
        RandoManager.HookRando();
        CurseManager.Initialize();
        On.UIManager.StartNewGame += UIManager_StartNewGame;
    }

    private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        foreach (Curse curse in CurseManager.GetCurses())
            curse.ResetData();
        orig(self, permaDeath, bossRush);
    }

    private string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key == "Curse_Randomizer_Fool_1")
            orig = "FOOL";
        return orig;
    }

    #region Save Data control

    public void OnLoadGlobal(GlobalSaveData randoSettings) => _settings = randoSettings.Settings;

    public void OnLoadLocal(LocalSaveData saveData) 
    {
        try
        {
            CurseManager.ParseSaveData(saveData?.CurseData);
            if (saveData == null)
                return;
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
            CurseManager.DefaultCurse = CurseManager.GetCurseByType(saveData.DefaultCurse);
            foreach (Curse curse in CurseManager.GetCurses())
                if (saveData.CurseCaps.ContainsKey(curse.Name))
                    curse.Cap = saveData.CurseCaps[curse.Name];
        }
        catch (System.Exception exception)
        {
            LogError("In LoadLocal: " + exception.StackTrace);
        }
    }

    public GlobalSaveData OnSaveGlobal() => new() { Settings = Settings };

    public LocalSaveData OnSaveLocal()
    {
        LocalSaveData saveData = new()
        {
            CurseData = new(),
            CurseCaps = new(),
            BronzeAccess = ModManager.CanAccessBronze,
            SilverAccess = ModManager.CanAccessSilver,
            GoldAccess = ModManager.CanAccessGold,
            StartGeo = ModManager.StartGeo,
            Wallets = ModManager.WalletAmount,
            SoulVessels = ModManager.SoulVessel,
            DreamNailFragments = ModManager.DreamUpgrade,
            UseCaps = CurseManager.UseCaps,
            DefaultCurse = CurseManager.DefaultCurse == null ? CurseType.Pain :CurseManager.DefaultCurse.Type,
            ColoCursed = ModManager.IsColoCursed,
            VesselCursed = ModManager.IsVesselCursed,
            WalletCursed = ModManager.IsWalletCursed,
            DreamNailCursed = ModManager.IsDreamNailCursed
        };
        foreach (Curse curse in CurseManager.GetCurses())
        {
            object data = curse.ParseData();
            if (data != null)
                saveData.CurseData.Add(curse.Name, data);
            saveData.CurseCaps.Add(curse.Name, curse.Cap);
        }
	    return saveData;
    }

    #endregion
}
