using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using System.Collections;
using UnityEngine;

namespace CurseRandomizer;

/// <summary>
/// An item, which appears as a normal item, but applies a curse.
/// </summary>
internal class CurseItem : AbstractItem
{
    public string CurseName { get; set; }

    public override void GiveImmediate(GiveInfo info)
    {
        Curse curse = CurseManager.GetCurseByName(CurseName);
        bool showFoolBox = UIDef is not BigUIDef || (info.MessageType != MessageType.Any && info.MessageType != MessageType.Big);
        if (curse.CanApplyCurse())
            curse.CastCurse(showFoolBox);
        else if (CurseManager.DefaultCurse.CanApplyCurse())
        {
            CurseManager.DefaultCurse.CastCurse(showFoolBox);
            CurseName = CurseManager.DefaultCurse.Name;
        }
        else
        {
            CurseManager.GetCurseByType(CurseType.Desorientation).CastCurse(showFoolBox);
            CurseName = "Desorientation";
        }

        if (UIDef is not BigUIDef || (info.MessageType != MessageType.Any && info.MessageType != MessageType.Big))
            (UIDef as MsgUIDef).name = new BoxedString($"<color=#c034eb>{CurseName}</color>");
        else
        {
            // For the big UI we want to display the FOOL message there, which is why the recent item name is set via a tag.
            (UIDef as MsgUIDef).name = new BoxedString($"<color=#c034eb>Fool!</color>");
            tags.Add(new InteropTag() { Message = "RecentItems", Properties = new() { { "DisplayName", CurseName } } });
        }
        (UIDef as MsgUIDef).sprite = new CustomSprite("Fool");

        // Forces the player out of a shop.
        if (info.Transform != null && (
            info.Transform.name == "Sly Shop" || info.Transform.name == "Sly"
            || info.Transform.name == "Iselda" || info.Transform.name == "Charm Slug"
            || info.Transform.name == "Leg_Eater"))
        {
            PlayMakerFSM.BroadcastEvent("CLOSE SHOP WINDOW");
            HeroController.instance.RegainControl();
            HeroController.instance.StartAnimationControl();
            PlayerData.instance.SetBool(nameof(PlayerData.instance.disablePause), false);
        }
    }

}
