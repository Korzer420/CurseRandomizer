using CurseRandomizer.Randomizer;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static RandomizerMod.RC.RequestBuilder;

namespace CurseRandomizer;

internal static class RandoManager
{
    private static List<AbstractItem> _mimicableItems = new();
    private static List<Curse> _availableCurses = new();
    private static Random _generator;

    public static List<string> ReplacedItems { get; set; } = new();

    internal static void HookRando()
    {
        OnUpdate.Subscribe(9999f, ModifyRequest);
        AddMimicableItems();
        Finder.DefineCustomItem(new CurseItem()
        {
            name = "Fool_Item",
            tags = new()
        });
        //Finder.DefineCustomItem(new IntItem()
        //{
        //    name = "Wallet",
        //    amount = 1,
        //    fieldName = "geoWallet",
        //    UIDef = new MsgUIDef()
        //    {
        //        name = new BoxedString("Wallet"),
        //        shopDesc = new BoxedString("You may wanna buy this, so you can buy more from me later."),
        //        sprite = null
        //    }
        //});
        Finder.GetItemOverride += TranformCurseItems;
        RCData.RuntimeLogicOverride.Subscribe(9999f, ModifyLogic);
        RandomizerMenu.AttachMenu();
    }

    /// <summary>
    /// Apply the mimic properties and the curse.
    /// </summary>
    private static void TranformCurseItems(GetItemEventArgs requestedItemArgs)
    {
        try
        {
            if (requestedItemArgs.ItemName.StartsWith("Fool_Item") && CurseRandomizer.Instance.Settings.UseCurses)
            {
                AbstractItem mimicItem = _mimicableItems[_generator.Next(0, _mimicableItems.Count)];

                CurseItem curseItem = new()
                {
                    name = "Fake_" + mimicItem.name,
                    UIDef = mimicItem.GetResolvedUIDef().Clone()
                };
                if (curseItem.UIDef is BigUIDef bigScreen)
                {
                    bigScreen.take = new BoxedString("You are a:");
                    bigScreen.descOne = new BoxedString(string.Empty);
                    bigScreen.descTwo = new BoxedString(string.Empty);
                }
                else if (curseItem.UIDef is LoreUIDef lore)
                    lore.lore = new BoxedString("You are a FOOL");

                if (curseItem.UIDef is not MsgUIDef msgUIDef)
                    CurseRandomizer.Instance.LogError("Item " + mimicItem.name + " couldn't be mimicked correctly. UI Def has to be inhert from MsgUIDef");
                else if (MimicNames.Mimics.ContainsKey(mimicItem.name))
                    msgUIDef.name = new BoxedString(MimicNames.Mimics[mimicItem.name][_generator.Next(0, MimicNames.Mimics[mimicItem.name].Length)]);
                else
                {
                    (mimicItem.tags.First(x => x is IInteropTag) as IInteropTag).TryGetProperty("MimicNames", out string[] mimicNames);
                    if (mimicNames == null || !mimicNames.Any())
                        CurseRandomizer.Instance.LogError("Couldn't find a mimic name.");
                    else
                        msgUIDef.name = new BoxedString(mimicNames[_generator.Next(0, mimicNames.Length)]);
                }
                curseItem.CurseName = _availableCurses[_generator.Next(0, _availableCurses.Count)].Name;
                requestedItemArgs.Current = curseItem;
            }
        }
        catch (Exception exception)
        {
            throw new Exception("Couldn't resolve item "+exception.StackTrace);
        }
    }

