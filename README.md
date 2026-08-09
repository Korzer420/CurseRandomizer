# Curse Randomizer
Hollow Knight Randomizer Connection for more cursed stuff.

## Curses

Adds items which grant debuffs upon obtaining them. They mimic the appearance of normal items with the only distinction, that they have an incorrect name, giving the player a chance (in some contexts) to avoid those.

Upon pickup, the curse evaluates if it can be applied in the first place. If it cannot, the default curse will be checked as well. If even the default curse fails, the "Disorientation" curse is applied.

Basically:
Can normal curse be applied? If not -> Can default curse be applied? If not -> Apply disorientation.

With this method it is ensured, that a curse is applied regardless of the context. Curses will only be applied, if you have control of the knight (otherwise, the curse will wait for you, to have control again and block the pause menu).

### Curses
Here's a list of available curses:
- Amnesia (Permanent): Lowers the damage of your spells by 10% (up to 90%).
- Clumsy (Permanent): Taking damage lowers your soul by 1 for each time this curse has been casted.
- Confusion* (Temporary): Switches player actions with each other. Vanishes after killing a boss (Enemies with more than 200 hp are considered bosses), or 2 if this curse has been cast 5 times already. Affected one more action each time this curse is applied (up to 7 total).
- Darkness (Temporary): Lowers the vision range by 30% until you traverse 6 different rooms ("Traverse" as in you need to exit on a different side than you entered). The vision range decreases with subsequent casts even further.
- Despair (Permanent): Causes most other **future** curses to be more harmful. See below for their effects.
- Diminish (Permanent): Lowers your nail range by 0.05. For perspective, your base nail range is around 1.4. Do the math yourself :c
- Disorientation (Instant): Warps you back to your bench.
- Doubt (Instant): Unequippes all charms that you're wearing. Also reshuffles the cost of ALL charms. The total cost is increased by up to 5. For example: If you have Dashmaster (3) and Compass (5), a possible final cost could be Dashmaster (6) and Compass (3) (from 8 total to 9). Charms can never cost more than 6 notches. The added extra costs decreases as your total cost goes higher.
- Emptiness (Temporary): You can no longer gain hp, even through benches. Trying to focus will deal 1 damage to you instead. Dealing 200 damage to enemies, will cease the curse (Damage is capped at 100 per enemy type) and heal you for 1 hp. Increases by 200 additional damage each time this is casted.
- Greed (Instant): Takes 30% of you geo.
- Lost (Instant): Remove a notch, mask or vessel (It is ensured, that you have at least one notch to equip quest charms). Can take vessels from the base one, lowering up to 33 soul. You'll always have enough soul to cast at least one spell. The taken consumable will be reshuffled to a an already cleared location.
- Maze (Temporary): Each time you enter a room, there is a 7% chance that you will enter a known room instead. This curse vanishes after picking up 5 different items. Note that shop items (besides "Generosity") are not counted! A wrong warp is marked by "???" appearing.
- Melancholy (Instant): Respawns all already picked up curses.
- Normality (Permanent): Makes a charm useless, removing all it's effect, but it heals you to full health. This curse cannot be applied to quest charms. 
- Omen (Temporary): Upon taking a hit, apply a random permanent curse onto you. This curse vanishes after killing 5 different TYPES of enemies (although the casted curses through "Omen" remain). With each cast, 5 additional enemies types are needed (up to 50 if uncapped). Taking a hit reduces the needed kill amount by 5. If you have less than 5 kills remaining, it will grant you 1 instead. If no permanent curse can be applied, you'll be killed instantly instead.
- Pain (Instant): Take 1 damage. This will not be affected by overcharming.
- Regret* (Temporary): Each time you kill an enemy, there is a 2% chance that a random instant curse is applied to you. The chance increases by 4% for each time you have killed that enemy in the last 20. Resets the enemy list each time a curse is applied. Can also cast instant curses that are not activated in the mod menu! Spending 300 Geo removes the curse. 300 more geo is needed each time the curse is casted again.
- Sloth (Permanent): Add a additional cooldown to your nail swing (0.05 seconds).
- Stupidity (Permanent): Spells cost 3 more soul. Also applies to focus. (Since focus works different, the extra amount is taken after the cast is finished.)
- Thirst (Permanent): You gain 1 soul less from all sources (but never less than 1).
- Trauma (Temporary): For the next 300 seconds spawn a Primal Aspid above you each 60 seconds. This interval decreases the more this curse is casted.
- Unknown (Permanent): Disables the health, soul, geo or essence display.
- Weakness (Permanent): Reduce your base nail damage by 1.

