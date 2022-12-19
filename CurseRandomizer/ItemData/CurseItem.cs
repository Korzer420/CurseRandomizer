using CurseRandomizer.ItemData;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace CurseRandomizer;

/// <summary>
/// An item, which appears as a normal item, but applies a curse.
/// </summary>
internal class CurseItem : AbstractItem
{
    public const string CursePrefix = "Fool_Item-";

    public string CurseName { get; set; }

    public override void GiveImmediate(GiveInfo info)
    {
        Curse curse = CurseManager.GetCurseByName(CurseName);
        CurseModule module = ItemChangerMod.Modules.GetOrAdd<CurseModule>();
        if (curse.CanApplyCurse())
            module.QueueCurse(CurseName);
        else if (CurseManager.DefaultCurse.CanApplyCurse())
        {
            module.QueueCurse(CurseManager.DefaultCurse.Name);
            CurseName = CurseManager.DefaultCurse.Name;
        }
        else
        {
            module.QueueCurse("Disorientation");
            CurseName = "Disorientation";
        }

        if (UIDef is not BigUIDef || (info.MessageType != MessageType.Any && info.MessageType != MessageType.Big))
            (UIDef as MsgUIDef).name = new BoxedString($"<color=#c034eb>{CurseName}</color>");
        else
        {
            // For the big UI we want to display the FOOL message there, which is why the recent item name is set via a tag.
            (UIDef as MsgUIDef).name = new BoxedString($"<color=#c034eb>Fool!</color>");
            tags.Add(new InteropTag() { Message = "RecentItems", Properties = new() { { "DisplayName", $"<color=#c034eb>{CurseName}</color>" } } });
            (UIDef as BigUIDef).descOne = new BoxedString("You've been cursed by " + CurseName);
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
