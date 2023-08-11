using CurseRandomizer.Curses;
using HutongGames.PlayMaker;
using ItemChanger;
using ItemChanger.Modules;
using KorzUtils.Helper;
using Modding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurseRandomizer.Modules;

public class CurseModule : Module
{
    public delegate void QueueFinished();

    #region Members

    private bool _isQueueRunning = false;
    /// <summary>
    /// Fired when the queue casted the last curse, so it remains empty.
    /// </summary>
    public event QueueFinished OnFinished;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a list with curses that should be applied once the player has control over their character again.
    /// </summary>
    public List<string> CurseQueue { get; set; } = new();

    #endregion

    #region Event handler

    private int ForceBargainAvailability(string name, int orig) => name == "takeCurse" ? 1 : orig;

    #endregion

    #region Methods

    public override void Initialize()
    {
        ItemChangerMod.Modules.GetOrAdd<VesselModule>();
        foreach (Curse curse in CurseManager.GetCurses())
            curse.ApplyHooks();
        CoroutineHelper.WaitForHero(() =>
        {
            if (CurseQueue.Any())
                CurseManager.Handler.StartCoroutine(WaitForControl());
        }, true);
        ModHooks.GetPlayerIntHook += ForceBargainAvailability;
    }

    public override void Unload()
    {
        foreach (Curse curse in CurseManager.GetCurses())
            curse.Unhook();
        CurseManager.Handler.StopAllCoroutines();
        ModHooks.GetPlayerIntHook -= ForceBargainAvailability;
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
        OnFinished?.Invoke();
        PlayerData.instance.SetBool(nameof(PlayerData.instance.disablePause), false);
        _isQueueRunning = false;
    } 

    #endregion
}
