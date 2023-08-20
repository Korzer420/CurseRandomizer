using CurseRandomizer.Modules;
using ItemChanger;
using System.Collections.Generic;

namespace CurseRandomizer.Manager;

/// <summary>
/// A manager which modifies all ingame related stuff. (Besides curses)
/// </summary>
internal static class ModManager
{
    #region Members

    private static List<AbstractPlacement> _placementsToAdd = new();

    #endregion

    static ModManager() => On.UIManager.StartNewGame += UIManager_StartNewGame;
    
    #region Properties

    public static int[] WalletCapacities { get; set; }

    #endregion

    #region Eventhandler

    private static void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        orig(self, permaDeath, bossRush);

        if (!RandomizerMod.RandomizerMod.IsRandoSave)
            return;

        if (WalletCapacities.Length > 0)
            ItemChangerMod.Modules.GetOrAdd<WalletModule>().Capacities = WalletCapacities;
        if (CurseRandomizer.Instance.Settings.GeneralSettings.Enabled && CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
        {
            ItemChangerMod.Modules.GetOrAdd<CurseModule>();
            foreach (Curse curse in CurseManager.GetCurses())
                curse.ResetData();
            if (ItemChanger.Internal.Ref.Settings.Placements.ContainsKey(LocationNames.Iselda))
                ItemChanger.Internal.Ref.Settings.Placements[LocationNames.Iselda].Add(Finder.GetItem("Generosity"));
            else
            {
                AbstractPlacement placement = Finder.GetLocation(LocationNames.Iselda).Wrap();
                placement.Add(Finder.GetItem("Generosity"));
                placement.Add(Finder.GetItem(ItemNames.Wayward_Compass));
                placement.Add(Finder.GetItem(ItemNames.Ancient_Basin_Map));
                placement.Add(Finder.GetItem(ItemNames.City_of_Tears_Map));
                placement.Add(Finder.GetItem(ItemNames.Crossroads_Map));
                placement.Add(Finder.GetItem(ItemNames.Crystal_Peak_Map));
                placement.Add(Finder.GetItem(ItemNames.Deepnest_Map));
                placement.Add(Finder.GetItem(ItemNames.Fog_Canyon_Map));
                placement.Add(Finder.GetItem(ItemNames.Fungal_Wastes_Map));
                placement.Add(Finder.GetItem(ItemNames.Greenpath_Map));
                placement.Add(Finder.GetItem(ItemNames.Howling_Cliffs_Map));
                placement.Add(Finder.GetItem(ItemNames.Kingdoms_Edge_Map));
                placement.Add(Finder.GetItem(ItemNames.Queens_Gardens_Map));
                placement.Add(Finder.GetItem(ItemNames.Resting_Grounds_Map));
                placement.Add(Finder.GetItem(ItemNames.Royal_Waterways_Map));
                placement.Add(Finder.GetItem(ItemNames.Quill));
                ItemChangerMod.AddPlacements(new List<AbstractPlacement>() { placement });
            }
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.Enabled && CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel > 0)
            ItemChangerMod.Modules.GetOrAdd<VesselModule>().SoulVessel = 2 - CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel;
    } 

    #endregion

