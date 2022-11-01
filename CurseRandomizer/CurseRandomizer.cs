using CurseRandomizer.Curses;
using CurseRandomizer.SaveManagment;
using Modding;

namespace CurseRandomizer;

public class CurseRandomizer : Mod, IGlobalSettings<GlobalSaveData>/*, ILocalSettings<LocalSaveData>*/
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
    }

    public void OnLoadGlobal(GlobalSaveData randoSettings) => _settings = randoSettings.Settings;

    public void OnLoadLocal(LocalSaveData saveData) => CurseManager.ParseSaveData(saveData?.CurseData);

    public GlobalSaveData OnSaveGlobal() => new() { Settings = Settings };

    public LocalSaveData OnSaveLocal()
    {
        LocalSaveData saveData = new();
        saveData.CurseData = new();
        foreach (Curse curse in CurseManager.GetCurses())
        {
            object data = curse.ParseData();
            if (data != null)
                saveData.CurseData.Add(curse.Name, data);
        }
        return saveData;
    }

    private string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key == "Curse_Randomizer_Fool_1")
            orig = "FOOL";
        return orig;
    }

}
