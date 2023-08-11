using CurseRandomizer.ItemData;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Modules;
using KorzUtils.Helper;
using Modding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Modules;

public class DreamNailModule : Module
{
    #region Members

    [NonSerialized]
    private List<DreamFragmentItem> _dreamFragments = new();

    #endregion

    #region Properties

    [JsonIgnore]
    public int DreamUpgrade => _dreamFragments.Count(x => x.IsObtained());

    #endregion

    #region Eventhandler

    private string ShowDreamNailDescription(string key, string sheetTitle, string orig)
    {
        if (key == "INV_DESC_DREAMNAIL_A" || key == "INV_DESC_DREAMNAIL_B")
        {
            orig += "\r\n";
            if (DreamUpgrade == 0)
                orig += "It's not strong enough yet to fight the strong warriors from the past.";
            else if (DreamUpgrade == 1)
                orig += "With the fight fragment you can challenge the restless spirits of the strong warriors. " +
                    "But it isn't strong enough yet to pierce through the stronger remaining dreams.";
            else
                orig += "With both fragments assembled no restless spirit can resist the challenge.";
        }
        return orig;
    }

    

    private void PreventDreamBosses(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, PlayerDataBoolTest self)
    {
        if ((self.IsCorrectContext("Appear", "Ghost Warrior NPC", "Init")
            || self.IsCorrectContext("Conversation Control", "Ghost Warrior NPC", "Init")) && DreamUpgrade == 0)
            self.isTrue = self.isFalse;
        else if (DreamUpgrade < 2 && self.boolName.Value == "hasDreamNail" && self.IsCorrectContext("Control", null, "Check") &&
            (self.Fsm.GameObject.name == "FK Corpse" || self.Fsm.GameObject.name == "IK Remains"
            || self.Fsm.GameObject.name == "Mage Lord Remains"))
            self.isTrue = self.isFalse;
        orig(self);
    }

    private void PreventWhiteDefender(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (DreamUpgrade < 2 && self.FsmName == "Control" && self.gameObject.name == "Dream Enter" && GameManager.instance?.sceneName == "Waterways_15")
            self.GetState("Idle").ClearTransitions();
        orig(self);
    }

    private void PreventGreyPrinceZote(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, IntCompare self)
    {
        if (DreamUpgrade < 2 && self.IsCorrectContext("FSM", "Dream Enter", "Check") && self.Fsm.Variables.FindFsmString("PD Int Name")?.Value == "greyPrinceDefeats")
        {
            self.equal = self.greaterThan;
            self.lessThan = self.greaterThan;
        }
        orig(self);
    }

    #endregion

    #region Methods

    public override void Initialize()
    {
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PreventDreamBosses;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += PreventGreyPrinceZote;
        On.PlayMakerFSM.OnEnable += PreventWhiteDefender;
        ModHooks.LanguageGetHook += ShowDreamNailDescription;
    }

    public override void Unload()
    {
        _dreamFragments.Clear();
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= PreventDreamBosses;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= PreventGreyPrinceZote;
        On.PlayMakerFSM.OnEnable -= PreventWhiteDefender;
        ModHooks.LanguageGetHook -= ShowDreamNailDescription;
    }

    public void AddFragment(DreamFragmentItem item)
    {
        if (!_dreamFragments.Contains(item))
            _dreamFragments.Add(item);
    }

    #endregion
}
