namespace CurseRandomizer.Curses;

internal class EmptynessCurse : Curse
{
    public override bool CanApplyCurse()
    {
        int cap = UseCap ? Cap : 1;
        return PlayerData.instance.GetInt("maxHealth") > cap;
    }

    public override void ApplyCurse() 
    { 
        HeroController.instance.AddToMaxHealth(-1);
        // To force the UI to update to amount of masks.
        if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
            GameCameras.instance.hudCanvas.gameObject.SetActive(true);
        else
        {
            GameCameras.instance.hudCanvas.gameObject.SetActive(false);
            GameCameras.instance.hudCanvas.gameObject.SetActive(true);
        }
    }
}