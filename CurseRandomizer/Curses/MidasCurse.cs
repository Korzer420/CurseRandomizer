using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.UIDefs;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemChanger.ItemNames;

namespace CurseRandomizer.Curses;

internal class MidasCurse : Curse
{
    #region Members

    private Coroutine _coroutine;
    private float _safeTime = 5f;
    private bool _cursedTouch = false;
    private static tk2dSprite _sprite;
    private List<string> _curseableItems = new();
    private static readonly string[] _alwaysReplacable = new string[]
    {
        Soul_Catcher,
        Soul_Eater,
        Soul_Refill,
        Soul_Totem_A,
        Soul_Totem_B,
        Soul_Totem_C,
        Soul_Totem_D,
        Soul_Totem_E,
        Soul_Totem_F,
        Soul_Totem_G,
        Soul_Totem_Palace,
        Soul_Totem_Path_of_Pain,
        Geo_Chest_Crystal_Peak,
        Geo_Chest_False_Knight,
        Geo_Chest_Greenpath,
        Geo_Chest_Junk_Pit_1,
        Geo_Chest_Junk_Pit_2,
        Geo_Chest_Junk_Pit_3,
        Geo_Chest_Junk_Pit_5,
        Geo_Chest_Mantis_Lords,
        Geo_Chest_Resting_Grounds,
        Geo_Chest_Soul_Master,
        Geo_Chest_Watcher_Knights,
        Geo_Chest_Weavers_Den,
        Geo_Rock_Abyss,
        Geo_Rock_City,
        Geo_Rock_Deepnest,
        Geo_Rock_Default,
        Geo_Rock_Fung01,
        Geo_Rock_Fung02,
        Geo_Rock_Grave01,
        Geo_Rock_Grave02,
        Geo_Rock_GreenPath01,
        Geo_Rock_GreenPath02,
        Geo_Rock_Hive,
        Geo_Rock_Mine,
        Geo_Rock_Outskirts,
        Geo_Rock_Outskirts420,
        Shade_Soul,
        Abyss_Shriek,
        Descending_Dark,
        Gathering_Swarm,
        Grubsong,
        Stalwart_Shell,
        Baldur_Shell,
        Fury_of_the_Fallen,
        Quick_Focus,
        Flukenest,
        Thorns_of_Agony,
        Steady_Body,
        Heavy_Blow,
        Sharp_Shadow,
        Shaman_Stone,
        Glowing_Womb,
        Nailmasters_Glory,
        Hiveblood,
        Dream_Wielder,
        Dashmaster,
        Quick_Slash,
        Spell_Twister,
        Deep_Focus,
        Grubberflys_Elegy,
        Sprintmaster,Dreamshield,
        Weaversong,
        Charm_Notch,
        Mask_Shard,
        Double_Mask_Shard,
        Full_Mask,
        Vessel_Fragment,
        Double_Vessel_Fragment,
        Full_Soul_Vessel,
        "Nail_Upgrade",
        Quill,
        Ancient_Basin_Map,
        City_of_Tears_Map,
        Collectors_Map,
        Crossroads_Map,
        Crystal_Peak_Map,
        Deepnest_Map,
        Fog_Canyon_Map,
        Fungal_Wastes_Map,
        Greenpath_Map,
        Howling_Cliffs_Map,
        Kingdoms_Edge_Map,
        Queens_Gardens_Map,
        Resting_Grounds_Map,
        Royal_Waterways_Map
    };

    #endregion

    #region Properties

    public static bool Colorless { get; set; }

    public static tk2dSprite Sprite => _sprite == null ? _sprite = HeroController.instance.GetComponent<tk2dSprite>() : _sprite;

