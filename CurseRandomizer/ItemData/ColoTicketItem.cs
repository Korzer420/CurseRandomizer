using CurseRandomizer.Enums;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.FsmStateActions;
using KorzUtils.Data;
using KorzUtils.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CurseRandomizer.ItemData;

//public class ColoTicketItem : AbstractItem
//{
//    #region Properties

//    public string Trial { get; set; }

//    #endregion

//    #region Eventhandler

//    private void BlockColoAccess(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
//    {
//        try
//        {
//            if (self.FsmName == "Conversation Control" && self.gameObject.name == Trial + " Trial Board")
//            {
//                CallMethodProper actionReference = self.GetState("Unpaid").GetFirstAction<CallMethodProper>();
//                self.AddState("Unworthy", () => actionReference.gameObject.GameObject.Value.GetComponent<DialogueBox>()
//                    .StartConversation($"Unworthy ({Trial})", "Minor_NPC"),
//                    FsmTransitionData.FromTargetState("Anim End").WithEventName("CONVO_FINISH"));

//                self.AddState("Box Up", self.GetState("Box Up 2").Actions,
//                    FsmTransitionData.FromTargetState("Unworthy").WithEventName("FINISHED"));

//                self.AddState("Has Ticket?", () => self.SendEvent(IsObtained() ? "OPEN" : "FINISHED"), 
//                    FsmTransitionData.FromTargetState("Box Up YN").WithEventName("OPEN"),
//                    FsmTransitionData.FromTargetState("Box Up").WithEventName("FINISHED"));

//                // Set up transition
//                self.GetState("State Check").AdjustTransition("OPEN", "Has Ticket?");
//            }
//        }
//        catch (System.Exception exception)
//        {
//            CurseRandomizer.Instance.LogError("Couldn't modify colo trial boards: " + exception.StackTrace);
//        }
//        orig(self);
//    }

//    #endregion

//    #region Methods

//    protected override void OnLoad()
//    {
//        base.OnLoad();
//    }

//    protected override void OnUnload()
//    {
//        base.OnUnload();
//    }

//    #endregion
//}
