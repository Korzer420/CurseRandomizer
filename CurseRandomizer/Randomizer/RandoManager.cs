using CurseRandomizer.Curses;
using CurseRandomizer.ItemData;
using CurseRandomizer.Manager;
using CurseRandomizer.Randomizer;
using CurseRandomizer.Randomizer.Settings;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
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
using static RandomizerMod.RC.RequestBuilder;
using static RandomizerMod.Settings.MiscSettings;

namespace CurseRandomizer;

internal static class RandoManager
{
    #region Constants

    public const string Geo_Wallet = "Geo_Wallet";
    public const string Bronze_Trial_Ticket = "Bronze_Trial_Ticket";
    public const string Silver_Trial_Ticket = "Silver_Trial_Ticket";
    public const string Gold_Trial_Ticket = "Gold_Trial_Ticket";
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

        // Data for wallets.
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
                         {"MimicNames", new string[] {"Wallet", "Moneybag", "Ge0 Wallet"} },
                         {"CanMimic",  new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet) }
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

        string[] shopNames = new string[] { LocationNames.Sly, LocationNames.Sly_Key, LocationNames.Iselda, LocationNames.Salubra, LocationNames.Leg_Eater };
        string[] stages = new string[] { "Cheap", "Medium", "Expensive", "Extreme_Valuable" };
        foreach (string locationName in shopNames)
        {
            ShopLocation originalLocation = Finder.GetLocation(locationName) as ShopLocation;
            foreach (string stage in stages)
            {
                AbstractLocation currentLocation = originalLocation.Clone();
                currentLocation.name = $"{locationName}_{stage}";
                Finder.DefineCustomLocation(currentLocation);
            }
        }

        // Data for cursed colo
        Finder.DefineCustomItem(new ItemChanger.Items.BoolItem()
        {
            name = Bronze_Trial_Ticket,
            fieldName = "CanAccessBronze",
            setValue = true,
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Bronze Trial Ticket"),
                shopDesc = new BoxedString("You like beating up someone? Then this is a must-have."),
                sprite = new CustomSprite("Bronze_Pass")
            },
            tags = new()
            {
                new InteropTag()
                {
                    Message = "CurseData",
                     Properties = new()
                     {
                         {"MimicNames", new string[] {"Colo 1 Access", "Warrior Trial Ticket", "Bronce Trial Ticket"} },
                         {"CanMimic", new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedColo) }
                     }
                },
                new InteropTag()
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        {"IsMajorItem", true },
                        {"MajorItemName", Bronze_Trial_Ticket }
                    }
                }
            },
        });
        Finder.DefineCustomItem(new ItemChanger.Items.BoolItem()
        {
            name = Silver_Trial_Ticket,
            fieldName = "CanAccessSilver",
            setValue = true,
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Silver Trial Ticket"),
                shopDesc = new BoxedString("You like beating up someone? Then this is a must-have."),
                sprite = new CustomSprite("Silver_Pass")
            },
            tags = new()
            {
                new InteropTag()
                {
                    Message = "CurseData",
                     Properties = new()
                     {
                         {"MimicNames", new string[] {"Colo 2 Access", "Silver Trial Pass", "Silwer Trial Ticket"} },
                         {"CanMimic", new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedColo) }
                     }
                },
                new InteropTag()
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        {"IsMajorItem", true },
                        {"MajorItemName", Silver_Trial_Ticket }
                    }
                }
            },
        });
        Finder.DefineCustomItem(new ItemChanger.Items.BoolItem()
        {
            name = Gold_Trial_Ticket,
            fieldName = "CanAccessGold",
            setValue = true,
            UIDef = new MsgUIDef()
            {
                name = new BoxedString("Gold Trial Ticket"),
                shopDesc = new BoxedString("You like beating up someone? Then this is a must-have."),
                sprite = new CustomSprite("Gold_Pass")
            },
            tags = new()
            {
                new InteropTag()
                {
                    Message = "CurseData",
                     Properties = new()
                     {
                         {"MimicNames", new string[] {"Colo 3 Access", "Fool Trial Ticket", "Golt Trial Ticket"} },
                         {"CanMimic", new BoxedBool(CurseRandomizer.Instance.Settings.GeneralSettings.CursedColo) }
                     }
                },
                new InteropTag()
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        {"IsMajorItem", true },
                        {"MajorItemName", Gold_Trial_Ticket}
                    }
                }
            },
        });

        // Data for cursed dream nail
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

        // Data for regret curse
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
    }

    private static void SetupVesselTerm(LogicManager logicManager, GenerationSettings generationSettings, ProgressionInitializer progressionInitializer)
    {
        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel > 0)
            progressionInitializer.Increments.Add(new(logicManager.GetTerm("VESSELFRAGMENTS"), 6 - CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel * 3));
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
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
        {
            ModManager.WalletAmount = 4;
            ModManager.IsWalletCursed = false;
            ModManager.CanAccessBronze = true;
            ModManager.CanAccessSilver = true;
            ModManager.CanAccessGold = true;
            ModManager.IsColoCursed = false;
            ModManager.IsDreamNailCursed = false;
            ModManager.IsVesselCursed = false;
            ModManager.SoulVessel = 2;
            return;
        }
        _generator = new(builder.gs.Seed);
        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet)
        {
            ModManager.IsWalletCursed = true;
            ModManager.WalletAmount = 0;
            builder.AddItemByName("Geo_Wallet", 4);
            if (builder.StartItems.EnumerateDistinct().FirstOrDefault(x => x.EndsWith("_Geo")) is string geoName)
            {
                if (int.TryParse(new string(geoName.TakeWhile(x => !x.Equals('_')).ToArray()), out int value))
                    ModManager.StartGeo = value;
                else
                    ModManager.StartGeo = 0;
            }
            else
                ModManager.StartGeo = 0;
            // Replace normal shop locations with our own.
            List<string> shopToReplace = new();
            foreach (ItemGroupBuilder group in builder.EnumerateItemGroups())
            {
                if (group.Locations.GetCount(LocationNames.Iselda) > 0 && !shopToReplace.Contains(LocationNames.Iselda))
                    shopToReplace.Add(LocationNames.Iselda);
                if (group.Locations.GetCount(LocationNames.Salubra) > 0 && !shopToReplace.Contains(LocationNames.Salubra))
                    shopToReplace.Add(LocationNames.Salubra);
                if (group.Locations.GetCount(LocationNames.Sly) > 0 && !shopToReplace.Contains(LocationNames.Sly))
                    shopToReplace.Add(LocationNames.Sly);
                if (group.Locations.GetCount(LocationNames.Sly_Key) > 0 && !shopToReplace.Contains(LocationNames.Sly_Key))
                    shopToReplace.Add(LocationNames.Sly_Key);
                if (group.Locations.GetCount(LocationNames.Leg_Eater) > 0 && !shopToReplace.Contains(LocationNames.Leg_Eater))
                    shopToReplace.Add(LocationNames.Leg_Eater);
            }
            foreach (string shop in shopToReplace)
            {
                builder.ReplaceLocation(shop, $"{shop}_Cheap");
                builder.AddLocationByName($"{shop}_Medium");
                builder.AddLocationByName($"{shop}_Expensive");
                builder.AddLocationByName($"{shop}_Extreme_Valuable");

                builder.EditLocationRequest($"{shop}_Cheap", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        FlexibleCount = true,
                        Name = $"{shop}_Cheap",
                        AdditionalProgressionPenalty = true,
                    };
                    info.onRandoLocationCreation += (factory, location) =>
                    {
                        location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(0, 201)));
                    };
                    info.onRandomizerFinish += placement =>
                    {
                        if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                            return;
                        if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                            foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                gc.GeoAmount = 1;
                    };
                });
                builder.EditLocationRequest($"{shop}_Medium", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        FlexibleCount = true,
                        Name = $"{shop}_Medium",
                        AdditionalProgressionPenalty = true,
                    };
                    info.onRandoLocationCreation += (factory, location) =>
                    {
                        location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(201, 501)));
                    };
                    info.onRandomizerFinish += placement =>
                    {
                        if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                            return;
                        if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                            foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                gc.GeoAmount = 1;
                    };
                });
                builder.EditLocationRequest($"{shop}_Expensive", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        FlexibleCount = true,
                        Name = $"{shop}_Expensive",
                        AdditionalProgressionPenalty = true,
                    };
                    info.onRandoLocationCreation += (factory, location) =>
                    {
                        location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(501, 1001)));
                    };
                    info.onRandomizerFinish += placement =>
                    {
                        if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                            return;
                        if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                            foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                gc.GeoAmount = 1;
                    };
                });
                builder.EditLocationRequest($"{shop}_Extreme_Valuable", info =>
                {
                    info.getLocationDef = () => new()
                    {
                        FlexibleCount = true,
                        Name = $"{shop}_Extreme_Valuable",
                        AdditionalProgressionPenalty = true,
                    };
                    info.onRandoLocationCreation += (factory, location) =>
                    {
                        location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(1001, 1801)));
                    };
                    info.onRandomizerFinish += placement =>
                    {
                        if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                            return;
                        if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                            foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                gc.GeoAmount = 1;
                    };
                });

                if (shop == "Salubra" && (builder.gs.MiscSettings.SalubraNotches == SalubraNotchesSetting.Randomized
                    || (builder.gs.MiscSettings.SalubraNotches == SalubraNotchesSetting.GroupedWithCharmNotchesPool && builder.gs.PoolSettings.CharmNotches)))
                {
                    builder.ReplaceLocation("Salubra_(Requires_Charms)", "Salubra_(Requires_Charms)_Cheap");
                    builder.AddLocationByName("Salubra_(Requires_Charms)_Medium");
                    builder.AddLocationByName("Salubra_(Requires_Charms)_Expensive");
                    builder.AddLocationByName("Salubra_(Requires_Charms)_Extreme_Valuable");

                    builder.EditLocationRequest("Salubra_(Requires_Charms)_Cheap", info =>
                    {
                        info.getLocationDef = () =>
                        new LocationDef()
                        {
                            AdditionalProgressionPenalty = true,
                            FlexibleCount = true,
                            Name = "Salubra_(Requires_Charms)_Cheap"
                        };
                        info.randoLocationCreator += factory => factory.MakeLocation("Salubra_Cheap");
                        info.onRandoLocationCreation += (factory, location) =>
                        {
                            if (location.costs != null && location.costs.FirstOrDefault(x => x is LogicGeoCost) is LogicGeoCost cost)
                                cost.GeoAmount = _generator.Next(0, 201);
                            else
                                location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(0, 201)));
                            location.AddCost(new SimpleCost(factory.lm.GetTerm("CHARMS"), factory.rng.Next(factory.gs.CostSettings.MinimumCharmCost, factory.gs.CostSettings.MaximumCharmCost + 1)));
                        };
                        info.onRandomizerFinish += placement =>
                        {
                            if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                                return;
                            if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                                foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                    gc.GeoAmount = 1;
                        };
                    });
                    builder.EditLocationRequest("Salubra_(Requires_Charms)_Medium", info =>
                    {
                        info.getLocationDef = () =>
                        new LocationDef()
                        {
                            AdditionalProgressionPenalty = true,
                            FlexibleCount = true,
                            Name = "Salubra_(Requires_Charms)_Medium"
                        };
                        info.randoLocationCreator += factory => factory.MakeLocation("Salubra_Medium");
                        info.onRandoLocationCreation += (factory, location) =>
                        {
                            if (location.costs != null && location.costs.FirstOrDefault(x => x is LogicGeoCost) is LogicGeoCost cost)
                                cost.GeoAmount = _generator.Next(201, 501);
                            else
                                location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(201, 501)));
                            location.AddCost(new SimpleCost(factory.lm.GetTerm("CHARMS"), factory.rng.Next(factory.gs.CostSettings.MinimumCharmCost, factory.gs.CostSettings.MaximumCharmCost + 1)));
                        };
                        info.onRandomizerFinish += placement =>
                        {
                            if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                                return;
                            if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                                foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                    gc.GeoAmount = 1;
                        };
                    });
                    builder.EditLocationRequest("Salubra_(Requires_Charms)_Expensive", info =>
                    {
                        info.getLocationDef = () =>
                        new LocationDef()
                        {
                            AdditionalProgressionPenalty = true,
                            FlexibleCount = true,
                            Name = "Salubra_(Requires_Charms)_Expensive"
                        };
                        info.randoLocationCreator += factory => factory.MakeLocation("Salubra_Expensive");
                        info.onRandoLocationCreation += (factory, location) =>
                        {
                            if (location.costs != null && location.costs.FirstOrDefault(x => x is LogicGeoCost) is LogicGeoCost cost)
                                cost.GeoAmount = _generator.Next(501, 1001);
                            else
                                location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(501, 1001)));
                            location.AddCost(new SimpleCost(factory.lm.GetTerm("CHARMS"), factory.rng.Next(factory.gs.CostSettings.MinimumCharmCost, factory.gs.CostSettings.MaximumCharmCost + 1)));
                        };
                        info.onRandomizerFinish += placement =>
                        {
                            if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                || randoLocation.costs == null)
                                return;
                            if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                                foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                    gc.GeoAmount = 1;
                        };
                    });
                    builder.EditLocationRequest("Salubra_(Requires_Charms)_Extreme_Valuable", info =>
                    {
                        info.getLocationDef = () =>
                        new LocationDef()
                        {
                            AdditionalProgressionPenalty = true,
                            FlexibleCount = true,
                            Name = "Salubra_(Requires_Charms)_Extreme_Valuable"
                        };
                        info.randoLocationCreator += factory => factory.MakeLocation("Salubra_Extreme_Valuable");
                        info.onRandoLocationCreation += (factory, location) =>
                        {
                            if (location.costs != null && location.costs.FirstOrDefault(x => x is LogicGeoCost) is LogicGeoCost cost)
                                cost.GeoAmount = _generator.Next(1001, 1801);
                            else
                                location.AddCost(new LogicGeoCost(builder.lm, _generator.Next(1001, 1801)));
                            location.AddCost(new SimpleCost(factory.lm.GetTerm("CHARMS"), factory.rng.Next(factory.gs.CostSettings.MinimumCharmCost, factory.gs.CostSettings.MaximumCharmCost + 1)));
                        };
                        info.onRandomizerFinish += placement =>
                        {
                            if (placement.Location is not RandoModLocation randoLocation || placement.Item is not RandoModItem ri
                                 || randoLocation.costs == null)
                                return;
                            if (ri.item?.Name != null && ri.item.Name.StartsWith("Geo_"))
                                foreach (LogicGeoCost gc in randoLocation.costs.OfType<LogicGeoCost>())
                                    gc.GeoAmount = 1;
                        };
                    });
                }
            }
            AddShopDefaultItems(builder);
        }
        else
        {
            ModManager.WalletAmount = 4;
            ModManager.IsWalletCursed = false;
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedColo)
        {
            ModManager.IsColoCursed = true;
            ModManager.CanAccessBronze = false;
            ModManager.CanAccessSilver = false;
            ModManager.CanAccessGold = false;

            builder.AddItemByName(Bronze_Trial_Ticket);
            builder.AddItemByName(Silver_Trial_Ticket);
            builder.AddItemByName(Gold_Trial_Ticket);

            builder.EditItemRequest(Bronze_Trial_Ticket, info =>
            {
                info.getItemDef = () =>
                new()
                {
                    Name = Bronze_Trial_Ticket,
                    MajorItem = true,
                    Pool = "Keys"
                };
            });
            builder.EditItemRequest(Silver_Trial_Ticket, info =>
            {
                info.getItemDef = () =>
                new()
                {
                    Name = Silver_Trial_Ticket,
                    MajorItem = true,
                    Pool = "Keys"
                };
            });
            builder.EditItemRequest(Gold_Trial_Ticket, info =>
            {
                info.getItemDef = () =>
                new()
                {
                    Name = Gold_Trial_Ticket,
                    MajorItem = true,
                    Pool = "Keys"
                };
            });
        }
        else
        {
            ModManager.CanAccessBronze = true;
            ModManager.CanAccessSilver = true;
            ModManager.CanAccessGold = true;
            ModManager.IsColoCursed = false;
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedDreamNail)
        {
            ModManager.DreamUpgrade = 0;
            builder.AddItemByName(Dreamnail_Fragment, 2);
            ModManager.IsDreamNailCursed = true;
        }
        else
            ModManager.IsDreamNailCursed = false;

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel != 0)
        {
            ModManager.IsVesselCursed = true;
            ModManager.SoulVessel = (CurseRandomizer.Instance.Settings.GeneralSettings.CursedVessel - 2) * -1;
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
        else
        {
            ModManager.IsVesselCursed = false;
            ModManager.SoulVessel = 2;
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
            ModManager.UseCurses = false;
            OmenCurse.OmenMode = false;
            return;
        }
        ModManager.UseCurses = true;
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
        CurseManager.DefaultCurse ??= CurseManager.GetCurseByType(CurseType.Pain);

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
                string itemToMimic = CurseRandomizer.Instance.Settings.CurseControlSettings.TakeReplaceGroup ?
                        "Evil_" + pickedItem
                    : RollMimic();
                builder.AddItemByName(CurseItem.CursePrefix + itemToMimic);
                CurseRandomizer.Instance.LogDebug("Removed " + pickedItem + " for a curse.");
            }

        if (amount > 0 && CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.ForceReplace)
            for (; amount > 0; amount--)
                builder.AddItemByName(CurseItem.CursePrefix + RollMimic());

        else if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod == RequestMethod.ForceReplace && amount > 0)
            CurseRandomizer.Instance.LogWarn("Couldn't replace enough items to satisfy the selected amount. Disposed amount: " + amount);

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
#if RELEASE
        AddBaseMimics(requestBuilder.gs);
