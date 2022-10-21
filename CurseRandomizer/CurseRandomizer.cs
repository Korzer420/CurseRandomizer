using Modding;

namespace CurseRandomizer;

public class CurseRandomizer : Mod
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
        base.Initialize();
        ModHooks.LanguageGetHook += ModHooks_LanguageGetHook;
    }

    private string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key == "Curse_Randomizer_Fool_1")
            orig = "FOOL";
        return orig;
    }

}
