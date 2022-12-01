using ItemChanger;
using System.Collections.Generic;
using System.Linq;
using static ItemChanger.ItemNames;

namespace CurseRandomizer;

public static class MimicNames
{
    public readonly static Dictionary<string, string[]> Mimics = new()
    {
        // Skills
        { Monarch_Wings, new string[]{"Wonarch Mings", "Monach Wings", "Monarch Wing", "Monarch W1ngs", "Double Jump" } },
        { Left_Mantis_Claw, new string[]{"Left Mantis C1aw", "Mantis Left Claw", "Left Manis Claw", "Left Manti5 Claw", "Left Claw", "Left Mantis Clavv" } },
        { Right_Mantis_Claw, new string[]{"Right Mantis C1aw", "Mantis Right Claw", "Right Manis Claw", "Right Manti5 Claw", "Right Claw", "Right Mantis Clavv" } },
        { Mantis_Claw, new string[]{"Mantis C1aw", "Manis Claw", "Manti5 Claw", "Claw", "Mantis Clavv" } },
        { Left_Mothwing_Cloak, new string[]{ "Left M0thwing Cloak", "Left Mothwing Cl0ak", "Left Motwing Cloak", "Left Mothwing C1oak", "Let Mothwing Cloak", "Left Dash" } },
        { Mothwing_Cloak, new string[]{ "M0thwing Cloak", "Mothwing Cl0ak", "Motwing Cloak", "Mothwing C1oak", "Dash", "Cothwing Mloak" } },
        { Right_Mothwing_Cloak, new string[]{ "Right M0thwing Cloak", "Right Mothwing Cl0ak", "Right Motwing Cloak", " Right Mothwing C1oak", "Right Dash", "Rigt Mothwing Cloak", "Ride Mothwing Cloak" } },
        { Shade_Cloak, new string[] {"Shad Cloak", "Schade Cloak", "Shade Clock"} },
        { Crystal_Heart, new string[]{"Crystal Hearth", "Crysta1 Heart", "Cyrstal Heart", "Superdash", "Krystal Heart"} },
        { Left_Crystal_Heart, new string[]{"Lef7 Crystal Heart", "Left Crysta1 Heart", "Left Superdash", "Left Cyrstal Heart" } },
        { Right_Crystal_Heart, new string[]{"Rigth Crystal Heart", "Right Crysta1 Heart", "Right Superdash", "Right Cyrstal Heart" } },
        { Vengeful_Spirit, new string[]{"Fireball", "Vengful Spirit", "Vengefu1 Spirit", "Ven9eful Spirit", "Vengeful Sprit"} },
        { Shade_Soul, new string[]{"Shade Sou1", "Schade Soul", "Shade Sool", "Fireball" } },
        { Desolate_Dive, new string[]{"Desolate Diwe", "Deso1ate Dive", "Desolat Dive", "Disolate Dive", "Dive"} },
        { Descending_Dark, new string[]{"Desending Dark", "Descending Dak", "Descend Diark", "Descending Dive", "DDark" } },
        { Howling_Wraiths, new string[]{"Howling Wraths", "Howling Wraihts", "Holwing Wraiths", "Howling Wraith", "How1ing Wraiths" } },
        { Abyss_Shriek, new string[]{"Abys Shriek", "Abyss Sriek", "Abyss Shreik", "Abyss Shiek" } },
        { Ismas_Tear, new string[]{ "1sma Tear", "Isma's Tears", "1sma's Tear", "Tear", "Imsa's Tear", "Isma's Teer" } },
        { Cyclone_Slash, new string[]{"Cyklone Slash", "Cyclone Slesh", "Ciclone Slash", "Cyc1one Slash", "Cyclone S1ash" } },
        { Great_Slash, new string[]{"Greet Slash", "Great S1ash", "Great Slahs" } },
        { Dash_Slash, new string[]{"Dash's Slash", "Desh Slash", "Dasch Slash", "Dash Slesh", "Dash S1ash" } },
        { Focus, new string[]{"Healing", "Focus Spell", "Fukos"} },
        { World_Sense, new string[]{"Wor1d Sense", "World Sence"} },
        { Lumafly_Lantern, new string[]{"Lantern", "Lumaflies Lantern", "Lumafly Lanter", "Lumaf1y Lantern" } },
        { Dream_Nail, new string[] {"Dreem Nail", "Dream Nayl", "Dream Nai1" } },
        { Dream_Gate, new string[] {"Dreem Gate", "Dream Gade", "Driem Gate", "Dream Gaid" } },
        { Awoken_Dream_Nail, new string[] {"Awoken Dream Nail", "Dream Nail Upgrade", "Better Dream Nail", "Awokened Dream Nail" } },
        // Charms
        { Wayward_Compass, new string[]{"Waywad Compass", "Compass", "Wayward Compas" } },
        { Gathering_Swarm, new string[]{"Gatering Swarm", "Gathering Sarm", "Gathering Swam"} },
        { Stalwart_Shell, new string[]{"Stalward Shell", "Stalvart Shell", "Stalwart Shall", "Stalwart Shel", "Stalwart She11"} },
        { Soul_Catcher, new string[]{"Sool Catcher", "Soul Catcha", "Soul Capture", "Soul Catchar", "Sou1 Catcher"} },
        { Shaman_Stone, new string[]{"Shamam Stone", "Shanan Stone", "Shaman's Stone", "Shaman Stones"} },
        { Soul_Eater, new string[]{"Sool Eater", "Soul Eaters", "Sou1 Eater"} },
        { Dashmaster, new string[]{"Dash Master", "Deshmaster"} },
        { Sprintmaster, new string[]{"Spintmaster", "Sprint Master"} },
        { Grubsong, new string[]{"Grobsong", "Grubbsong", "Grubson9"} },
        { Grubberflys_Elegy, new string[]{"Grubberflys Elegy", "Grubberfly's Elegey", "Grubfly's Elegy", "Grubberflies Elegy"} },
        { Fragile_Heart, new string[]{"Fragil Heart", "Fragile Hearth", "Fragile Hart"} },
        { Fragile_Greed, new string[]{"Fragil Greed", "Fragile Gread", "Fragi1e Greed", "Fragile Greet"} },
        { Fragile_Strength, new string[]{"Fragil Strength", "Fragile Strentgh", "Fragile Strenght"} },
        { Spell_Twister, new string[]{"Spe11 Twister", "Spell Twistar", "Spelltwister"} },
        { Steady_Body, new string[]{"Steaty Body", "Stead Body", "Stady Body", "Steady Bodie"} },
        { Heavy_Blow, new string[]{"Heavi Blow", "Heavy Blovv", "Heavy Bl0w"} },
        { Quick_Slash, new string[]{"Quik Slash", "Quick S1ash", "Quick Slesh"} },
        { Longnail, new string[]{"Longnai1", "Long Nail"} },
        { Mark_of_Pride, new string[]{"Mark 0f Pride", "Mak of Pride"} },
        { Fury_of_the_Fallen, new string[]{"Fury of Fallen", "Furry of the Fallen", "Fury of the Fa11en"} },
        { Thorns_of_Agony, new string[]{"Thorn of Agony", "Throns of Agony", "Thorns of Agonie"} },
        { Baldur_Shell, new string[]{"Baldur She11", "Ba1dur Shell", "Baldur Shall", "Baldur Sell"} },
        { Flukenest, new string[]{"Fluke Nest", "Fluknest", "Flukenet"} },
        { Defenders_Crest, new string[]{"Defender Crest", "Defenders Crest", "Defender's Cest"} },
        { Glowing_Womb, new string[]{"Glovving Womb", "Wlowing Gomb"} },
        { Quick_Focus, new string[]{"Quik Focus", "Quicker Focus", "Quick Fokus"} },
        { Deep_Focus, new string[]{"Deeep Focus", "Deep F0cus"} },
        { Lifeblood_Heart, new string[]{"Life Blood Heart", "Lifeb1ood Heart", "Lifeblood Hearth", "Lifeblood Harth"} },
        { Lifeblood_Core, new string[]{"Life Blood Core", "Liveblood Core"} },
        { Jonis_Blessing, new string[]{"Jonis Blessing", "Joni Blessing"} },
        { Hiveblood, new string[]{"Hiweblood", "Hiveblod", "Hivebloot"} },
        { Spore_Shroom, new string[]{"Spor Shroom", "Spore Shoom"} },
        { Sharp_Shadow, new string[]{"Sharb Shadow"} },
        { Shape_of_Unn, new string[]{"Unns Shape", "Shabe of Unn", "Shape of Un"} },
        { Nailmasters_Glory, new string[]{"Nailmaster Glory", "Nail Master Glory", "Nai1master's Glory", "Nailmaster's Glorie"} },
        { Weaversong, new string[]{"VVeaversong", "Weaver Song", "Weaver Sog"} },
        { Dream_Wielder, new string[]{"Dream VVielder", "Drean Wielder", "Dream Wie1der"} },
        { Dreamshield, new string[]{"Dream Shield", "Dreem Shield"} },
        { Grimmchild1, new string[]{"Grimm Child", "Grim Child"} },
        { King_Fragment, new string[]{"Whit Fragment", "White Fragement"} },
        { Queen_Fragment, new string[]{"Whit Fragment", "White Fragement"} },
        { Kingsoul, new string[]{"King's Soul", "King Soul"} },
        { Void_Heart, new string[]{"Voidheart", "Voit Heart", "Void Hearth"} },
        // Dreamer
        { Monomon, new string[]{"Momomon", "Mononon", "Monoomon", "Onomon"} },
        { Lurien, new string[]{"Lurin", "Lorien", "Lucien", "Lurian"} },
        { Herrah, new string[]{"Herra", "Herah", "Herrach"} },
        // Relics
        { Wanderers_Journal, new string[]{"Wander Journal", "Wanderers Journal", "Wanderer's J0urnal", "Wanderer's Joornal" } },
        { Hallownest_Seal, new string[]{"Hallawnest Seal", "Hollownest Seal", "Hallownest Seel" } },
        { Kings_Idol, new string[]{"King's 1dol", "King Idol", "Kin9 Idol", "Kings Idol" } },
        { Arcane_Egg, new string[]{ "Arkane Egg", "Arcane Eggg", "Acane Egg" } },
        // Stag
        { Stag_Nest_Stag, new string[]{"Stag's Nest Stag", "Stack Nest Stack", "Sta9 Nest Sta9"} },
        { City_Storerooms_Stag, new string[]{"Citie Storerooms Stag", "City Store Rooms Stag", "City Store Room's Stag"} },
        { Crossroads_Stag, new string[]{"Crosroads Stag", "Crossroad Stag", "Crossroads Stags"} },
        { Dirtmouth_Stag, new string[]{"Dirmouth Stag", "Dirthmouth Stag", "Dirtmout Stag"} },
        { Distant_Village_Stag, new string[]{"Distance Village Stag", "Distant Vi11age Stag"} },
        { Greenpath_Stag, new string[]{"Green Path Stag", "Greypath Staag", "Greenpath "} },
        { Hidden_Station_Stag, new string[]{"Hidden's Station Stag", "Hidden Stag"} },
        { Kings_Station_Stag, new string[]{"King Station Stag", "King's Stag", "King's Station Stack"} },
        { Queens_Gardens_Stag, new string[]{ "Queen Garden Stag", "White Lady Garden Stag", "Garden Stag", "Quen's Garden Stag" } },
        { Queens_Station_Stag, new string[]{ "Queen Station Stag", "Queen Stetion Stag", "Quen's Station Stag", "Queen's Station Stack" } },
        // Misc
        { Grub, new string[]{ "Grob", "Grubb", "Grup" } },
        { City_Crest , new string[]{"City's Crest", "Zity Crest", "Citie Crest" } },
        { Tram_Pass, new string[]{"Trem Pass", "Tramm Pass", "Tram Pas", "Tram Passs", "Tram Pess" } },
        { Mask_Shard, new string[]{"Mask Sard", "Mask Shart", "Masks Shard" } },
        { Vessel_Fragment, new string[]{"Vesel Fragment", "Vessel Fragement", "Wessel Fragment", "Vessel Shard"} },
        { Simple_Key, new string[]{"Simpel Key", "Simple Keiy", "Simp1e Key" } },
        { Love_Key, new string[]{"Lowe Key", "Love Kay"} },
        { Elegant_Key, new string[]{"Elegent Key", "Elagant Key", "Elegand Key" } },
        { Pale_Ore, new string[]{"Pal Ore", "Pa1e Ore", "Pale Or", "Ore" } },
        { Rancid_Egg, new string[]{"Rancit Egg", "Rancid's Egg"} },
        { Kings_Brand, new string[]{"King Brand", "King's Brendt", "King's Brands", "Kang's Brind"} }
    };

    /// <summary>
    /// Checks if it the passed item is a major item (for the all major tracker). Only checks for skills.
    /// </summary>
    public static bool IsMajorItem(string item) => Mimics.Take(27).Any(x => x.Key == item);
}
