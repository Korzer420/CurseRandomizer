using CurseRandomizer.ItemData;
using CurseRandomizer.Manager;
using CurseRandomizer.Randomizer;
using CurseRandomizer.Randomizer.Settings;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Modding;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
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
    private static List<AbstractItem> _mimicableItems = new();
    private static List<Curse> _availableCurses = new();
    private static Random _generator;

    public static List<string> ReplacedItems { get; set; } = new();

    #region Setup

    internal static void DefineItemChangerData()
    {
        Finder.DefineCustomItem(new CurseItem()
        {
            name = "Fool_Item",
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
                         {"CanMimic",  new InternalBoolCheck(){ ItemNumber = 0 } }
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
                         {"CanMimic", new InternalBoolCheck() { ItemNumber = 1 } }
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
                         {"CanMimic", new InternalBoolCheck() { ItemNumber = 1 } }
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
                         {"CanMimic", new InternalBoolCheck() { ItemNumber = 1 } }
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
                         {"CanMimic", new InternalBoolCheck() { ItemNumber = 2 } }
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
    }

    internal static void HookRando()
    {
        DefineItemChangerData();
        OnUpdate.Subscribe(40f, ApplySettings);
        OnUpdate.Subscribe(9999f, ApplyCurses);
        Finder.GetItemOverride += TranformCurseItems;
        RCData.RuntimeLogicOverride.Subscribe(9999f, ModifyLogic);
        RandoController.OnCalculateHash += RandoController_OnCalculateHash;
        RandomizerMenu.AttachMenu();
        SettingsLog.AfterLogSettings += WriteCurseRandoSettings;

        if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            HookRandoSettingsManager();
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
        () => CurseRandomizer.Instance.Settings));
    }

    private static int RandoController_OnCalculateHash(RandoController controller, int hashValue)
    {
        if (!CurseRandomizer.Instance.Settings.Enabled || !CurseRandomizer.Instance.Settings.UseCurses)
            return 0;
        int addition = 0;
        if (CurseRandomizer.Instance.Settings.PerfectMimics)
            addition += 410;
        if (CurseRandomizer.Instance.Settings.CapEffects)
            addition++;

        foreach (Curse curse in _availableCurses)
        {
            addition += 120 * (int)curse.Type;
            addition += 5 * curse.Cap;
        }
        addition += (int)CurseManager.DefaultCurse.Type * 420;
        addition += (int)CurseRandomizer.Instance.Settings.DefaultCurse * 777;

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
            if (requestedItemArgs.ItemName.StartsWith("Fool_Item") && CurseRandomizer.Instance.Settings.UseCurses)
            {
                itemToMimic = _mimicableItems[_generator.Next(0, _mimicableItems.Count)];
                CurseRandomizer.Instance.LogDebug("Try to replicate: " + itemToMimic.name);
                CurseItem curseItem = new()
                {
                    name = requestedItemArgs.ItemName.StartsWith("Fool_Item_") ? requestedItemArgs.ItemName : "Fake_" + itemToMimic.name,
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

                if (!CurseRandomizer.Instance.Settings.PerfectMimics)
                {
                    if (curseItem.UIDef is not MsgUIDef msgUIDef)
                        CurseRandomizer.Instance.LogError("Item " + itemToMimic.name + " couldn't be mimicked correctly. UI Def has to be inhert from MsgUIDef.");
                    else if (MimicNames.Mimics.ContainsKey(itemToMimic.name))
                        msgUIDef.name = new BoxedString(MimicNames.Mimics[itemToMimic.name][_generator.Next(0, MimicNames.Mimics[itemToMimic.name].Length)]);
                    else
                    {
                        (itemToMimic.tags.First(x => x is IInteropTag tag && tag.Message == "CurseData") as IInteropTag).TryGetProperty("MimicNames", out string[] mimicNames);
                        if (mimicNames == null || !mimicNames.Any())
                            CurseRandomizer.Instance.LogError("Couldn't find a mimic name. Will take the normal UI name of: " + itemToMimic.name);
                        else
                            msgUIDef.name = new BoxedString(mimicNames[_generator.Next(0, mimicNames.Length)]);
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
                                {"MajorItemName", "Fake_"+itemToMimic.name }
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
    /// Handles all settings besides the actual curses/mimics.
    /// </summary>
    /// <param name="builder"></param>
    private static void ApplySettings(RequestBuilder builder)
    {
        if (!CurseRandomizer.Instance.Settings.Enabled)
            return;
        _generator = new(builder.gs.Seed);
        if (CurseRandomizer.Instance.Settings.CursedWallet)
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
        }
        else
        {
            ModManager.WalletAmount = 4;
            ModManager.IsWalletCursed = false;
        }

        if (CurseRandomizer.Instance.Settings.CursedColo)
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

        if (CurseRandomizer.Instance.Settings.CursedDreamNail)
        {
            ModManager.DreamUpgrade = 0;
            builder.AddItemByName(Dreamnail_Fragment, 2);
            ModManager.IsDreamNailCursed = true;
        }
        else
            ModManager.IsDreamNailCursed = false;

        // To not even bother with figuring out with EVERY SINGLE FIREBALL SKIP, we just prevent this setting from working, if fireball skips are on.
        if (CurseRandomizer.Instance.Settings.CursedVessel != 0 && !builder.gs.SkipSettings.FireballSkips)
        {
            ModManager.IsVesselCursed = true;
            ModManager.SoulVessel = (CurseRandomizer.Instance.Settings.CursedVessel - 2) * -1;
            if (builder.gs.MiscSettings.VesselFragments == VesselFragmentType.OneFragmentPerVessel)
                builder.AddItemByName(ItemNames.Full_Soul_Vessel, CurseRandomizer.Instance.Settings.CursedVessel);
            else if (builder.gs.MiscSettings.VesselFragments == VesselFragmentType.TwoFragmentsPerVessel)
            {
                if (CurseRandomizer.Instance.Settings.CursedVessel == 2)
                    builder.AddItemByName(ItemNames.Double_Vessel_Fragment, 3);
                else
                {
                    builder.AddItemByName(ItemNames.Double_Vessel_Fragment, 1);
                    builder.AddItemByName(ItemNames.Vessel_Fragment, 1);
                }
            }
            else
                builder.AddItemByName(ItemNames.Vessel_Fragment, CurseRandomizer.Instance.Settings.CursedVessel * 3);
        }
        else
        {
            ModManager.IsVesselCursed = false;
            ModManager.SoulVessel = 2;
        }
    }

    /// <summary>
    /// Evaluates the curse settings and prepares IC for potential mimics.
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="Exception"></exception>
    private static void ApplyCurses(RequestBuilder builder)
    {
        ReplacedItems.Clear();
        if (!CurseRandomizer.Instance.Settings.Enabled || !CurseRandomizer.Instance.Settings.UseCurses)
            return;

        AddCustomMimics(builder.gs);
        // Get all items which can be removed.
        List<string> replacableItems = GetReplaceableItems(builder.gs);

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
        if (CurseRandomizer.Instance.Settings.DisorientationCurse)
            _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Disorientation));
        if (CurseRandomizer.Instance.Settings.EmptinessCurse)
            _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Emptiness));
        if (CurseRandomizer.Instance.Settings.ThirstCurse)
            _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Thirst));
        if (CurseRandomizer.Instance.Settings.WeaknessCurse)
            _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Weakness));
        if (CurseRandomizer.Instance.Settings.LostCurse)
            _availableCurses.Add(CurseManager.GetCurseByType(CurseType.Lost));
        if (CurseRandomizer.Instance.Settings.CustomCurses && CurseManager.GetCurses().Any(x => x.Type == CurseType.Custom))
            _availableCurses.AddRange(CurseManager.GetCurses().Where(x => x.Type == CurseType.Custom));

        if (!_availableCurses.Any())
            throw new Exception("No curses available to place.");

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
                CurseRandomizer.Instance.LogDebug("Removed " + pickedItem + " for a curse.");
            }

        if (amount > 0 && CurseRandomizer.Instance.Settings.CurseMethod != RequestMethod.ForceReplace)
            builder.AddItemByName("Fool_Item", amount);
        else if (CurseRandomizer.Instance.Settings.CurseMethod == RequestMethod.ForceReplace && amount > 0)
            CurseRandomizer.Instance.LogWarn("Couldn't replace enough items to satisfy the selected amount. Disposed amount: " + amount);

        // Set the caps
        CurseManager.GetCurseByType(CurseType.Pain).Cap = CurseRandomizer.Instance.Settings.PainCap;
        CurseManager.GetCurseByType(CurseType.Stupidity).Cap = CurseRandomizer.Instance.Settings.StupidityCap;
        CurseManager.GetCurseByType(CurseType.Normality).Cap = CurseRandomizer.Instance.Settings.NormalityCap;
        CurseManager.GetCurseByType(CurseType.Emptiness).Cap = CurseRandomizer.Instance.Settings.EmptynessCap;
        CurseManager.GetCurseByType(CurseType.Greed).Cap = CurseRandomizer.Instance.Settings.GreedCap;
        CurseManager.GetCurseByType(CurseType.Lost).Cap = CurseRandomizer.Instance.Settings.LostCap;
        CurseManager.GetCurseByType(CurseType.Thirst).Cap = CurseRandomizer.Instance.Settings.ThirstCap;
        CurseManager.GetCurseByType(CurseType.Weakness).Cap = CurseRandomizer.Instance.Settings.WeaknessCap;

        CurseManager.DefaultCurse = CurseManager.GetCurseByType(CurseRandomizer.Instance.Settings.DefaultCurse);
    }

    /// <summary>
    /// Adds the base mimic set which can be used, depending on the settings.
    /// </summary>
    /// <param name="settings"></param>
    private static void AddBaseMimics(GenerationSettings settings)
    {
        if (settings.PoolSettings.Skills)
        {
            _mimicableItems.Add(Finder.GetItem(ItemNames.Monarch_Wings));
            // Claw
            if (settings.NoveltySettings.SplitClaw)
            {
                _mimicableItems.Add(Finder.GetItem(ItemNames.Left_Mantis_Claw));
                _mimicableItems.Add(Finder.GetItem(ItemNames.Right_Mantis_Claw));
            }
            else
                _mimicableItems.Add(Finder.GetItem(ItemNames.Mantis_Claw));

            // Dash
            if (settings.NoveltySettings.SplitCloak)
            {
                _mimicableItems.Add(Finder.GetItem(ItemNames.Left_Mothwing_Cloak));
                _mimicableItems.Add(Finder.GetItem(ItemNames.Right_Mothwing_Cloak));
            }
            else
                _mimicableItems.Add(Finder.GetItem(ItemNames.Mothwing_Cloak));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Shade_Cloak));

            // Crystal Dash
            if (settings.NoveltySettings.SplitSuperdash)
            {
                _mimicableItems.Add(Finder.GetItem(ItemNames.Left_Crystal_Heart));
                _mimicableItems.Add(Finder.GetItem(ItemNames.Right_Crystal_Heart));
            }
            else
                _mimicableItems.Add(Finder.GetItem(ItemNames.Crystal_Heart));

            // Spells
            _mimicableItems.Add(Finder.GetItem(ItemNames.Howling_Wraiths));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Descending_Dark));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Vengeful_Spirit));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Shade_Soul));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Desolate_Dive));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Abyss_Shriek));

            // Nail arts
            _mimicableItems.Add(Finder.GetItem(ItemNames.Great_Slash));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Cyclone_Slash));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Dash_Slash));

            // Misc
            _mimicableItems.Add(Finder.GetItem(ItemNames.Dream_Nail));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Dream_Gate));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Awoken_Dream_Nail));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Ismas_Tear));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Monarch_Wings));
        }

        if (settings.PoolSettings.Keys)
        {
            _mimicableItems.Add(Finder.GetItem(ItemNames.Tram_Pass));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Simple_Key));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Elegant_Key));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Love_Key));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Kings_Brand));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Lumafly_Lantern));
            _mimicableItems.Add(Finder.GetItem(ItemNames.City_Crest));
        }

        if (settings.PoolSettings.Charms)
            foreach (string charmName in MimicNames.Mimics.SkipWhile(x => x.Key != ItemNames.Awoken_Dream_Nail).Skip(1).TakeWhile(x => x.Key != ItemNames.Void_Heart).Take(1).Select(x => x.Key))
                _mimicableItems.Add(Finder.GetItem(charmName));

        if (settings.PoolSettings.Dreamers)
        {
            _mimicableItems.Add(Finder.GetItem(ItemNames.Monomon));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Lurien));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Herrah));
            _mimicableItems.Add(Finder.GetItem(ItemNames.World_Sense));
        }

        if (settings.PoolSettings.Relics)
        {
            _mimicableItems.Add(Finder.GetItem(ItemNames.Wanderers_Journal));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Hallownest_Seal));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Kings_Idol));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Arcane_Egg));
        }

        if (settings.PoolSettings.Stags)
        {
            _mimicableItems.Add(Finder.GetItem(ItemNames.Stag_Nest_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.City_Storerooms_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Crossroads_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Dirtmouth_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Distant_Village_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Greenpath_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Hidden_Station_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Kings_Station_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Queens_Gardens_Stag));
            _mimicableItems.Add(Finder.GetItem(ItemNames.Queens_Station_Stag));
        }

        if (settings.PoolSettings.PaleOre)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Pale_Ore));

        if (settings.PoolSettings.MaskShards)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Mask_Shard));

        if (settings.PoolSettings.VesselFragments)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Vessel_Fragment));

        if (settings.PoolSettings.Grubs)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Grub));

        if (settings.PoolSettings.RancidEggs)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Rancid_Egg));

        if (settings.NoveltySettings.RandomizeFocus)
            _mimicableItems.Add(Finder.GetItem(ItemNames.Focus));
    }

    /// <summary>
    /// Check if other connection want to place their own mimics and add them, if possible.
    /// </summary>
    private static void AddCustomMimics(GenerationSettings settings)
    {
        _mimicableItems.Clear();
#if RELEASE
        AddBaseMimics(settings);
#endif
        foreach (KeyValuePair<string, AbstractItem> item in ReflectionHelper.GetField<Dictionary<string, AbstractItem>>(typeof(Finder), "CustomItems"))
        {
            if (_mimicableItems.Contains(item.Value))
                continue;
            // Other connections can add mimics via an interop tag, here we get all items which are eligatible to be mimicked.
            try
            {
                if (item.Value.tags?.FirstOrDefault(x => x is IInteropTag tag && tag.Message == "CurseData") is IInteropTag curseTag)
                {
                    CurseRandomizer.Instance.LogDebug("Found " + item.Key + " with a viable tag.");
                    if (curseTag.TryGetProperty("CanMimic", out IBool check) && check.Value)
                    {
                        CurseRandomizer.Instance.LogDebug("Added " + item.Key + " as a viable mimic.");
                        _mimicableItems.Add(item.Value);
                    }
                    else
                        CurseRandomizer.Instance.LogDebug("Couldn't find a CanMimic property or didn't pass the check");
                }
            }
            catch (Exception exception)
            {
                CurseRandomizer.Instance.LogError("Couldn't add mimics for item " + item.Key + ": " + exception.Message);
                CurseRandomizer.Instance.LogError(exception.StackTrace);
            }
        }
    }

    /// <summary>
    /// Get all items which this mod can replace with curses.
    /// </summary>
    private static List<string> GetReplaceableItems(GenerationSettings generationSettings)
    {
        List<string> viableItems = new();

        if (CurseRandomizer.Instance.Settings.MaskShards && generationSettings.PoolSettings.MaskShards)
            viableItems.Add(generationSettings.MiscSettings.MaskShards switch
            {
                MaskShardType.FourShardsPerMask => ItemNames.Mask_Shard,
                MaskShardType.TwoShardsPerMask => ItemNames.Double_Mask_Shard,
                _ => ItemNames.Full_Mask
            });
        if (CurseRandomizer.Instance.Settings.VesselFragments && generationSettings.PoolSettings.VesselFragments)
            viableItems.Add(generationSettings.MiscSettings.VesselFragments switch
            {
                VesselFragmentType.TwoFragmentsPerVessel => ItemNames.Double_Vessel_Fragment,
                VesselFragmentType.OneFragmentPerVessel => ItemNames.Full_Soul_Vessel,
                _ => ItemNames.Vessel_Fragment
            });
        if (CurseRandomizer.Instance.Settings.PaleOre && generationSettings.PoolSettings.PaleOre)
            viableItems.Add(ItemNames.Pale_Ore);
        if (CurseRandomizer.Instance.Settings.Notches && generationSettings.PoolSettings.CharmNotches)
            viableItems.Add(ItemNames.Charm_Notch);
        if (CurseRandomizer.Instance.Settings.Relics && generationSettings.PoolSettings.Relics)
            viableItems.AddRange(new string[] { ItemNames.Wanderers_Journal, ItemNames.Hallownest_Seal, ItemNames.Kings_Idol, ItemNames.Arcane_Egg });
        if (CurseRandomizer.Instance.Settings.Rocks && generationSettings.PoolSettings.GeoRocks)
            viableItems.AddRange(new string[] {ItemNames.Geo_Rock_Abyss, ItemNames.Geo_Rock_City, ItemNames.Geo_Rock_Deepnest, ItemNames.Geo_Rock_Default,
            ItemNames.Geo_Rock_Fung01, ItemNames.Geo_Rock_Fung02, ItemNames.Geo_Rock_Grave01, ItemNames.Geo_Rock_Grave02, ItemNames.Geo_Rock_GreenPath01,
            ItemNames.Geo_Rock_GreenPath02, ItemNames.Geo_Rock_Hive, ItemNames.Geo_Rock_Mine, ItemNames.Geo_Rock_Outskirts, ItemNames.Geo_Rock_Outskirts420});
        if (CurseRandomizer.Instance.Settings.Geo && generationSettings.PoolSettings.GeoChests)
            viableItems.AddRange(new string[] {ItemNames.Geo_Chest_Crystal_Peak, ItemNames.Geo_Chest_False_Knight, ItemNames.Geo_Chest_Greenpath, ItemNames.Geo_Chest_Junk_Pit_1,
            ItemNames.Geo_Chest_Junk_Pit_2, ItemNames.Geo_Chest_Junk_Pit_3, ItemNames.Geo_Chest_Junk_Pit_5, ItemNames.Geo_Chest_Mantis_Lords, ItemNames.Geo_Chest_Resting_Grounds,
            ItemNames.Geo_Chest_Soul_Master, ItemNames.Geo_Chest_Watcher_Knights, ItemNames.Geo_Chest_Weavers_Den});
        if (CurseRandomizer.Instance.Settings.Geo && generationSettings.PoolSettings.BossGeo)
            viableItems.AddRange(new string[] {ItemNames.Boss_Geo_Crystal_Guardian, ItemNames.Boss_Geo_Elegant_Soul_Warrior, ItemNames.Boss_Geo_Enraged_Guardian,
            ItemNames.Boss_Geo_Gorgeous_Husk, ItemNames.Boss_Geo_Gruz_Mother, ItemNames.Boss_Geo_Massive_Moss_Charger, ItemNames.Boss_Geo_Sanctum_Soul_Warrior,
            ItemNames.Boss_Geo_Vengefly_King});
        if (CurseRandomizer.Instance.Settings.Totems && generationSettings.PoolSettings.SoulTotems)
        {
            viableItems.AddRange(new string[] { ItemNames.Soul_Totem_A, ItemNames.Soul_Totem_B, ItemNames.Soul_Totem_C, ItemNames.Soul_Totem_D,
            ItemNames.Soul_Totem_E,ItemNames.Soul_Totem_F,ItemNames.Soul_Totem_G, ItemNames.Soul_Refill});
            if (generationSettings.LongLocationSettings.WhitePalaceRando != LongLocationSettings.WPSetting.ExcludeWhitePalace)
            {
                viableItems.Add(ItemNames.Soul_Totem_Palace);
                viableItems.Add(ItemNames.Soul_Totem_Path_of_Pain);
            }
        }

        if (CurseRandomizer.Instance.Settings.Custom)
            foreach (KeyValuePair<string, AbstractItem> item in ReflectionHelper.GetField<Dictionary<string, AbstractItem>>(typeof(Finder), "CustomItems"))
            {
                try
                {
                    if (viableItems.Contains(item.Key))
                        continue;
                    if (item.Value.tags?.FirstOrDefault(x => x is IInteropTag tag && tag.Message == "CurseData") is IInteropTag curseData)
                        if (curseData.TryGetProperty("CanReplace", out IBool canReplace) && canReplace.Value)
                            viableItems.Add(item.Key);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Couldn't determine if item {item.Key} can be replaced: " + exception.Message + " StackTrace: " + exception.StackTrace);
                }
            }

        return viableItems;
    }

    private static void ModifyLogic(GenerationSettings settings, LogicManagerBuilder builder)
    {
        if (!CurseRandomizer.Instance.Settings.Enabled)
            return;
        if (CurseRandomizer.Instance.Settings.UseCurses)
        {
            builder.AddItem(new EmptyItem("Fool_Item"));
            builder.AddItem(new SingleItem("Fool_Item_Mocked_Shard", new(builder.GetTerm("MASKSHARDS"), 1)));
            builder.AddItem(new SingleItem("Fool_Item_Two_Mocked_Shards", new(builder.GetTerm("MASKSHARDS"), 2)));
            builder.AddItem(new SingleItem("Fool_Item_Mocked_Mask", new(builder.GetTerm("MASKSHARDS"), 4)));
        }

        if (CurseRandomizer.Instance.Settings.CursedWallet)
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

        if (CurseRandomizer.Instance.Settings.CursedColo)
        {
            Term term = builder.GetOrAddTerm("BRONZE");
            builder.AddItem(new BoolItem(Bronze_Trial_Ticket, term));

            term = builder.GetOrAddTerm("SILVER");
            builder.AddItem(new BoolItem(Silver_Trial_Ticket, term));

            term = builder.GetOrAddTerm("GOLD");
            builder.AddItem(new BoolItem(Gold_Trial_Ticket, term));

            builder.DoLogicEdit(new("Defeated_Colosseum_1", "(ORIG) + BRONZE"));
            builder.DoLogicEdit(new("Defeated_Colosseum_2", "(ORIG) + SILVER"));
            if (builder.Waypoints.Contains("Defeated_Colosseum_3"))
                builder.DoLogicEdit(new("Defeated_Colosseum_3", "(ORIG) + GOLD"));
        }

        if (CurseRandomizer.Instance.Settings.CursedDreamNail)
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

                builder.DoLogicEdit(new(LocationNames.Boss_Essence_Failed_Champion, "(ORIG) + DREAMNAILFRAGMENT>1"));
                builder.DoLogicEdit(new(LocationNames.Boss_Essence_Lost_Kin, "(ORIG) + DREAMNAILFRAGMENT>1"));
                builder.DoLogicEdit(new(LocationNames.Boss_Essence_Soul_Tyrant, "(ORIG) + DREAMNAILFRAGMENT>1"));
                builder.DoLogicEdit(new(LocationNames.Boss_Essence_White_Defender, "(ORIG) + DREAMNAILFRAGMENT>1"));
                builder.DoLogicEdit(new(LocationNames.Boss_Essence_Grey_Prince_Zote, "(ORIG) + DREAMNAILFRAGMENT>1"));
            }
        }
    }

    #endregion
}
