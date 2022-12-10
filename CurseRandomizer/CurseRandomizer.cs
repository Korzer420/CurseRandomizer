using CurseRandomizer.Manager;
using CurseRandomizer.Randomizer.Settings;
using CurseRandomizer.SaveManagment;
using Modding;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer;

public class CurseRandomizer : Mod, IGlobalSettings<GlobalSaveData>, ILocalSettings<LocalSaveData>
{
    private RandoSettings _settings;

    public CurseRandomizer()
    {
        Instance = this;
    }

    public static CurseRandomizer Instance { get; set; }

    public override string GetVersion() => /*Since this doesn't work SOMEHOW Assembly.GetExecutingAssembly().GetName().Version.ToString()*/ "1.0.0.0";

    public RandoSettings Settings => _settings ??= new();

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
        else if (key == "Curse_Randomizer_Remove_Darkness_1")
            orig = "The curse of darkness vanished";
        else if (key == "Curse_Randomizer_Remove_Omen_1")
            orig = "The omen has been vanished";
        else if (key.StartsWith("Curse_Randomizer_Omen_Affect_"))
        {
            key = key.Substring("Curse_Randomizer_Omen_Affect_".Length);
            orig = $"The curse of <color={Curse.TextColor}>{ key.Split(new string[] {"_1"}, System.StringSplitOptions.RemoveEmptyEntries)[0]}</color> was layed upon you!";
        }    
        return orig;
    }

    #region Save Data control

    public void OnLoadGlobal(GlobalSaveData randoSettings) => _settings = randoSettings.Settings;

    public void OnLoadLocal(LocalSaveData saveData) 
    {
        try
        {
            Log("Load local data");
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
            CurseManager.DefaultCurse = CurseManager.GetCurseByName(saveData.DefaultCurse);
        }
        catch (System.Exception exception)
        {
            LogError("In LoadLocal: " + exception.StackTrace);
        }
    }

    public GlobalSaveData OnSaveGlobal() => new() { Settings = Settings };

    public LocalSaveData OnSaveLocal()
    {
        Log("Save local data");
        Dictionary<string, CurseData> curseData = new();
        foreach (Curse curse in CurseManager.GetCurses())
            curseData.Add(curse.Name, curse.Data);
        
        LocalSaveData saveData = new()
        {
            Data = curseData,
            BronzeAccess = ModManager.CanAccessBronze,
            SilverAccess = ModManager.CanAccessSilver,
            GoldAccess = ModManager.CanAccessGold,
            StartGeo = ModManager.StartGeo,
            Wallets = ModManager.WalletAmount,
            SoulVessels = ModManager.SoulVessel,
            DreamNailFragments = ModManager.DreamUpgrade,
            UseCaps = CurseManager.UseCaps,
            DefaultCurse = CurseManager.DefaultCurse == null ? "Pain" :CurseManager.DefaultCurse.Name,
            ColoCursed = ModManager.IsColoCursed,
            VesselCursed = ModManager.IsVesselCursed,
            WalletCursed = ModManager.IsWalletCursed,
            DreamNailCursed = ModManager.IsDreamNailCursed
        };
	    return saveData;
    }

    #endregion
}
