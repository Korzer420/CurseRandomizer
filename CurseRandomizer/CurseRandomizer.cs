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
    }
}
