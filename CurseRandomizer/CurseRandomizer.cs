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
    }

    public GlobalSaveData OnSaveGlobal() => new() { Settings = Settings };

    public LocalSaveData OnSaveLocal()
    {
        LocalSaveData saveData = new()
        {
            CurseData = new(),
            BronzeAccess = ModManager.CanAccessBronze,
            SilverAccess = ModManager.CanAccessSilver,
            GoldAccess = ModManager.CanAccessGold,
            StartGeo = ModManager.StartGeo,
            Wallets = ModManager.WalletAmount,
            SoulVessels = ModManager.SoulVessel,
            DreamNailFragments = ModManager.DreamUpgrade
        };
        foreach (Curse curse in CurseManager.GetCurses())
        {
            object data = curse.ParseData();
            if (data != null)
                saveData.CurseData.Add(curse.Name, data);
        }
	    return saveData;
    }

    #endregion
}
