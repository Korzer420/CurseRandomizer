using CurseRandomizer.Modules;
using ItemChanger;
using KorzUtils.Helper;
using System.Collections.Generic;
using System.Linq;
using static CurseRandomizer.Modules.CurseModule;

namespace CurseRandomizer.ItemData;

public record CurseBargainCost : Cost
{
    #region Constructors

    public CurseBargainCost(int curses) => CursesToCast = curses;
    
    #endregion

    #region Properties

    public int CursesToCast { get; set; }

    #endregion

    public override bool CanPay() => true;

    public override string GetCostText() => $"<color=#ff17ff>Picking this up will cast {CursesToCast} curses onto you.</color>";

    public override bool HasPayEffects() => false;

    public override void OnPay()
    {
        CoroutineHelper.WaitFrames(() =>
        {
            PlayMakerFSM.BroadcastEvent("CLOSE SHOP WINDOW");
            HeroController.instance.RegainControl();
            HeroController.instance.StartAnimationControl();
            PlayerData.instance.SetBool(nameof(PlayerData.instance.disablePause), true);
            CurseModule curseModule = ItemChangerMod.Modules.GetOrAdd<CurseModule>();

            List<string> appliedCurses = new();
            QueueFinished queueFinished = null;
            queueFinished = () =>
            {
                curseModule.OnFinished -= queueFinished;
                GameHelper.DisplayMessage("Applied curses: "+ appliedCurses.Aggregate((x,y) => $"{x}, {y}"));
            };
            curseModule.OnFinished += queueFinished;

            for (int i = 0; i < CursesToCast; i++)
            {
                List<Curse> viableCurses = CurseManager.GetCurses().Where(x => x.CanApplyCurse()).ToList();
                Curse selectedCurse = viableCurses[UnityEngine.Random.Range(0, viableCurses.Count)];
                appliedCurses.Add(selectedCurse.Name);
                curseModule.QueueCurse(selectedCurse.Name);
            }
        }, true, 30);
        
    }
}
