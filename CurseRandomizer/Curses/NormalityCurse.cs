using CurseRandomizer.Helper;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using static On.FSMUtility;

namespace CurseRandomizer.Curses;

internal class NormalityCurse : Curse
{
    public NormalityCurse()
    {
        ModHooks.LanguageGetHook += ShowUselessCharm;
        CheckFsmStateAction.OnEnter += CheckFsmStateAction_OnEnter;
    }

    private void CheckFsmStateAction_OnEnter(CheckFsmStateAction.orig_OnEnter orig, FSMUtility.CheckFsmStateAction self)
    {
        if (self.IsCorrectContext("UI Charms", "Charms", "Deactivate UI"))
            self.falseEvent = DisabledCharmId.Contains(int.Parse(self.Fsm.Variables.FindFsmString("Item Num String").Value)) ? self.trueEvent : null;
        orig(self);
    }

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (name.StartsWith("equippedCharm_") && int.TryParse(name.Substring(14), out int charmId))
            orig = orig && !DisabledCharmId.Contains(charmId);
        return orig;
    }

    internal List<int> DisabledCharmId { get; set; } = new();

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

    public override bool CanApplyCurse()
    {
        if (UseCap && DisabledCharmId.Count >= Cap)
            return false;
        List<int> availableCharms = new();
        for (int i = 1; i < 41; i++)
        {
            // Skip quest charms
            if (i == 36 || i == 10 || i == 17 || i == 23 || i == 24 || i == 25 || i == 40)
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
        if (PlayerData.instance.GetBool("equippedCharm_"+rolledCharm))
        {
            PlayerData.instance.SetBool("equippedCharm_" + rolledCharm, false);
            PlayerData.instance.GetVariable<List<int>>(nameof(PlayerData.instance.equippedCharms)).Remove(rolledCharm);
            HeroController.instance.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        }
    }

    public override object ParseData() => DisabledCharmId;

    public override void LoadData(object data) => DisabledCharmId = (List<int>)data;
}