    public int DisabledCharms
    {
        get
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = 0;
            return Convert.ToInt32(Data.AdditionalData);
        }
        set
        {
            if (Data.AdditionalData == null)
                Data.AdditionalData = 0;
            Data.AdditionalData = value;
        }
    }

    #endregion

    #region Event handler

    private void AbstractItem_ModifyItemGlobal(GiveEventArgs obj)
    {
        if (_cursedTouch)
        {
            // Spell upgrades can only be removed, if the player already has the normal spell
            bool hasBaseSpell = true;
            try
            {
                if (obj.Item.name == Vengeful_Spirit || obj.Item.name == Shade_Soul)
                    hasBaseSpell = PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel)) > 0;
                else if (obj.Item.name == Desolate_Dive || obj.Item.name == Descending_Dark)
                    hasBaseSpell = PlayerData.instance.GetInt(nameof(PlayerData.quakeLevel)) > 0;
                else if (obj.Item.name == Abyss_Shriek || obj.Item.name == Howling_Wraiths)
                    hasBaseSpell = PlayerData.instance.GetInt(nameof(PlayerData.screamLevel)) > 0;
                if (_curseableItems.Contains(obj.Item.name) && hasBaseSpell)
                {
                    if (obj.Item is ItemChanger.Items.CharmItem)
                        DisabledCharms++;
                    GameHelper.DisplayMessage("It turned to gold...?");
                    string itemName = obj.Item.UIDef is MsgUIDef msg ? msg.name.Value : "geo";
                    obj.Item = Finder.GetItem(Geo_Rock_Default);
                    (obj.Item.UIDef as MsgUIDef).name = new BoxedString("Gilded " + itemName);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Write<CurseRandomizer>("Couldn't transform item in gold.", exception);
            }
        }
    }

    private void HeroController_AddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        orig(self, amount);
        _safeTime = Mathf.Max(30, _safeTime + amount / 100f);
    }

    private void PlayerData_CountCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
    {
        orig(self);
        for (int i = 0; i < DisabledCharms; i++)
            PlayerData.instance.IncrementInt(nameof(PlayerData.charmsOwned));
    }

    #endregion

    #region Control

    public override void ApplyHooks()
    {
        CalculateReplaceableItems();
        AbstractItem.ModifyItemGlobal += AbstractItem_ModifyItemGlobal;
        On.HeroController.AddGeo += HeroController_AddGeo;
        On.PlayerData.CountCharms += PlayerData_CountCharms;
        if (Data.CastedAmount > 0)
            _coroutine = CurseManager.Handler.StartCoroutine(CursedTouch());
    }

    public override void Unhook()
    {
        AbstractItem.ModifyItemGlobal -= AbstractItem_ModifyItemGlobal;
        On.HeroController.AddGeo -= HeroController_AddGeo;
        On.PlayerData.CountCharms -= PlayerData_CountCharms;
    }

    public override void ApplyCurse()
    {
        if (_coroutine is null)
            _coroutine = CurseManager.Handler.StartCoroutine(CursedTouch());
        _safeTime = 15f;
    }

    public override int SetCap(int value) => Math.Max(Math.Min(value, 20), 1);

    public override void ResetAdditionalData() => Data.AdditionalData = 0;

    #endregion

    #region Methods

    private void CalculateReplaceableItems()
    {
        _curseableItems.Clear();
        _curseableItems.AddRange(_alwaysReplacable);

        // 1 Relics, 2 eggs, 3 pale ore
        (bool, bool, bool) used = new();
        foreach (AbstractPlacement placement in ItemChanger.Internal.Ref.Settings.Placements.Values)
            foreach (AbstractItem item in placement.Items)
            {
                if (placement is ISingleCostPlacement single)
                {
                    List<ConsumablePDIntCost> costs = new();
                    if (single.Cost?.GetBaseCost() is ConsumablePDIntCost consumablePDInt)
                        costs.Add(consumablePDInt);
                    else if (single.Cost?.GetBaseCost() is MultiCost multiCost)
                        costs = multiCost.Where(x => x.GetBaseCost() is ConsumablePDIntCost c)
                            .Select(x => x as ConsumablePDIntCost)
                            .ToList();

                    if (costs.Any(x => x.fieldName?.StartsWith("trinket") == true))
                        used.Item1 = true;
                    else if (costs.Any(x => x.fieldName == nameof(PlayerData.rancidEggs)))
                        used.Item2 = true;
                    else if (costs.Any(x => x.fieldName == nameof(PlayerData.ore)))
                        used.Item3 = true;
                    break;
                }
                else
                {
                    IEnumerable<CostTag> tags = item.tags?.Where(x => x is CostTag)
                        .Select(x => x as CostTag);
                    if (tags != null)
                        foreach (CostTag tag in tags)
                        {
                            List<ConsumablePDIntCost> costs = new();
                            if (tag.Cost.GetBaseCost() is ConsumablePDIntCost consumablePDInt)
                                costs.Add(consumablePDInt);
                            else if (tag.Cost.GetBaseCost() is MultiCost multiCost)
                                costs = multiCost.Where(x => x.GetBaseCost() is ConsumablePDIntCost c)
                                    .Select(x => x as ConsumablePDIntCost)
                                    .ToList();

                            if (costs.Any(x => x.fieldName?.StartsWith("trinket") == true))
                                used.Item1 = true;
                            else if (costs.Any(x => x.fieldName == nameof(PlayerData.rancidEggs)))
                                used.Item2 = true;
                            else if (costs.Any(x => x.fieldName == nameof(PlayerData.ore)))
                                used.Item3 = true;
                        }
                }
            }

        if (!used.Item1)
        {
            _curseableItems.Add(Hallownest_Seal);
            _curseableItems.Add(Wanderers_Journal);
            _curseableItems.Add(Kings_Idol);
            _curseableItems.Add(Arcane_Egg);
        }
        if (!used.Item2)
            _curseableItems.Add(Rancid_Egg);
        if (!(ItemChanger.Internal.Ref.Settings.Placements.ContainsKey("Nailsmith_Upgrade_1") || used.Item3))
            _curseableItems.Add(Pale_Ore);
        if (!ItemChanger.Internal.Ref.Settings.Placements.ContainsKey("Slug_In_Tub-Shrine"))
            _curseableItems.Add(Shape_of_Unn);
        if (!ItemChanger.Internal.Ref.Settings.Placements.ContainsKey("Longest_Nail-Shrine"))
        {
            _curseableItems.Add(Longnail);
            _curseableItems.Add(Mark_of_Pride);
        }
    }

    private IEnumerator CursedTouch()
    {
        _cursedTouch = false;
        while (true)
        {
            if (_safeTime > 0f)
            {
                _safeTime -= Time.deltaTime;
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => GameManager.instance != null && !GameManager.instance.IsGamePaused()
            && HeroController.instance?.CanInput() == true);
            if (UnityEngine.Random.Range(1, 201) <= Data.CastedAmount)
            {
                if (Colorless)
                    GameHelper.DisplayMessage("You are cursed...");
                _safeTime = UnityEngine.Random.Range(5, Math.Max(15, 31 - Data.CastedAmount));
                Sprite.color = Color.yellow;
                _cursedTouch = true;
                float goldenBoy = UnityEngine.Random.Range(1, 6);
                while (goldenBoy > 0f)
                {
                    goldenBoy -= Time.deltaTime;
                    yield return null;
                }
                yield return new WaitUntil(() => GameManager.instance != null && !GameManager.instance.IsGamePaused());
                Sprite.color = Color.white;
                if (Colorless)
                    GameHelper.DisplayMessage("You are no longer cursed...");
            }
            _cursedTouch = false;
        }
    }

    #endregion
}
