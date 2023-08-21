using CurseRandomizer.Curses;
using CurseRandomizer.ItemData;
using CurseRandomizer.Manager;
using CurseRandomizer.ModInterop.MoreLocations;
using CurseRandomizer.Randomizer;
using CurseRandomizer.Randomizer.Settings;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using KorzUtils.Helper;
using Modding;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringLogic;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ItemChanger.Internal.SpriteManager;
using static RandomizerMod.RC.RequestBuilder;
using static RandomizerMod.Settings.MiscSettings;

namespace CurseRandomizer;

internal static class RandoManager
{
    #region Constants

    public const string Geo_Wallet = "Geo_Wallet";
    public const string Dreamnail_Fragment = "Dreamnail_Fragment";

    #endregion

    #region Members

    private static List<(string, float)> _mimicableItems = new();
    private static List<Curse> _availableCurses = new();
    private static Random _generator;

    #endregion

    #region Properties

    /// <summary>
    /// Contains all items which the curse randomizer replaced
    /// </summary>
    public static List<string> ReplacedItems { get; set; } = new();

    #endregion

    #region Setup

    internal static void DefineItemChangerData()
    {
        Finder.DefineCustomItem(new CurseItem()
        {
            name = CurseItem.CursePrefix,
            tags = new()
        });

        Finder.DefineCustomItem(new WalletItem()
        {
            name = Geo_Wallet,
            tags = new()
            {
                new InteropTag()
                {
                    Message = "CurseData",
                     Properties = new()
                     {
                         { "MimicNames", new string[] {"Wallet", "Moneybag", "Ge0 Wallet" } },
                         { "CanMimic",  new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet) }
                     }
                },
                new InteropTag()
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        {"IsMajorItem", true },
                        {"MajorItemName", Geo_Wallet }
                    }
                }
            },
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Geo Wallet"),
                shopDesc = new BoxedString("Seems kinda useful to me."),
                sprite = new CustomSprite("Wallet")
            }
        });

        Finder.DefineCustomItem(new DreamFragmentItem()
        {
            name = Dreamnail_Fragment,
            tags = new()
            {
                new InteropTag()
                {
                    Message = "CurseData",
                     Properties = new()
                     {
                         {"MimicNames", new string[] {"Dreem Nail Fragment", "Dream Nayl Fragment", "Dream Nai1 Fragment"} },
                         {"CanMimic", new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedDreamNail) }
                     }
                },
                new InteropTag()
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        {"IsMajorItem", true },
                        {"MajorItemName", Dreamnail_Fragment }
                    }
                }
            },
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Fight Fragment"),
                shopDesc = new BoxedString("You can insert this into your dreamnail to restore a part of it's original power."),
                sprite = (Finder.GetItem(ItemNames.Dream_Nail).UIDef as MsgUIDef).sprite
            }
        });

        // Special item to always get rid of curses that require geo to spend
        Finder.DefineCustomItem(new IntItem()
        {
            name = "Generosity",
            amount = 1,
            fieldName = "Generosity",
            tags = new()
            {
                new PDBoolShopRemoveTag()
                {
                    fieldName = "HasRegrets",
                    removeVal = false
                },
                new CostTag()
                {
                    Cost = new GeoCost(200)
                    {
                        Recurring = true
                    }
                },
                new PersistentItemTag()
                {
                    Persistence = Persistence.Persistent
                }
            },
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Generosity"),
                shopDesc = new BoxedString("If this does cleanse you from your past regrets is beyond me. But I wouldn't mind having more geo."),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.One_Geo).GetResolvedUIDef().GetSprite())
            }
        });
    }

    internal static void HookRando()
    {
        DefineItemChangerData();
        OnUpdate.Subscribe(-400f, CreateMatcher);
        OnUpdate.Subscribe(40f, ApplySettings);
        OnUpdate.Subscribe(9999f, ApplyCurses);
        Finder.GetItemOverride += TranformCurseItems;
        RCData.RuntimeLogicOverride.Subscribe(9999f, ModifyLogic);
        RandoController.OnCalculateHash += RandoController_OnCalculateHash;
        RandomizerMenu.AttachMenu();
        SettingsLog.AfterLogSettings += WriteCurseRandoSettings;
        ProgressionInitializer.OnCreateProgressionInitializer += SetupVesselTerm;

        if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            HookRandoSettingsManager();

        if (ModHooks.GetMod("FStatsMod") is Mod)
            HookFStats();

        if (ModHooks.GetMod("MoreLocations") is Mod)
            MoreLocationsInterop.Hook();
    }

    private static void SetupVesselTerm(LogicManager logicManager, GenerationSettings generationSettings, ProgressionInitializer progressionInitializer)
    {
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
            return;
        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel > 0)
            progressionInitializer.Increments.Add(new(logicManager.GetTerm("VESSELFRAGMENTS"), 6 - CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel * 3));
        if (CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses && CurseRandomizer.Instance.Settings.CurseControlSettings.Bargains)
            progressionInitializer.Increments.Add(new(logicManager.GetTerm("TAKECURSE"), 4));
    }

    private static void WriteCurseRandoSettings(LogArguments args, TextWriter textWriter)
    {
        textWriter.WriteLine("Curse Randomizer settings:");
        using Newtonsoft.Json.JsonTextWriter jsonTextWriter = new(textWriter) { CloseOutput = false, };
        JsonUtil._js.Serialize(jsonTextWriter, CurseRandomizer.Instance.Settings);
        textWriter.WriteLine();
    }

    private static void HookRandoSettingsManager()
    {
        RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<RandoSettings>(CurseRandomizer.Instance,
        RandomizerMenu.Instance.UpdateMenuSettings,
        () => CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? CurseRandomizer.Instance.Settings : null));
    }

    private static void HookFStats() => CurseStats.HookFStats();

    private static int RandoController_OnCalculateHash(RandoController controller, int hashValue)
    {
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled || !CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
            return 0;
        int addition = 0;
        if (CurseRandomizer.Instance.Settings.CurseControlSettings.PerfectMimics)
            addition += 410;
        if (CurseRandomizer.Instance.Settings.CurseControlSettings.CapEffects)
            addition++;

        foreach (Curse curse in _availableCurses)
        {
            addition += 120 * (int)curse.Type;
            addition += 5 * curse.Cap;
        }
        addition += (int)CurseManager.DefaultCurse.Type * 420;
        addition += (CurseManager.GetCurses().Select(x => x.Name).IndexOf(CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse) + 1) * 777;

        return 24691 + addition;
    }

    /// <summary>
    /// Apply the mimic properties and the curse to the requested items.
    /// </summary>
    private static void TranformCurseItems(GetItemEventArgs requestedItemArgs)
    {
        AbstractItem itemToMimic = null;
        try
        {
            if (requestedItemArgs.ItemName == "OMEN")
                requestedItemArgs.Current = new CurseItem()
                {
                    CurseName = "Omen",
                    name = "Omen",
                    UIDef = new MsgUIDef()
                    {
                        name = new BoxedString("Omen"),
                        sprite = new CustomSprite("Fool"),
                        shopDesc = new BoxedString("If you can read this, something went wrong")
                    }
                };
            else if (requestedItemArgs.ItemName.StartsWith(CurseItem.CursePrefix) && CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
            {
                if (requestedItemArgs.ItemName.Contains("Evil"))
                    itemToMimic = Finder.GetItem(RollMimic());
                else
                    itemToMimic = Finder.GetItem(requestedItemArgs.ItemName.Substring(CurseItem.CursePrefix.Length));
                if (itemToMimic == null)
                {
                    CurseRandomizer.Instance.LogError("Tried to replicate unknown item: " + requestedItemArgs.ItemName);
                    return;
                }
                CurseRandomizer.Instance.LogDebug("Try to replicate: " + itemToMimic.name);
                CurseItem curseItem = new()
                {
                    name = requestedItemArgs.ItemName,
                    UIDef = itemToMimic.GetResolvedUIDef().Clone()
                };
                if (curseItem.UIDef is BigUIDef bigScreen)
                {
                    bigScreen.take = new BoxedString("You are a:");
                    bigScreen.descOne = new BoxedString(string.Empty);
                    bigScreen.descTwo = new BoxedString(string.Empty);
                }
                else if (curseItem.UIDef is LoreUIDef lore)
                    lore.lore = new BoxedString("You are a FOOL");

                if (!CurseRandomizer.Instance.Settings.CurseControlSettings.PerfectMimics)
                {
                    if (curseItem.UIDef is not MsgUIDef msgUIDef)
                        CurseRandomizer.Instance.LogError("Item " + itemToMimic.name + " couldn't be mimicked correctly. UI Def has to be inhert from MsgUIDef.");
                    else if (MimicNames.Mimics.ContainsKey(itemToMimic.name))
                        msgUIDef.name = new BoxedString(MimicNames.Mimics[itemToMimic.name][_generator.Next(0, MimicNames.Mimics[itemToMimic.name].Length)]);
                    else
                    {
                        if (itemToMimic.tags.FirstOrDefault(x => x is IInteropTag tag && tag.Message == "CurseData") is not IInteropTag interopTag)
                            CurseRandomizer.Instance.LogWarn("Couldn't find interop tag on requested item: " + itemToMimic.name + ". Take normal name instead.");
                        else
                        {
                            interopTag.TryGetProperty("MimicNames", out string[] mimicNames);
                            if (mimicNames == null || !mimicNames.Any())
                                CurseRandomizer.Instance.LogError("Couldn't find a mimic name. Will take the normal UI name of: " + itemToMimic.name);
                            else
                                msgUIDef.name = new BoxedString(mimicNames[_generator.Next(0, mimicNames.Length)]);
                        }
                    }
                }
                else
                {
                    // If perfect mimics is enabled, skills and items marked as major are 
                    InteropTag tag = itemToMimic.tags?.FirstOrDefault(x => x is IInteropTag interop && interop.Message == "RandoSupplementalMetadata") as InteropTag;
                    if (MimicNames.IsMajorItem(itemToMimic.name) || (tag != null && tag.TryGetProperty("IsMajorItem", out bool isMajor)))
                        curseItem.AddTag(new InteropTag()
                        {
                            Message = "RandoSupplementalMetadata",
                            Properties = new()
                            {
                                {"IsMajorItem", true },
                                {"MajorItemName", requestedItemArgs.ItemName }
                            }
                        });
                }
                curseItem.CurseName = _availableCurses[_generator.Next(0, _availableCurses.Count)].Name;
                requestedItemArgs.Current = curseItem;
            }
        }
        catch (ArgumentOutOfRangeException outOfRange)
        {
            CurseRandomizer.Instance.LogError("Couldn't found any mimickable items. Make sure that at least one item exists.");
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.LogError($"Couldn't transform curse item: {itemToMimic?.name}" + exception.StackTrace);
        }
    }

    /// <summary>
    /// Managed the logic items for curse items.
    /// </summary>
    /// <param name="builder"></param>
    private static void CreateMatcher(RequestBuilder builder)
    {
        bool TryMatch(string name, out ItemRequestInfo info)
        {
            if (name.StartsWith(CurseItem.CursePrefix))
            {
                string innerItem = name.Substring(CurseItem.CursePrefix.Length);
                info = new ItemRequestInfo
                {
                    randoItemCreator = factory => new RandoModItem() { item = new EmptyItem(name) },
                    getItemDef = () => builder.TryGetItemDef(innerItem, out ItemDef def) ? def : null,
                };
                return true;
            }
            else
            {
                info = default;
                return false;
            }
        }
        builder.ItemMatchers.Add(TryMatch);
        builder.OnGetGroupFor.Subscribe(150f, MimickGroupResolve);
    }

    private static bool MimickGroupResolve(RequestBuilder builder, string item, ElementType elementType, out GroupBuilder groupBuilder)
    {
        if ((elementType == ElementType.Item || elementType == ElementType.Unknown) && item.StartsWith(CurseItem.CursePrefix))
        {
            if (item.Contains("Evil"))
                groupBuilder = builder.GetItemGroupFor(item.Substring(CurseItem.CursePrefix.Length + "Evil_".Length));
            else
                groupBuilder = builder.GetItemGroupFor(item.Substring(CurseItem.CursePrefix.Length));
            CurseRandomizer.Instance.LogDebug("Group builder is: " + groupBuilder.label);
            return true;
        }
        groupBuilder = default;
        return false;
    }

    /// <summary>
    /// Handles all settings besides the actual curses/mimics.
    /// </summary>
    /// <param name="builder"></param>
    private static void ApplySettings(RequestBuilder builder)
    {
        ModManager.WalletCapacities = Array.Empty<int>();
        _generator = new(builder.gs.Seed);

        // In case wallets are added through other means, we still check for wallets, even if the mod is disabled.
        if (CurseRandomizer.Instance.Settings.GeneralSettings.Enabled && CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet)
            builder.AddItemByName("Geo_Wallet", 4);

        int walletAmounts = 0;
        foreach (ItemGroupBuilder group in builder.EnumerateItemGroups())
            walletAmounts += group.Items.GetCount("Geo_Wallet");
        builder.CostConverters.Subscribe(420f, CostConverter);
        if (walletAmounts > 0)
        {
            ModManager.WalletCapacities = new int[walletAmounts];
            for (int i = 0; i < walletAmounts; i++)
                ModManager.WalletCapacities[i] = 500 + 500 * i;

            builder.EditLocationRequest(LocationNames.Sly, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest(LocationNames.Sly_Key, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest(LocationNames.Salubra, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest(LocationNames.Iselda, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest(LocationNames.Leg_Eater, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest("Salubra_(Requires_Charms)", info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest(LocationNames.Lemm, info => ModifyShopsForWallets(info, walletAmounts, builder));
            builder.EditLocationRequest("Junk_Shop", info => ModifyShopsForWallets(info, walletAmounts, builder));
        }

        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
            return;

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedDreamNail)
            builder.AddItemByName(Dreamnail_Fragment, 2);

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel != 0)
        {
            if (builder.gs.MiscSettings.VesselFragments == VesselFragmentType.OneFragmentPerVessel)
                builder.AddItemByName(ItemNames.Full_Soul_Vessel, CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel);
            else if (builder.gs.MiscSettings.VesselFragments == VesselFragmentType.TwoFragmentsPerVessel)
            {
                if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel == 2)
                    builder.AddItemByName(ItemNames.Double_Vessel_Fragment, 3);
                else
                {
                    builder.AddItemByName(ItemNames.Double_Vessel_Fragment, 1);
                    builder.AddItemByName(ItemNames.Vessel_Fragment, 1);
                }
            }
            else
                builder.AddItemByName(ItemNames.Vessel_Fragment, CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel * 3);
        }
    }

    /// <summary>
    /// Check and add the default items to logic
    /// </summary>
    /// <param name="builder"></param>
    private static void AddShopDefaultItems(RequestBuilder builder)
    {
        if (!builder.gs.PoolSettings.Charms)
        {
            // Salubra charms.
            builder.AddToVanilla(ItemNames.Lifeblood_Heart, "Salubra_Medium"); // 250
            builder.AddToVanilla(ItemNames.Longnail, "Salubra_Medium"); //300
            builder.AddToVanilla(ItemNames.Steady_Body, "Salubra_Cheap"); // 120
            builder.AddToVanilla(ItemNames.Shaman_Stone, "Salubra_Medium"); //220
            builder.AddToVanilla(ItemNames.Quick_Focus, "Salubra_Expensive"); //800
            builder.AddToVanilla(ItemNames.Salubras_Blessing, "Salubra_Expensive"); // 800

            // Iselda charms.
            builder.AddToVanilla(ItemNames.Wayward_Compass, "Iselda_Medium"); //220

            // Sly charms.
            builder.AddToVanilla(ItemNames.Gathering_Swarm, "Sly_Medium"); //300
            builder.AddToVanilla(ItemNames.Stalwart_Shell, "Sly_Cheap"); //200
            builder.AddToVanilla(ItemNames.Heavy_Blow, "Sly_(Key)_Medium"); //350
            builder.AddToVanilla(ItemNames.Sprintmaster, "Sly_(Key)_Medium"); //400

            // Leg eater charms.
            builder.AddToVanilla(ItemNames.Fragile_Heart, "Leg_Eater_Medium"); // 350
            //builder.AddToVanilla(ItemNames.Fragile_Heart_Repair, "Leg_Eater_Cheap"); // 200
            builder.AddToVanilla(ItemNames.Fragile_Greed, "Leg_Eater_Medium"); //250
            //builder.AddToVanilla(ItemNames.Fragile_Greed_Repair, "Leg_Eater_Cheap"); // 150
            builder.AddToVanilla(ItemNames.Fragile_Strength, "Leg_Eater_Expensive"); // 600
            //builder.AddToVanilla(ItemNames.Fragile_Strength_Repair, "Leg_Eater_Medium"); // 350
        }

        if (builder.gs.MiscSettings.SalubraNotches == SalubraNotchesSetting.Vanilla)
        {
            builder.AddToVanilla(ItemNames.Charm_Notch, "Salubra_Cheap");
            builder.AddToVanilla(ItemNames.Charm_Notch, "Salubra_Medium");
            builder.AddToVanilla(ItemNames.Charm_Notch, "Salubra_Expensive");
            builder.AddToVanilla(ItemNames.Charm_Notch, "Salubra_Extreme_Valuable");
        }

        if (!builder.gs.PoolSettings.Keys)
        {
            builder.AddToVanilla(ItemNames.Simple_Key, "Sly_Expensive");
            builder.AddToVanilla(ItemNames.Elegant_Key, "Sly_(Key)_Expensive");
        }

        if (!builder.gs.PoolSettings.MaskShards)
        {
            builder.AddToVanilla(ItemNames.Mask_Shard, "Sly_Cheap");
            builder.AddToVanilla(ItemNames.Mask_Shard, "Sly_Medium");
            builder.AddToVanilla(ItemNames.Mask_Shard, "Sly_(Key)_Expensive");
            builder.AddToVanilla(ItemNames.Mask_Shard, "Sly_(Key)_Extreme_Valuable");
        }

        if (!builder.gs.PoolSettings.VesselFragments)
        {
            builder.AddToVanilla(ItemNames.Vessel_Fragment, "Sly_Expensive");
            builder.AddToVanilla(ItemNames.Vessel_Fragment, "Sly_(Key)_Expensive");
        }

        if (!builder.gs.PoolSettings.Keys)
            builder.AddToVanilla(ItemNames.Lumafly_Lantern, "Sly_Extreme_Valuable");

        if (!builder.gs.PoolSettings.RancidEggs)
            builder.AddToVanilla(ItemNames.Rancid_Egg, "Sly_Cheap");

        if (!builder.gs.PoolSettings.Maps)
            builder.AddToVanilla(ItemNames.Quill, "Iselda_Cheap");

        builder.RemoveFromVanilla(LocationNames.Sly);
        builder.RemoveFromVanilla(LocationNames.Sly_Key);
        builder.RemoveFromVanilla(LocationNames.Leg_Eater);
        builder.RemoveFromVanilla(LocationNames.Salubra);
        builder.RemoveFromVanilla(LocationNames.Salubra + "_(Requires_Charms)");
        builder.RemoveFromVanilla(LocationNames.Iselda);
    }

    /// <summary>
    /// Evaluates the curse settings and prepares IC for potential mimics.
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="Exception"></exception>
    private static void ApplyCurses(RequestBuilder builder)
    {
        ReplacedItems.Clear();
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled || !CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
        {
            OmenCurse.OmenMode = false;
            return;
        }
        OmenCurse.OmenMode = CurseRandomizer.Instance.Settings.CurseControlSettings.OmenMode;

        // Get all items which can be removed.
        // Also check the total amount of items.
        List<string> replacableItems = GetReplaceableItems(builder);
        int totalItemCount = 0;
        List<ItemGroupBuilder> availablePools = new();
        foreach (StageBuilder stage in builder.Stages)
            foreach (ItemGroupBuilder itemGroup in stage.Groups.Where(x => x is ItemGroupBuilder).Select(x => x as ItemGroupBuilder))
            {
                totalItemCount += itemGroup.Items.GetTotal();
                if (availablePools.Contains(itemGroup))
                    continue;
                foreach (string item in itemGroup.Items.EnumerateDistinct())
                    if (replacableItems.Contains(item))
                    {
                        availablePools.Add(itemGroup);
                        break;
                    }
            }

        // Check all curses that can be used.
        _availableCurses.Clear();
        CurseManager.DefaultCurse = null;
        foreach (CurseSettings settings in CurseRandomizer.Instance.Settings.CurseSettings)
            if (CurseManager.GetCurseByName(settings.Name) is Curse curse)
            {
                if (curse.Type != CurseType.Custom || CurseRandomizer.Instance.Settings.CurseControlSettings.CustomCurses)
                {
                    curse.Data.Active = settings.Active;
                    curse.Data.Cap = settings.Cap;
                    if (settings.Active)
                        _availableCurses.Add(curse);
                }
                if (settings.Name == CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse && settings.Active)
                    CurseManager.DefaultCurse = curse;
            }

        if (!_availableCurses.Any())
            throw new Exception("No curses available to place.");

        // If for some reason the default curse is not active, we just select the curse of pain.
        CurseManager.DefaultCurse ??= CurseManager.GetCurse<PainCurse>();

        CurseRandomizer.Instance.LogDebug("Total amount of items is: " + totalItemCount);
        // Get the amount of curses to be placed.
        int amount = CurseRandomizer.Instance.Settings.CurseControlSettings.CurseAmount switch
        {
            Amount.Few => builder.rng.Next(Math.Min(3, totalItemCount / 100 * 1), Math.Max(5, totalItemCount / 100 * 3)),
            Amount.Some => builder.rng.Next(Math.Min(5, totalItemCount / 100 * 4), Math.Max(10, totalItemCount / 100 * 6)),
            Amount.Medium => builder.rng.Next(Math.Min(10, totalItemCount / 100 * 7), Math.Max(15, totalItemCount / 100 * 9)),
            Amount.Many => builder.rng.Next(Math.Min(15, totalItemCount / 100 * 10), Math.Max(20, totalItemCount / 100 * 12)),
            Amount.OhOh => builder.rng.Next(Math.Min(20, totalItemCount / 100 * 13), Math.Max(30, totalItemCount / 100 * 15)),
            Amount.Custom => CurseRandomizer.Instance.Settings.CurseControlSettings.CurseItems,
            _ => 0
        };
        CurseRandomizer.Instance.LogDebug("Amount of curses is: " + amount);

        AddMimickableItems(builder);

        // Since system random doesn't support float values (above 1) we manipulate the random seed of unity to ensure all players have the same mimics.
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(builder.gs.Seed);

        if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.Add)
            // Remove the items.
            for (; amount > 0; amount--)
            {
                if (!availablePools.Any())
                    break;
                ItemGroupBuilder pickedGroup = availablePools[builder.rng.Next(0, availablePools.Count)];
                string[] availableItems = pickedGroup.Items.EnumerateDistinct().Where(replacableItems.Contains).ToArray();

                // Just in case no items could be found in the groups.
                if (availableItems.Length == 0)
                {
                    availablePools.Remove(pickedGroup);
                    amount++;
                    if (!availablePools.Any())
                    {
                        CurseRandomizer.Instance.LogError("No pools available, couldn't place curses.");
                        break;
                    }
                }
                string pickedItem = availableItems[builder.rng.Next(0, availableItems.Length)];
                pickedGroup.Items.Remove(pickedItem, 1);
                ReplacedItems.Add(pickedItem);
                if (availableItems.Length == 0)
                    availablePools.Remove(pickedGroup);
                string itemToMimic = CurseRandomizer.Instance.Settings.CurseControlSettings.TakeReplaceGroup 
                    ? "Evil_" + pickedItem
                    : RollMimic();
                builder.AddItemByName(CurseItem.CursePrefix + itemToMimic);
                CurseRandomizer.Instance.LogDebug("Removed " + pickedItem + " for a curse.");
            }

        if (amount > 0 && CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.ForceReplace)
            for (; amount > 0; amount--)
                builder.AddItemByName(CurseItem.CursePrefix + RollMimic());
        else if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod == RequestMethod.ForceReplace && amount > 0)
            CurseRandomizer.Instance.LogWarn("Couldn't replace enough items to satisfy the selected amount. Disposed amount: " + amount);

        if (CurseRandomizer.Instance.Settings.CurseControlSettings.Bargains)
        {
            builder.EditLocationRequest(LocationNames.Sly, info => ModifyShopsForCurses(info, builder));
            builder.EditLocationRequest(LocationNames.Sly_Key, info => ModifyShopsForCurses(info, builder));
            builder.EditLocationRequest(LocationNames.Salubra, info => ModifyShopsForCurses(info, builder));
            builder.EditLocationRequest(LocationNames.Iselda, info => ModifyShopsForCurses(info, builder));
            builder.EditLocationRequest(LocationNames.Leg_Eater, info => ModifyShopsForCurses(info, builder));
            builder.EditLocationRequest("Salubra_(Requires_Charms)", info => ModifyShopsForCurses(info, builder));
            //builder.EditLocationRequest(LocationNames.Lemm, info => ModifyShopsForWallets(info, walletAmounts, builder));
            //builder.EditLocationRequest("Junk_Shop", info => ModifyShopsForWallets(info, walletAmounts, builder));
        }
        // Reset the unity random state.
        UnityEngine.Random.state = state;
        if (CurseRandomizer.Instance.Settings.CurseControlSettings.OmenMode)
            builder.AddToStart("OMEN");
    }

    /// <summary>
    /// Adds the base mimic set which can be used, depending on the settings.
    /// </summary>
    /// <param name="settings"></param>
    private static void AddBaseMimics(GenerationSettings settings)
    {
        if (settings.PoolSettings.Skills)
        {
            // Claw
            if (settings.NoveltySettings.SplitClaw)
            {
                _mimicableItems.Add(new(ItemNames.Left_Mantis_Claw, 1f));
                _mimicableItems.Add(new(ItemNames.Right_Mantis_Claw, 1f));
            }
            else
                _mimicableItems.Add(new(ItemNames.Mantis_Claw, 1f));

            // Dash
            if (settings.NoveltySettings.SplitCloak)
            {
                _mimicableItems.Add(new(ItemNames.Left_Mothwing_Cloak, 1f));
                _mimicableItems.Add(new(ItemNames.Right_Mothwing_Cloak, 1f));
            }
            else
                _mimicableItems.Add(new(ItemNames.Mothwing_Cloak, 1f));
            _mimicableItems.Add(new(ItemNames.Shade_Cloak, 1f));

            // Crystal Dash
            if (settings.NoveltySettings.SplitSuperdash)
            {
                _mimicableItems.Add(new(ItemNames.Left_Crystal_Heart, 1f));
                _mimicableItems.Add(new(ItemNames.Right_Crystal_Heart, 1f));
            }
            else
                _mimicableItems.Add(new(ItemNames.Crystal_Heart, 1f));

            // Spells
            _mimicableItems.Add(new(ItemNames.Howling_Wraiths, 1f));
            _mimicableItems.Add(new(ItemNames.Descending_Dark, 1f));
            _mimicableItems.Add(new(ItemNames.Vengeful_Spirit, 1f));
            _mimicableItems.Add(new(ItemNames.Shade_Soul, 1f));
            _mimicableItems.Add(new(ItemNames.Desolate_Dive, 1f));
            _mimicableItems.Add(new(ItemNames.Abyss_Shriek, 1f));

            // Nail arts
            _mimicableItems.Add(new(ItemNames.Great_Slash, 1f));
            _mimicableItems.Add(new(ItemNames.Cyclone_Slash, 1f));
            _mimicableItems.Add(new(ItemNames.Dash_Slash, 1f));

            // Misc
            _mimicableItems.Add(new(ItemNames.Dream_Nail, 1f));
            _mimicableItems.Add(new(ItemNames.Dream_Gate, 1f));
            _mimicableItems.Add(new(ItemNames.Awoken_Dream_Nail, 1f));
            _mimicableItems.Add(new(ItemNames.Ismas_Tear, 1f));
            _mimicableItems.Add(new(ItemNames.Monarch_Wings, 1f));
        }

        if (settings.PoolSettings.Keys)
        {
            _mimicableItems.Add(new(ItemNames.Tram_Pass, 1f));
            _mimicableItems.Add(new(ItemNames.Simple_Key, .5f));
            _mimicableItems.Add(new(ItemNames.Elegant_Key, 1f));
            _mimicableItems.Add(new(ItemNames.Love_Key, 1f));
            _mimicableItems.Add(new(ItemNames.Kings_Brand, 1f));
            _mimicableItems.Add(new(ItemNames.Lumafly_Lantern, 1f));
            _mimicableItems.Add(new(ItemNames.City_Crest, .5f));
        }

        if (settings.PoolSettings.Charms)
            foreach (string charmName in MimicNames.Mimics.SkipWhile(x => x.Key != ItemNames.Awoken_Dream_Nail).Skip(1).TakeWhile(x => x.Key != ItemNames.Void_Heart).Take(1).Select(x => x.Key))
                _mimicableItems.Add(new(charmName, .75f));

        if (settings.PoolSettings.Dreamers)
        {
            _mimicableItems.Add(new(ItemNames.Monomon, 2f));
            _mimicableItems.Add(new(ItemNames.Lurien, 2f));
            _mimicableItems.Add(new(ItemNames.Herrah, 2f));
            _mimicableItems.Add(new(ItemNames.World_Sense, .25f));
        }

        if (settings.PoolSettings.Relics)
        {
            _mimicableItems.Add(new(ItemNames.Wanderers_Journal, .25f));
            _mimicableItems.Add(new(ItemNames.Hallownest_Seal, .25f));
            _mimicableItems.Add(new(ItemNames.Kings_Idol, .25f));
            _mimicableItems.Add(new(ItemNames.Arcane_Egg, .25f));
        }

        if (settings.PoolSettings.Stags)
        {
            _mimicableItems.Add(new(ItemNames.Stag_Nest_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.City_Storerooms_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Crossroads_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Dirtmouth_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Distant_Village_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Greenpath_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Hidden_Station_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Kings_Station_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Queens_Gardens_Stag, .25f));
            _mimicableItems.Add(new(ItemNames.Queens_Station_Stag, .25f));
        }

        if (settings.PoolSettings.PaleOre)
            _mimicableItems.Add(new(ItemNames.Pale_Ore, 1f));

        if (settings.PoolSettings.MaskShards)
            _mimicableItems.Add(new(ItemNames.Mask_Shard, .75f));

        if (settings.PoolSettings.VesselFragments)
            _mimicableItems.Add(new(ItemNames.Vessel_Fragment, .75f));

        if (settings.PoolSettings.Grubs)
            _mimicableItems.Add(new(ItemNames.Grub, .25f));

        if (settings.PoolSettings.RancidEggs)
            _mimicableItems.Add(new(ItemNames.Rancid_Egg, .15f));

        if (settings.NoveltySettings.RandomizeFocus)
            _mimicableItems.Add(new(ItemNames.Focus, 2f));
    }

    /// <summary>
    /// Check if other connection want to place their own mimics and add them, if possible.
    /// </summary>
    private static void AddMimickableItems(RequestBuilder requestBuilder)
    {
        _mimicableItems.Clear();
        AddBaseMimics(requestBuilder.gs);
        CurseRandomizer.Instance.LogDebug("Check for additional mimicks");
        // Check for additional mimickable items, like from other connections.
        List<string> mimickableItemNames = _mimicableItems.Select(x => x.Item1).ToList();
        foreach (StageBuilder stage in requestBuilder.Stages)
            foreach (ItemGroupBuilder itemGroup in stage.Groups.Where(x => x is ItemGroupBuilder).Select(x => x as ItemGroupBuilder))
                foreach (string itemName in itemGroup.Items.EnumerateDistinct())
                {
                    if (Finder.GetItem(itemName) is not AbstractItem item || mimickableItemNames.Contains(item.name))
                        continue;
                    try
                    {
                        if (item.tags?.FirstOrDefault(x => x is IInteropTag tag && tag.Message == "CurseData") is IInteropTag curseTag)
                        {
                            CurseRandomizer.Instance.LogDebug("Found " + item.name + " with a viable tag.");
                            if (curseTag.TryGetProperty("CanMimic", out IBool check) && check.Value)
                            {
                                mimickableItemNames.Add(item.name);
                                if (curseTag.TryGetProperty("Weight", out float weight))
                                    _mimicableItems.Add(new(item.name, Math.Min(1, weight)));
                                else
                                    _mimicableItems.Add(new(item.name, 1f));
                                CurseRandomizer.Instance.LogDebug("Added " + item.name + " as a viable mimic.");
                            }
                            else
                                CurseRandomizer.Instance.LogDebug("Couldn't find a CanMimic property or didn't pass the check");
                        }
                    }
                    catch (Exception exception)
                    {
                        CurseRandomizer.Instance.LogError("Couldn't add mimics for item " + item?.name + ": " + exception.Message);
                        CurseRandomizer.Instance.LogError("At: " + exception.StackTrace);
                    }
                }

        if (!_mimicableItems.Any())
            _mimicableItems.Add((ItemNames.Mothwing_Cloak, 1f));
    }

    private static string RollMimic()
    {
        float totalWeigth = 0f;
        foreach ((string, float) mimick in _mimicableItems)
            totalWeigth += mimick.Item2;

        float rolled = UnityEngine.Random.Range(0, totalWeigth);
        for (int index = 0; index < _mimicableItems.Count; index++)
        {
            if (rolled < _mimicableItems[index].Item2)
                return _mimicableItems[index].Item1;
            rolled -= _mimicableItems[index].Item2;
        }
        return null;
    }

    /// <summary>
    /// Get all items which this mod can replace with curses.
    /// </summary>
    private static List<string> GetReplaceableItems(RequestBuilder builder)
    {
        List<string> viableItems = new();

        if (CurseRandomizer.Instance.Settings.Pools.MaskShards && builder.gs.PoolSettings.MaskShards)
            viableItems.Add(builder.gs.MiscSettings.MaskShards switch
            {
                MaskShardType.FourShardsPerMask => ItemNames.Mask_Shard,
                MaskShardType.TwoShardsPerMask => ItemNames.Double_Mask_Shard,
                _ => ItemNames.Full_Mask
            });
        if (CurseRandomizer.Instance.Settings.Pools.VesselFragments && builder.gs.PoolSettings.VesselFragments)
            viableItems.Add(builder.gs.MiscSettings.VesselFragments switch
            {
                VesselFragmentType.TwoFragmentsPerVessel => ItemNames.Double_Vessel_Fragment,
                VesselFragmentType.OneFragmentPerVessel => ItemNames.Full_Soul_Vessel,
                _ => ItemNames.Vessel_Fragment
            });
        if (CurseRandomizer.Instance.Settings.Pools.PaleOre && builder.gs.PoolSettings.PaleOre)
            viableItems.Add(ItemNames.Pale_Ore);
        if (CurseRandomizer.Instance.Settings.Pools.Notches && builder.gs.PoolSettings.CharmNotches)
            viableItems.Add(ItemNames.Charm_Notch);
        if (CurseRandomizer.Instance.Settings.Pools.Relics && builder.gs.PoolSettings.Relics)
            viableItems.AddRange(new string[] { ItemNames.Wanderers_Journal, ItemNames.Hallownest_Seal, ItemNames.Kings_Idol, ItemNames.Arcane_Egg });
        if (CurseRandomizer.Instance.Settings.Pools.Rocks && builder.gs.PoolSettings.GeoRocks)
            viableItems.AddRange(new string[] {ItemNames.Geo_Rock_Abyss, ItemNames.Geo_Rock_City, ItemNames.Geo_Rock_Deepnest, ItemNames.Geo_Rock_Default,
            ItemNames.Geo_Rock_Fung01, ItemNames.Geo_Rock_Fung02, ItemNames.Geo_Rock_Grave01, ItemNames.Geo_Rock_Grave02, ItemNames.Geo_Rock_GreenPath01,
            ItemNames.Geo_Rock_GreenPath02, ItemNames.Geo_Rock_Hive, ItemNames.Geo_Rock_Mine, ItemNames.Geo_Rock_Outskirts, ItemNames.Geo_Rock_Outskirts420});
        if (CurseRandomizer.Instance.Settings.Pools.Geo && builder.gs.PoolSettings.GeoChests)
            viableItems.AddRange(new string[] {ItemNames.Geo_Chest_Crystal_Peak, ItemNames.Geo_Chest_False_Knight, ItemNames.Geo_Chest_Greenpath, ItemNames.Geo_Chest_Junk_Pit_1,
            ItemNames.Geo_Chest_Junk_Pit_2, ItemNames.Geo_Chest_Junk_Pit_3, ItemNames.Geo_Chest_Junk_Pit_5, ItemNames.Geo_Chest_Mantis_Lords, ItemNames.Geo_Chest_Resting_Grounds,
            ItemNames.Geo_Chest_Soul_Master, ItemNames.Geo_Chest_Watcher_Knights, ItemNames.Geo_Chest_Weavers_Den});
        if (CurseRandomizer.Instance.Settings.Pools.Geo && builder.gs.PoolSettings.BossGeo)
            viableItems.AddRange(new string[] {ItemNames.Boss_Geo_Crystal_Guardian, ItemNames.Boss_Geo_Elegant_Soul_Warrior, ItemNames.Boss_Geo_Enraged_Guardian,
            ItemNames.Boss_Geo_Gorgeous_Husk, ItemNames.Boss_Geo_Gruz_Mother, ItemNames.Boss_Geo_Massive_Moss_Charger, ItemNames.Boss_Geo_Sanctum_Soul_Warrior,
            ItemNames.Boss_Geo_Vengefly_King});
        if (CurseRandomizer.Instance.Settings.Pools.Totems && builder.gs.PoolSettings.SoulTotems)
        {
            viableItems.AddRange(new string[] { ItemNames.Soul_Totem_A, ItemNames.Soul_Totem_B, ItemNames.Soul_Totem_C, ItemNames.Soul_Totem_D,
            ItemNames.Soul_Totem_E,ItemNames.Soul_Totem_F,ItemNames.Soul_Totem_G, ItemNames.Soul_Refill});
            if (builder.gs.LongLocationSettings.WhitePalaceRando != LongLocationSettings.WPSetting.ExcludeWhitePalace)
            {
                viableItems.Add(ItemNames.Soul_Totem_Palace);
                viableItems.Add(ItemNames.Soul_Totem_Path_of_Pain);
            }
        }

        if (CurseRandomizer.Instance.Settings.Pools.Custom)
            foreach (StageBuilder stage in builder.Stages)
                foreach (ItemGroupBuilder itemGroup in stage.Groups.Where(x => x is ItemGroupBuilder).Select(x => x as ItemGroupBuilder))
                    foreach (string itemName in itemGroup.Items.EnumerateDistinct())
                        try
                        {
                            if (Finder.GetItem(itemName) is not AbstractItem item || viableItems.Contains(item.name))
                                continue;
                            if (item.tags?.FirstOrDefault(x => x is IInteropTag tag && tag.Message == "CurseData") is IInteropTag curseData)
                                if (curseData.TryGetProperty("CanReplace", out IBool canReplace) && canReplace.Value)
                                {
                                    viableItems.Add(item.name);
                                    CurseRandomizer.Instance.LogDebug("Added " + item.name + " as a replaceable item.");
                                }
                        }
                        catch (Exception exception)
                        {
                            CurseRandomizer.Instance.LogError("Error while trying to check for CanReplace tag." + exception.Message + " ;\nAt: " + exception.StackTrace);
                        }
        return viableItems;
    }

    private static void ModifyLogic(GenerationSettings settings, LogicManagerBuilder builder)
    {
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
            return;

        if (CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
        {
            //VariableResolver resolver = builder.VariableResolver;
            //builder.VariableResolver = new CurseVariableResolver() { Inner = resolver };
            builder.AddItem(new SingleItem("Fool_Item_Mocked_Shard", new(builder.GetTerm("MASKSHARDS"), 1)));
            builder.AddItem(new SingleItem("Fool_Item_Two_Mocked_Shards", new(builder.GetTerm("MASKSHARDS"), 2)));
            builder.AddItem(new SingleItem("Fool_Item_Mocked_Mask", new(builder.GetTerm("MASKSHARDS"), 4)));

            builder.GetOrAddTerm("NOCURSE");
            List<string> skipTerms = new()
            {
                "SHADESKIPS",
                "INFECTIONSKIPS",
                "BACKGROUNDPOGOS",
                "PRECISEMOVEMENT",
                "OBSCURESKIPS",
                "ENEMYPOGOS",
                "SPIKETUNNELS",
                "FIREBALLSKIPS",
                "COMPLEXSKIPS",
                "DIFFICULTSKIPS",
                "DAMAGEBOOSTS",
                "DANGEROUSSKIPS"
            };

            Dictionary<string, LogicClause> macros = ReflectionHelper.GetField<LogicProcessor, Dictionary<string, LogicClause>>(builder.LP, "macros");
            foreach (string term in macros.Keys.ToList())
                foreach (string skipTerm in skipTerms)
                    builder.DoSubst(new(term, skipTerm, "NOCURSE"));
            if (CurseRandomizer.Instance.Settings.CurseControlSettings.OmenMode)
                builder.AddItem(new EmptyItem("OMEN"));
            if (CurseRandomizer.Instance.Settings.CurseControlSettings.Bargains)
                builder.GetOrAddTerm("TAKECURSE");
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet)
        {
            Term wallet = builder.GetOrAddTerm("WALLET");
            builder.AddItem(new SingleItem("Geo_Wallet", new(wallet, 1)));

            // Divine upgrades.
            builder.DoLogicEdit(new(LocationNames.Unbreakable_Greed, "(ORIG) + WALLET>1"));
            builder.DoLogicEdit(new(LocationNames.Unbreakable_Heart, "(ORIG) + WALLET>1"));
            builder.DoLogicEdit(new(LocationNames.Unbreakable_Strength, "(ORIG) + WALLET>1"));

            // Modify stags which cost more than 200 geo.
            builder.DoLogicEdit(new(LocationNames.Stag_Nest_Stag, "(ORIG) + WALLET"));
            builder.DoLogicEdit(new(LocationNames.Distant_Village_Stag, "(ORIG) + WALLET"));
            builder.DoLogicEdit(new(LocationNames.Hidden_Station_Stag, "(ORIG) + WALLET"));
            builder.DoLogicEdit(new(LocationNames.Kings_Station_Stag, "(ORIG) + WALLET"));

            // Fountain in Basin
            builder.DoLogicEdit(new(LocationNames.Vessel_Fragment_Basin, "(ORIG) + WALLET>2"));

            // Oro
            builder.DoLogicEdit(new(LocationNames.Dash_Slash, "(ORIG) + WALLET>1"));

            // For nailsmith (when rando plus is being used)
            if (Finder.GetLocation("Nailsmith_Upgrade_1") != null
                && builder.LogicLookup.Any(x => string.Equals("Nailsmith_Upgrade_1", x.Key, StringComparison.CurrentCultureIgnoreCase)))
            {
                builder.DoLogicEdit(new("Nailsmith_Upgrade_1", "(ORIG) + WALLET"));
                builder.DoLogicEdit(new("Nailsmith_Upgrade_2", "(ORIG) + WALLET>1"));
                builder.DoLogicEdit(new("Nailsmith_Upgrade_3", "(ORIG) + WALLET>2"));
                builder.DoLogicEdit(new("Nailsmith_Upgrade_4", "(ORIG) + WALLET>2"));
            }
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedDreamNail)
        {
            Term fragment = builder.GetOrAddTerm("DREAMNAILFRAGMENT");
            builder.AddItem(new SingleItem(Dreamnail_Fragment, new(fragment, 1)));

            // Adjust dream warrior
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Elder_Hu, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Gorb, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_No_Eyes, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Marmu, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Galien, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Markoth, "(ORIG) + DREAMNAILFRAGMENT"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Xero, "(ORIG) + DREAMNAILFRAGMENT"));

            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Failed_Champion, "(ORIG) + DREAMNAILFRAGMENT>1"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Lost_Kin, "(ORIG) + DREAMNAILFRAGMENT>1"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Soul_Tyrant, "(ORIG) + DREAMNAILFRAGMENT>1"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_White_Defender, "(ORIG) + DREAMNAILFRAGMENT>1"));
            builder.DoLogicEdit(new(LocationNames.Boss_Essence_Grey_Prince_Zote, "(ORIG) + DREAMNAILFRAGMENT>1"));

            // For TRJR
            if (builder.Waypoints.Contains("Defeated_Any_Elder_Hu"))
            {
                builder.DoLogicEdit(new("Defeated_Any_Elder_Hu", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_Xero", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_No_Eyes", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_Marmu", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_Galien", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_Gorb", "(ORIG) + DREAMNAILFRAGMENT"));
                builder.DoLogicEdit(new("Defeated_Any_Markoth", "(ORIG) + DREAMNAILFRAGMENT"));

                builder.DoLogicEdit(new("Defeated_Any_White_Defender", "(ORIG) + DREAMNAILFRAGMENT>1"));
                builder.DoLogicEdit(new("Defeated_Any_Grey_Prince_Zote", "(ORIG) + DREAMNAILFRAGMENT>1"));
            }
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel > 0)
        {
            Dictionary<string, LogicClause> macros = ReflectionHelper.GetField<LogicProcessor, Dictionary<string, LogicClause>>(builder.LP, "macros");
            foreach (string term in macros.Keys.ToList())
                builder.DoSubst(new(term, "FIREBALLSKIPS", "(FIREBALLSKIPS + VESSELFRAGMENTS>5)"));
        }
    }

    private static void ModifyShopsForWallets(LocationRequestInfo info, int walletAmounts, RequestBuilder builder)
    {
        info.onRandoLocationCreation += (factory, location) =>
        {
            IEnumerable<LogicGeoCost> geoCosts = location.costs?.OfType<LogicGeoCost>();
            if (geoCosts == null || !geoCosts.Any())
                return;
            int neededWallets = builder.rng.Next(0, walletAmounts + 1);
            if (neededWallets > 0)
                location.AddCost(new SimpleCost(builder.lm.GetTerm("WALLET"), neededWallets));
        };
        info.onRandomizerFinish += placement =>
        {
            if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                    || randoLocation.costs == null)
                return;

            // To preserve the vanilla cost if possible, we just change it, when it is outside our range.
            SimpleCost walletCost = randoLocation.costs.OfType<SimpleCost>().FirstOrDefault(x => x.term?.Name == "WALLET");
            int minCost = walletCost == null
                ? 1
                : ModManager.WalletCapacities[walletCost.threshold - 1];

            int maxCost = walletCost == null
                ? ModManager.WalletCapacities[0]
                : (walletCost.threshold == walletAmounts
                    ? ModManager.WalletCapacities[walletAmounts - 1] + 500
                    : ModManager.WalletCapacities[walletCost.threshold]);

            foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                if (gc.GeoAmount == 1)
                    continue;
                else if (gc.GeoAmount < minCost || gc.GeoAmount > maxCost)
                    gc.GeoAmount = builder.rng.Next(minCost, maxCost);
        };
    }

    private static void ModifyShopsForCurses(LocationRequestInfo info, RequestBuilder builder)
    {
        info.onRandoLocationCreation += (factory, location) =>
        {
            // The chance of a curse bargain is based on the amount of curses, up to 70% if "Custom" amount is used.
            int chance = ((int)CurseRandomizer.Instance.Settings.CurseControlSettings.CurseAmount + 1) * 10;
            if (builder.rng.Next(1, 101) > chance)
                return;
            location.AddCost(new SimpleCost(builder.lm.GetTerm("TAKECURSE"), builder.rng.Next(1, 4)));
        };
    }

    private static bool CostConverter(LogicCost logicCost, out Cost cost)
    {
        cost = null;
        if (logicCost is not SimpleCost simpleCost)
            return false;
        if (simpleCost.term?.Name == "WALLET")
        {
            cost = new PDIntCost(simpleCost.threshold, "wallets", $"You'll need to obtain {simpleCost.threshold} wallet(s) to be able to buy this.");
            return true;
        }
        else if (simpleCost.term?.Name == "TAKECURSE")
        {
            cost = new CurseBargainCost(simpleCost.threshold);
            return true;
        }

        return false;
    }

    #endregion
}
