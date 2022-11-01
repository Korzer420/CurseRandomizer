namespace CurseRandomizer.Curses;

internal class DesorientationCurse : Curse
{
    public override void ApplyCurse() => GameManager.instance.StartCoroutine(HeroController.instance.Respawn());
}