#endif
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
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedWallet)
        {
            Term wallet = builder.GetOrAddTerm("WALLET");
            builder.AddItem(new SingleItem("Geo_Wallet", new(wallet, 1)));

            using Stream stream = typeof(RandoManager).Assembly.GetManifestResourceStream("CurseRandomizer.Resources.Logic.WalletLogic.json");
            builder.DeserializeJson(LogicManagerBuilder.JsonType.Locations, stream);

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

            if (builder.IsTerm("LISTEN"))
            {
                string[] stages = new string[] { "_Cheap", "_Medium", "_Expensive", "_Extreme_Valuable" };
                foreach (string stage in stages)
                {
                    builder.DoLogicEdit(new($"{LocationNames.Iselda}{stage}", "(ORIG) + LISTEN"));
                    builder.DoLogicEdit(new($"{LocationNames.Salubra}{stage}", "(ORIG) + LISTEN"));
                    builder.DoLogicEdit(new($"{LocationNames.Sly}{stage}", "(ORIG) + LISTEN"));
                    builder.DoLogicEdit(new($"{LocationNames.Sly_Key}{stage}", "(ORIG) + LISTEN"));
                    builder.DoLogicEdit(new($"{LocationNames.Leg_Eater}{stage}", "(ORIG) + LISTEN"));
                }
            }
        }

        if (CurseRandomizer.Instance.Settings.GeneralSettings.CursedColo)
        {
            Term term = builder.GetOrAddTerm("BRONZE");
            builder.AddItem(new RandomizerCore.LogicItems.BoolItem(Bronze_Trial_Ticket, term));

            term = builder.GetOrAddTerm("SILVER");
            builder.AddItem(new RandomizerCore.LogicItems.BoolItem(Silver_Trial_Ticket, term));

            term = builder.GetOrAddTerm("GOLD");
            builder.AddItem(new RandomizerCore.LogicItems.BoolItem(Gold_Trial_Ticket, term));

            builder.DoLogicEdit(new("Defeated_Colosseum_1", "(ORIG) + BRONZE"));
            builder.DoLogicEdit(new("Defeated_Colosseum_2", "(ORIG) + SILVER"));
            if (builder.Waypoints.Contains("Defeated_Colosseum_3"))
                builder.DoLogicEdit(new("Defeated_Colosseum_3", "(ORIG) + GOLD"));
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

    #endregion
}
