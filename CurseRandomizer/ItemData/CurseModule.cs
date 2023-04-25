using CurseRandomizer.Curses;
using HutongGames.PlayMaker;
using ItemChanger.Modules;
using KorzUtils.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.ItemData;

public class CurseModule : Module
{
    private bool _isQueueRunning = false;

    /// <summary>
    /// Gets or sets a list with curses that should be applied once the player has control over their character again.
    /// </summary>
    public List<string> CurseQueue { get; set; } = new();

    public override void Initialize() { }

    public override void Unload()
    {
        CurseManager.Handler.StopAllCoroutines();
    }

    public void QueueCurse(string curse)
    {
        CurseQueue.Add(curse);
        if (_isQueueRunning)
            return;
        _isQueueRunning = true;
        CurseManager.Handler.StartCoroutine(WaitForControl());
    }

    internal IEnumerator WaitForControl()
    {
        while (HeroController.instance == null || !HeroController.instance.acceptingInput)
        {
            PlayerData.instance?.SetBool(nameof(PlayerData.instance.disablePause), true);
            yield return null;
        }

        // Display the FOOL text.
        GameHelper.DisplayMessage("FOOL!");
        if (DespairCurse.DespairActive)
        { 
            (CurseManager.GetCurse<DespairCurse>().Data.AdditionalData as DespairTracker).CurseDesperation++;
            CurseManager.GetCurse<DespairCurse>().UpdateProgression();
        }

        while (CurseQueue.Any())
        {
            // Pain will be stacked
            if (CurseQueue[0] == "Pain")
            {
                int painAmount = CurseQueue.RemoveAll(x => x == "Pain");
                if (!CurseManager.GetCurse<PainCurse>().Data.Ignored)
                {
                    CurseManager.GetCurse<PainCurse>().Data.CastedAmount += painAmount;
                    PainCurse.DoDamage(painAmount);
                }
            }
            // Casting multiple disorientation curses doesn't serve any purpose which is why the get removed all at once.
            else if (CurseQueue[0] == "Disorientation")
            {
                int amount = CurseQueue.RemoveAll(x => x == "Disorientation");
                if (!CurseManager.GetCurse<DisorientationCurse>().Data.Ignored)
                {
                    CurseManager.GetCurse<DisorientationCurse>().Data.CastedAmount += amount;
                    CurseManager.GetCurse<DisorientationCurse>().ApplyCurse();
                }
            }
            else
            {
                Curse curse = CurseManager.GetCurseByName(CurseQueue[0]);
                if (curse != null)
                {
                    if (!curse.Data.Ignored)
                        if (!curse.CanApplyCurse())
                        {
                            CurseManager.GetCurse<DisorientationCurse>().Data.CastedAmount++;
                            CurseManager.GetCurse<DisorientationCurse>().ApplyCurse();
                        }
                        else
                        {
                            curse.Data.CastedAmount++;
                            curse.ApplyCurse();
                        }
                }
                else
                    CurseRandomizer.Instance.LogError("Tried to cast unknown curse: " + CurseQueue[0]);
                CurseQueue.RemoveAt(0);
            }
            yield return new WaitForSeconds(0.2f);
        }
        PlayerData.instance.SetBool(nameof(PlayerData.instance.disablePause), false);
        _isQueueRunning = false;
    }
}