**Choosing certain curses will override the logic to remove skip logic, since they may break specific skips. The application of the curses takes priority! Be aware of that.**

*If this curse is active, Iselda will repeatedly sell a dummy item for 400 geo.

#### Despair effects
- Amnesia: Adds a 5% chance per despair to deal just 1 damage instead.
- Clumsy: Adds a 5% chance per despair to take your full soul away instead.
- Confusion: Adds 2.5% chance on hit to reroll your binding again (only if the curse is active). This effect has a 60 seconds cooldown.
- Darkness: Increases the needed rooms to clear this curse by 2 per despair cast.
- Diminish: Casts this curse an additional time per despair.
- Disorientation: No effect.
- Doubt: Adds 1 extra notch cost to the total pool per despair.
- Emptiness: Lowers your health by 1 per despair when this is casted. This can never set your health below 1.
- Greed: Increases the geo you drop by 10%.
- Lost: Adds a 5% per despair to permanently destroy the dropped item instead.
- Maze: Increases the items needed to lift this curse by 5 for each despair. Caps at 50 items.
- Melancholy: No effect.
- Normality: Adds a 10% per despair to target a charm that the player has equipped instead.
- Omen: Adds a 1% chance per cast to reset progress upon taking a hit. Caps at 20%.
- Pain: Increases the damage by 1 per despair cast.
- Regret: Increases the amount of enemies "remembered" by 4 for each despair cast (up to 80).
- Sloth: Slows down the nail art charge speed by 0.075 seconds per despair.
- Stupidity: Adds a 5% chance to remove your full soul upon casting a spell or using focus.
- Thirst: Every 20th time you'd gain soul is negated. The interval decreases for each despair cast. Capped at every other time.
- Trauma: Increases the amount of Primal Aspid spawning by 1 for each despair.
- Unknown: No effect.
- Weakness: No effect.

### Main Settings

#### Perfect Mimics
If enabled, even the names will match the original items, giving the player no indicator if an item might be a curse instead. Playing with this is not recommended. (Unless you like pain... I guess) If you are using the AllMajorItemsByArea mod, items which mimic skills will be considered as major items, trying to trick you even more.

#### Default Curse
Determines which curse should be applied to be casted if the normal curse fails. If this fails as well, disorientation is cast instead. It is suggested that this should be pain or another non permanent curse.

#### Curse Method
Determines how the curses should be placed.
- Add: The mod simply adds the requested amount of curses as extra items. Note that setting a high number of curses with this option might flood the shops a bit.
- Replace: The mod tries to replace items, which the player allowed via "Replaceable Items". If no items are left to replace, the rest will be added as additional items. Note: If the randomizer fails repeatedly while also using rando plus, it is caused by curse randomizer removing pale ores, making the nail smith checks impossible to obtain. Consider turning off Pale Ore as replacable items in that case.
- Force Replace: The mod tries to replace items, which the player allowed via "Replaceable Items". If no items are left to replace, the rest amount will be disposed.

#### Take Replace Group
If this option is used, the mimic items will take the item group from the item they replaced instead of the one the copied. This is only available if you choose Curse Method "Replace" or "Force Replace". If you don't intend to use split group options at all, this setting doesn't change anything.

#### Curse Amount
Determines how many curses should be placed. These option are based on the total amount of items that the randomizer have, but still have a minimal amount.
- Few: 1% to 3% are curse items. At least 3 to 5.
- Some: 4% to 6% are curse items. At least 5 to 10.
- Medium: 7% to 9% are curse items. At least 10 to 15.
- Many: 10% to 12% are curse items. At least 15 to 20.
- Oh Oh: 13% to 15% are curse items. At least 20 to 30.
- Custom: A player defined range between 0 and 300.

#### Replacable Items
- Defines which items the mod can replace with curses if the curse method is not "Add".