    private static void ModifyRequest(RequestBuilder builder)
    {
        _generator = new(builder.gs.Seed);
        ReplacedItems.Clear();

        // Handles the placed curses.
        if (CurseRandomizer.Instance.Settings.UseCurses)
        {
            // Get all items which can be removed.
            List<string> replacableItems = new();
            foreach (PoolDef pool in Data.Pools)
                switch (pool.Name)
                {
                    case "Mask" when CurseRandomizer.Instance.Settings.MaskShards && builder.gs.PoolSettings.MaskShards:
                    case "CursedMask" when CurseRandomizer.Instance.Settings.MaskShards && builder.gs.CursedSettings.CursedMasks > 0:
                    case "Vessel" when CurseRandomizer.Instance.Settings.VesselFragments && builder.gs.PoolSettings.VesselFragments:
                    case "Ore" when CurseRandomizer.Instance.Settings.PaleOre && builder.gs.PoolSettings.PaleOre:
                    case "Notch" when CurseRandomizer.Instance.Settings.Notches && builder.gs.PoolSettings.CharmNotches:
                    case "CursedNotch" when CurseRandomizer.Instance.Settings.Notches && builder.gs.CursedSettings.CursedNotches > 0:
                    case "Relic" when CurseRandomizer.Instance.Settings.Relics && builder.gs.PoolSettings.Relics:
                    case "Rock" when CurseRandomizer.Instance.Settings.Rocks && builder.gs.PoolSettings.GeoRocks:
                    case "Geo" when CurseRandomizer.Instance.Settings.Geo && builder.gs.PoolSettings.GeoChests:
                    case "Boss_Geo" when CurseRandomizer.Instance.Settings.Geo && builder.gs.PoolSettings.BossGeo:
                    case "Soul" when CurseRandomizer.Instance.Settings.Totems && builder.gs.PoolSettings.SoulTotems:
                    case "PalaceSoul" when CurseRandomizer.Instance.Settings.Totems && builder.gs.PoolSettings.SoulTotems && builder.gs.LongLocationSettings.WhitePalaceRando != RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludeWhitePalace:
                        replacableItems.AddRange(pool.IncludeItems);
                        break;
                }

            // Get all pools, which the items can be removed from.
            List<ItemGroupBuilder> availablePools = new();
            foreach (StageBuilder stage in builder.Stages)
                foreach (ItemGroupBuilder itemGroup in stage.Groups.Where(x => x is ItemGroupBuilder).Select(x => x as ItemGroupBuilder))
                {
                    if (availablePools.Contains(itemGroup))
                        continue;
                    foreach (string item in itemGroup.Items.EnumerateDistinct())
                        if (replacableItems.Contains(item))
                        {
                            availablePools.Add(itemGroup);
                            break;
                        }
                }
            _availableCurses.Clear();
            if (CurseRandomizer.Instance.Settings.CustomCurses && CurseManager.GetCurseByType(CurseType.Custom) != null)
                _availableCurses.AddRange(CurseManager.GetCurses().Where(x => x.Type == CurseType.Custom));
            if (CurseRandomizer.Instance.Settings.PainCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Pain));
            if (CurseRandomizer.Instance.Settings.GreedCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Greed));
            if (CurseRandomizer.Instance.Settings.StupidityCurse && !builder.gs.SkipSettings.FireballSkips)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Stupidity));
            if (CurseRandomizer.Instance.Settings.NormalityCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Normality));
            if (CurseRandomizer.Instance.Settings.DesorientationCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Desorientation));
            if (CurseRandomizer.Instance.Settings.EmptynessCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Emptyness));
            if (CurseRandomizer.Instance.Settings.ThirstCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Thirst));
            if (CurseRandomizer.Instance.Settings.WeaknessCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Weakness));
            if (CurseRandomizer.Instance.Settings.LoseCurse)
                _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Lose));

            int amount = CurseRandomizer.Instance.Settings.CurseAmount switch
            {
                Amount.Few => builder.rng.Next(1, 6),
                Amount.Some => builder.rng.Next(3, 11),
                Amount.Medium => builder.rng.Next(5, 16),
                Amount.Many => builder.rng.Next(7, 21),
                Amount.OhOh => builder.rng.Next(10, 31),
                Amount.Custom => CurseRandomizer.Instance.Settings.CurseItems,
                _ => 0
            };
            if (CurseRandomizer.Instance.Settings.CurseMethod != RequestMethod.Add)
                // Remove the items.
                for (; amount > 0; amount--)
                {
                    if (!availablePools.Any())
                        break;
                    ItemGroupBuilder pickedGroup = availablePools[builder.rng.Next(0, availablePools.Count)];
                    string[] availableItems = pickedGroup.Items.EnumerateDistinct().Where(x => replacableItems.Contains(x)).ToArray();
                    string pickedItem = availableItems[builder.rng.Next(0, availableItems.Length)];
                    pickedGroup.Items.Remove(pickedItem, 1);
                    ReplacedItems.Add(pickedItem);
                    if (availableItems.Length == 0)
                        availablePools.Remove(pickedGroup);
                    if (pickedItem == ItemNames.Mask_Shard)
                        builder.AddItemByName("Fool_Item_Mocked_Shard");
                    else if (pickedItem == ItemNames.Double_Mask_Shard)
                        builder.AddItemByName("Fool_Item_Two_Mocked_Shards");
                    else if (pickedItem == ItemNames.Full_Mask)
                        builder.AddItemByName("Fool_Item_Mocked_Mask");
                    else
                        builder.AddItemByName("Fool_Item");
                }

            if (amount > 0 && CurseRandomizer.Instance.Settings.CurseMethod != RequestMethod.ForceReplace)
                builder.AddItemByName("Fool_Item", amount);
        }

        //if (CurseRandomizer.Instance.Settings.CursedWallet)
        //    builder.AddItemByName("Wallet", 4);
    }

    private static void AddMimicableItems()
    {
        CurseRandomizer.Instance.LogDebug("Add mimic items");
        foreach (string item in MimicNames.Mimics.Keys)
            _mimicableItems.Add(Finder.GetItem(item));
        foreach (KeyValuePair<string, AbstractItem> item in Finder.GetFullItemList().Where(x => x.Value.tags != null && x.Value.tags.Any(x => x is IInteropTag)))
        {
            if (_mimicableItems.Contains(item.Value))
                continue;
            // Other connections can add mimics via an interop tag, here we get all items which are eligatible to be mimicked.
            try
            {
                if ((item.Value.tags.First(x => x is IInteropTag) as IInteropTag).TryGetProperty("MimicNames", out string[] mimicNames))
                    _mimicableItems.Add(item.Value);
            }
            catch (Exception exception)
            {
                CurseRandomizer.Instance.LogError("Couldn't add mimics for item " + item.Key + ": " + exception.Message);
                CurseRandomizer.Instance.LogError(exception.StackTrace);
            }
        }
        CurseRandomizer.Instance.LogDebug("Completed setting up mimic names.");
    }

    private static void ModifyLogic(GenerationSettings settings, LogicManagerBuilder builder)
    {
        builder.AddItem(new EmptyItem("Fool_Item"));
        builder.AddItem(new SingleItem("Fool_Item_Mocked_Shard", new(builder.GetTerm("MASKSHARDS"), 1)));
        builder.AddItem(new SingleItem("Fool_Item_Two_Mocked_Shards", new(builder.GetTerm("MASKSHARDS"), 2)));
        builder.AddItem(new SingleItem("Fool_Item_Mocked_Mask", new(builder.GetTerm("MASKSHARDS"), 4)));
    }
}