    //private static void AddShopDefaults()
    //{
    //    _placementsToAdd.Clear();

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Charms)
    //    {
    //        // Salubra charms.
    //        AddDefaultItemToShop(Salubra_Cheap, ItemNames.Steady_Body, 150);
    //        AddDefaultItemToShop(Salubra_Medium, ItemNames.Lifeblood_Heart, 250);
    //        AddDefaultItemToShop(Salubra_Medium, ItemNames.Longnail, 300);
    //        AddDefaultItemToShop(Salubra_Medium, ItemNames.Shaman_Stone, 220);
    //        AddDefaultItemToShop(Salubra_Expensive, ItemNames.Quick_Focus, 800);
    //        AddDefaultItemToShop(Salubra_Expensive, ItemNames.Salubras_Blessing, 800, 40);

    //        // Iselda charms
    //        AddDefaultItemToShop(Iselda_Medium, ItemNames.Wayward_Compass, 220);

    //        // Sly charms.
    //        AddDefaultItemToShop(Sly_Cheap, ItemNames.Stalwart_Shell, 200);
    //        AddDefaultItemToShop(Sly_Medium, ItemNames.Gathering_Swarm, 300);
    //        AddDefaultItemToShop(Sly_Key_Medium, ItemNames.Heavy_Blow, 369);
    //        AddDefaultItemToShop(Sly_Key_Medium, ItemNames.Sprintmaster, 420);

    //        // Leg eater
    //        AddDefaultItemToShop(Leg_Eater_Medium, ItemNames.Fragile_Heart, 350);
    //        AddDefaultItemToShop(Leg_Eater_Medium, ItemNames.Fragile_Greed, 250);
    //        AddDefaultItemToShop(Leg_Eater_Expensive, ItemNames.Fragile_Strength, 600);

    //        // Repairable charms
    //        Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
    //        AbstractPlacement currentPlacement;
    //        AbstractItem currentItem = Finder.GetItem(ItemNames.Fragile_Heart_Repair);
    //        currentItem.tags ??= new();
    //        currentItem.AddTag(new CostTag() { Cost = new GeoCost(200) });
    //        currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_23) });
    //        if (!placements.ContainsKey(Leg_Eater_Cheap) && !_placementsToAdd.Select(x => x.Name).Contains(Leg_Eater_Cheap))
    //        {
    //            currentPlacement = Finder.GetLocation(Leg_Eater_Cheap).Wrap();
    //            _placementsToAdd.Add(currentPlacement);
    //        }
    //        else if (!placements.ContainsKey(Leg_Eater_Cheap))
    //            currentPlacement = _placementsToAdd.First(x => x.Name == Leg_Eater_Cheap);
    //        else
    //            currentPlacement = placements[Leg_Eater_Cheap];
    //        currentPlacement.Add(currentItem);

    //        // Greed
    //        currentItem = Finder.GetItem(ItemNames.Fragile_Greed_Repair);
    //        currentItem.tags ??= new();
    //        currentItem.AddTag(new CostTag() { Cost = new GeoCost(150) });
    //        currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_24) });
    //        currentPlacement.Add(currentItem);

    //        // Strength
    //        currentItem = Finder.GetItem(ItemNames.Fragile_Strength_Repair);
    //        currentItem.tags ??= new();
    //        currentItem.AddTag(new CostTag() { Cost = new GeoCost(350) });
    //        currentItem.AddTag(new PDBoolShopReqTag() { reqVal = true, fieldName = nameof(PlayerData.instance.brokenCharm_25) });
    //        if (!placements.ContainsKey(Leg_Eater_Medium) && !_placementsToAdd.Select(x => x.Name).Contains(Leg_Eater_Medium))
    //        {
    //            currentPlacement = Finder.GetLocation(Leg_Eater_Medium).Wrap();
    //            _placementsToAdd.Add(currentPlacement);
    //        }
    //        else if (!placements.ContainsKey(Leg_Eater_Medium))
    //            currentPlacement = _placementsToAdd.First(x => x.Name == Leg_Eater_Medium);
    //        else
    //            currentPlacement = placements[Leg_Eater_Medium];
    //        currentPlacement.Add(currentItem);
    //    }

    //    if (RandomizerMod.RandomizerMod.RS.GenerationSettings.MiscSettings.SalubraNotches == SalubraNotchesSetting.Vanilla)
    //    {
    //        // Salubra charms.
    //        AddDefaultItemToShop(Salubra_Cheap, ItemNames.Charm_Notch, 150, 5);
    //        AddDefaultItemToShop(Salubra_Medium, ItemNames.Charm_Notch, 500, 10);
    //        AddDefaultItemToShop(Salubra_Expensive, ItemNames.Charm_Notch, 900, 18);
    //        AddDefaultItemToShop(Salubra_Extreme_Valuable, ItemNames.Charm_Notch, 1400, 25);
    //    }

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Keys)
    //    {
    //        AddDefaultItemToShop(Sly_Expensive, ItemNames.Simple_Key, 950);
    //        AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Elegant_Key, 800);
    //    }

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.MaskShards)
    //    {
    //        AddDefaultItemToShop(Sly_Cheap, ItemNames.Mask_Shard, 150);
    //        AddDefaultItemToShop(Sly_Medium, ItemNames.Mask_Shard, 500);
    //        AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Mask_Shard, 800);
    //        AddDefaultItemToShop(Sly_Key_Extreme_Valuable, ItemNames.Mask_Shard, 1500);
    //    }

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.VesselFragments)
    //    {
    //        AddDefaultItemToShop(Sly_Expensive, ItemNames.Vessel_Fragment, 550);
    //        AddDefaultItemToShop(Sly_Key_Expensive, ItemNames.Vessel_Fragment, 900);
    //    }

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Keys)
    //        AddDefaultItemToShop(Sly_Extreme_Valuable, ItemNames.Lumafly_Lantern, 1800);

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.RancidEggs)
    //        AddDefaultItemToShop(Sly_Cheap, ItemNames.Rancid_Egg, 69);

    //    if (!RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Maps)
    //    {
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Quill, 120);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Ancient_Basin_Map, 112);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.City_of_Tears_Map, 90);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Crossroads_Map, 30);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Crystal_Peak_Map, 112);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Deepnest_Map, 38);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Fog_Canyon_Map, 150);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Fungal_Wastes_Map, 75);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Greenpath_Map, 60);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Howling_Cliffs_Map, 75);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Kingdoms_Edge_Map, 112);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Queens_Gardens_Map, 150);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Resting_Grounds_Map, 69);
    //        AddDefaultItemToShop(Iselda_Cheap, ItemNames.Royal_Waterways_Map, 75);
    //    }

    //    if (_placementsToAdd.Any())
    //        ItemChangerMod.AddPlacements(_placementsToAdd);
    //}

    //private static void AddDefaultItemToShop(string locationName, string itemName, int geoCost, int charmCost = -1)
    //{
    //    Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
    //    AbstractPlacement currentPlacement;
    //    AbstractItem currentItem = Finder.GetItem(itemName);
    //    currentItem.tags ??= new();
    //    if (charmCost == -1)
    //        currentItem.AddTag(new CostTag() { Cost = new GeoCost(geoCost) });
    //    else
    //        currentItem.AddTag(new CostTag()
    //        {
    //            Cost = new MultiCost(
    //            new GeoCost(geoCost),
    //            new PDIntCost(charmCost, nameof(PlayerData.instance.charmsOwned), $"You need to have {charmCost} charms to buy this."))
    //        });

    //    if (!placements.ContainsKey(locationName) && !_placementsToAdd.Select(x => x.Name).Contains(locationName))
    //    {
    //        currentPlacement = Finder.GetLocation(locationName).Wrap();
    //        _placementsToAdd.Add(currentPlacement);
    //    }
    //    else if (!placements.ContainsKey(locationName))
    //        currentPlacement = _placementsToAdd.First(x => x.Name == locationName);
    //    else
    //        currentPlacement = placements[locationName];
    //    currentPlacement.Add(currentItem);
    //}
}
