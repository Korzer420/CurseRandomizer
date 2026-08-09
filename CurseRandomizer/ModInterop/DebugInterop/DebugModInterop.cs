using CurseRandomizer.Modules;
using DebugMod;
using ItemChanger;

namespace CurseRandomizer.ModInterop.DebugInterop;

#if DEBUG
public static class DebugModInterop
{
    internal static void Initialize() => DebugMod.DebugMod.AddToKeyBindList(typeof(DebugModInterop));

    #region Commands

    [BindableMethod(name = "Apply Pain", category = "CurseRando")]
    public static void Pain() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Pain");

    [BindableMethod(name = "Apply Greed", category = "CurseRando")]
    public static void Greed() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Greed");

    [BindableMethod(name = "Apply Emptiness", category = "CurseRando")]
    public static void Emptiness() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Emptiness");

    [BindableMethod(name = "Apply Thirst", category = "CurseRando")]
    public static void Thirst() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Thirst");

    [BindableMethod(name = "Apply Weakness", category = "CurseRando")]
    public static void Weakness() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Weakness");

    [BindableMethod(name = "Apply Disorientation", category = "CurseRando")]
    public static void Disorientation() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Disorientation");

    [BindableMethod(name = "Apply Lost", category = "CurseRando")]
    public static void Lost() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Lost");

    [BindableMethod(name = "Apply Normality", category = "CurseRando")]
    public static void Normality() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Normality");

    [BindableMethod(name = "Apply Stupidity", category = "CurseRando")]
    public static void Stupidity() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Stupidity");

    [BindableMethod(name = "Apply Amnesia", category = "CurseRando")]
    public static void Amnesia() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Amnesia");

    [BindableMethod(name = "Apply Darkness", category = "CurseRando")]
    public static void Darkness() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Darkness");

    [BindableMethod(name = "Apply Diminish", category = "CurseRando")]
    public static void Diminish() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Diminish");

    [BindableMethod(name = "Apply Sloth", category = "CurseRando")]
    public static void Sloth() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Sloth");

    [BindableMethod(name = "Apply Unknown", category = "CurseRando")]
    public static void Unknown() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Unknown");

    [BindableMethod(name = "Apply Omen", category = "CurseRando")]
    public static void Omen() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Omen");

    [BindableMethod(name = "Apply Doubt", category = "CurseRando")]
    public static void Doubt() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Doubt");

    [BindableMethod(name = "Apply Confusion", category = "CurseRando")]
    public static void Confusion() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Confusion");

    [BindableMethod(name = "Apply Regret", category = "CurseRando")]
    public static void Regret() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Regret");

    [BindableMethod(name = "Apply Maze", category = "CurseRando")]
    public static void Maze() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Maze");

    [BindableMethod(name = "Apply Despair", category = "CurseRando")]
    public static void Despair() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Despair");

    [BindableMethod(name = "Apply Frail", category = "CurseRando")]
    public static void Frail() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Frail");

    [BindableMethod(name = "Apply Melancholy", category = "CurseRando")]
    public static void Melancholy() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Melancholy");

    [BindableMethod(name = "Apply Trauma", category = "CurseRando")]
    public static void Trauma() => ItemChangerMod.Modules.GetOrAdd<CurseModule>().QueueCurse("Trauma");

    #endregion
}
#endif
