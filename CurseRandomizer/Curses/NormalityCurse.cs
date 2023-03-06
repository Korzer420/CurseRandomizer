using KorzUtils.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using static On.FSMUtility;

namespace CurseRandomizer.Curses;

internal class NormalityCurse : Curse
{
    #region Constructors

    public NormalityCurse()
    {
        Data.AdditionalData = new List<int>();
    }

    #endregion

    #region Properties

    internal List<int> DisabledCharmId 
    { 
        get 
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = new List<int>();
            return Data.AdditionalData as List<int>;
        }
    }

    #endregion

    #region Event handler

    private void CheckFsmStateAction_OnEnter(CheckFsmStateAction.orig_OnEnter orig, FSMUtility.CheckFsmStateAction self)
    {
        if (self.IsCorrectContext("UI Charms", "Charms", "Deactivate UI"))
            self.falseEvent = DisabledCharmId.Contains(int.Parse(self.Fsm.Variables.FindFsmString("Item Num String").Value)) ? self.trueEvent : null;
        orig(self);
    }

    private string ShowUselessCharm(string key, string sheetTitle, string orig)
    {
        if (key.StartsWith("CHARM_DESC_"))
        {
            if (int.TryParse(key.Substring(11), out int charmId))
                if (DisabledCharmId.Contains(charmId))
                    orig += $"\r\n<color={TextColor}>It seems like this charm has lost it's power.</color>";
        }
        return orig;
    } 

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        ModHooks.LanguageGetHook += ShowUselessCharm;
        CheckFsmStateAction.OnEnter += CheckFsmStateAction_OnEnter;
    }

    public override void Unhook()
    {
        ModHooks.LanguageGetHook -= ShowUselessCharm;
        CheckFsmStateAction.OnEnter -= CheckFsmStateAction_OnEnter;
    }

    public override bool CanApplyCurse()
    {
        if (UseCap && DisabledCharmId.Count >= Cap)
            return false;
        List<int> availableCharms = new();
        for (int i = 1; i < 41; i++)
        {
            // Skip quest charms
            if (i == 36 || i == 10 || i == 17 || i == 23 || i == 24 || i == 25 || i == 40 || i == 2)
                continue;
            if (PlayerData.instance.GetBool($"gotCharm_{i}"))
                availableCharms.Add(i);
        }
        return availableCharms.Except(DisabledCharmId).Any();
    }

    public override void ApplyCurse()
    {
        List<int> availableCharms = new();
        for (int i = 1; i < 41; i++)
        {
            // Skip quest charms
            if (i == 36 || i == 10 || i == 17 || i == 23 || i == 24 || i == 25 || i == 40)
                continue;
            if (PlayerData.instance.GetBool($"gotCharm_{i}"))
                availableCharms.Add(i);
        }
        availableCharms = availableCharms.Except(DisabledCharmId).ToList();

        int rolledCharm = availableCharms[UnityEngine.Random.Range(0, availableCharms.Count())];
        DisabledCharmId.Add(rolledCharm);
        if (PlayerData.instance.GetBool("equippedCharm_" + rolledCharm))
        {
            PlayerData.instance.SetBool("equippedCharm_" + rolledCharm, false);
            PlayerData.instance.GetVariable<List<int>>(nameof(PlayerData.instance.equippedCharms)).Remove(rolledCharm);
            HeroController.instance.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        }
    }

    public override void ResetAdditionalData() => DisabledCharmId.Clear();

    public override int SetCap(int value) => Math.Max(1, Math.Min(value, 30)); 

    #endregion
}