#### Bargains
Allows items in the shop to be "cursed", so they apply curses upon you, once bought. A purple/pink text is visible under the item description showing how much curses will be applied to you. This ranges from 1-3 curses.
Note that the cursed items ARE NOT counted as normal curse items and will be placed **on top of the selected amount**! Depending on the selected amount, the chance for an item to be cursed is increased by 10% for each step. "Few" grants a 10% per shop item to be cursed up to 70% if "Custom" is used. The order can be seen under "Curse Amount". Once all curses of an item got resolved, it will display what the casted curses where. Note that if a curse was blocked (i.e "Lost" was about to be casted but you have no mask, vessel and charm notch to spare), the actual result might differ from the shown message!

## Cursed Wallet
Limits the amount of geo the player can hold at a time. The base capacity is 500.
Four wallet items will be added, increasing the max amount by 500 each time (Except the last one, which will uncap the geo).
Once a wallet is picked up, it will automatically be filled to full capacity (besides the "Uncap" wallet which will just grant you 420 geo)

### Special Interactions
- **This settings does overwrite the cost cap which items in shops normally have, to match the requirement, in case the item is not the wallet range already.** For example, normally major items are capped at 500 geo. If it is placed in the third stage (requiring two wallets), it's price will be randomized between 1001 and 1500 geo. The only exception to this are junk items which cost 1 geo normally. It still requires the determined wallet amount, but will remain at 1 geo.

## Cursed Dreamnail
Adds two extra progressive items, which deny you the access to dream warriors and dream bosses.
Obtaining one unlocks dream warriors, the second dream bosses.

## Cursed Vessel
Similiar to "Cursed Mask" this does add additional soul vessel fragments to the pool and let you only have 33/66 at a time.
This does affect the soul vessel in the UI by the fill amount but not the actual graphic. It will be adjusted, to fit percentage-wise with the amount you can store.

## Integration
This section is about how other (rando connection) mod developer can integrate their own items into this mod. If you are just a regular player, feel free to skip this part.

This mod does offer support for other connections to add their own mimics, curses and replacable items.

First off, a few general important things to note:
- **Your suggested replacements will only be taken into account, if the player chooses the "Custom" option under replaceable items!**
- Try to do your request stuff before the curse randomizer executes (usually it should be automatically granted, since the curse interaction does happen with priority 9999 in the request)

To allow this mod to consider your items, add a interop tag to them (or your own implementation of an IInteropTag) with the message "CurseData".

### Mimics
Add these properties to the Interop tag:
- CanMimic (IBool): The mod will evaluate this in the request to determine if mimics of your item can be created. In most cases this should be tied to a boolean on your setting. Use this to prevent mimics appear for items, which are not even randomized. If the mod cannot find a match with this, this item will be ignored.
- MimicNames (string[]): If your item is evaluated as a viable mimic, it will try to take a name from this array (if your item is rolled). This will be called, when Rando requests the items from IC. Note, that the "Perfect Mimics" option, will just take the original name.
- Optional: Weight (float): Determines how likely it is that your item will be chosen as a mimic. Should be between 0 and 1.

**The UIDef of the item has to be inhert from MsgUIDef, for mimics to work!** The mod will clone your UIDef and adjust a few things.

Try to make the mimics names not impossible to figure out, but requiring a little thinking shouldn't hurt. I'd recommend substitute letters with numbers (e.g. C1aw), small typos, letter swaps or a different name that might also fit with the item (e.g. "Dash" for "Mothwing Cloak").

Generally speaking, only items which have a significant purpose should be mimicked to actually fool players.

### Replacements
Add this property to the Interop tag:
- CanReplace (IBool): Evaluates in the request if an item can be replaced.

Only use this for real junk items, which have no purpose at all.
If it is important for your configuration, which items are actually removed, you can use RandoManager.RemovedItems to figure that out.
Note that your items are only considered if the player chooses "Custom" as viable "Replaceable Items".

### Curses
If you want to implement your own curses, that can be done as well:
- Create a class, which inherts from "Curse".
- Implement "ApplyCurse" and "SetCap". If your curse has to implement some hooks, overwrite "ApplyHooks" and "Unhook". To check if you curse can be applied at all overwrite "CanApplyCurse".
- If your curse does actually need save data, you can define the Data.AdditionalData object. Remember to reset it via overwriting "ResetAdditionalData".

**All additional curses fall under "Custom". Which means, that if player disable that option, you curse cannot be applied.**
