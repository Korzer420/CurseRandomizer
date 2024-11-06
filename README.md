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
- Amnesia (Permanent): Lowers the damage of your spells by 10% or takes away a spell upgrade (20% chance).
- Darkness (Temporarly): Lowers the vision range by 30% until you traverse 3 different rooms ("Traverse" as in you need to exit on a different side than you entered). Repeated cast increase the vision range penalty by 15% each (up to 90% if not capped). The needed room amount increases by 3 each time as well.
- Diminish (Permanent): Lowers your nail range by 0.1. For perspective, your base nail range is around 1.4. Do the math yourself :c
- Disorientation (Instant): Warps you back to your bench.
- Emptiness (Temporarly): You can no longer gain hp, even through benches. Trying to focus will deal 1 damage to you instead. Dealing 300 damage to enemies, will cease the curse (Damage is capped at 100 per enemy type) and heal you for 1 hp. Increases by 300 additional damage each time this is casted.
- Greed (Instant): Takes 50% of you geo.
- Lost (Instant): Remove a notch, mask or vessel (It is ensured, that you have at least one notch to equip quest charms). Can take vessels from the base one, lowering up to 33 soul. You'll always have enough soul to cast at least one spell.
- Normality (Permanent): Makes a charm useless, removing all it's effect, but it heals you to full health. This curse cannot be applied to quest charms. 
- Omen (Temporarly): Upon taking a hit, apply a random permanent curse onto you. This curse vanishes after killing 5 different TYPES of enemies (although the casted curses through "Omen" remain). With each cast, 5 additional enemies types are needed (up to 50 if uncapped). Taking a hit reduces the needed kill amount by 10. If you have less 10 ten kills remaining, it sets you to 1 instead. If no permanent curse can be applied, you'll be killed instantly instead.
- Pain (Instant): Take 1 to 3 damage. (60% for 1, 30% for 2 and 10% for 3 damage)
- Sloth (Permanent): Add a additional cooldown to your dash (0.1 seconds), crystal dash charge (0.15 seconds), nail art charge (0.15 seconds) or nail swing (0.1 seconds).
- Stupidity (Permanent): Spells cost 3 more soul. Also applies to focus. (Since focus works different, the extra amount is taken after the cast is finished.)
- Thirst (Permanent): Hits on enemies grant 1 soul less.
- Unknown (Permanent): Disables the health, soul, geo, essence or item display. Note that curses will retain at least their icon, so you know you got cursed even though the name is obscured.
- Weakness (Permanent): Reduce your base nail damage by 1.
- Doubt (Instant): Unequippes all charms that you're wearing. Also reshuffles the cost of ALL charms. The total cost is increased by up to 5. For example: If you have Dashmaster (3) and Compass (5), a possible final cost could be Dashmaster (6) and Compass (3) (from 8 total to 9). Charms can never cost more than 6 notches.
- Confusion (Temporarly): Switches all 9 player actions (nail, spell, dream nail etc.) with each other. Vanishes after killing a boss (Enemies with more than 200 hp are considered bosses). The needed boss kill amount is increased by 1 each time this is casted. Taking a hit has a 25% chance to reshuffle the controls if this curse is active. The shuffle is marked by "???" appearing.
- Regret (Temporarly): Each time you kill an enemy, there is a 5% chance that a random instant curse is applied to you. The chance increases by 4% for each time you have killed that enemy in the last 20. Resets the enemy list each time a curse is applied. Can also cast instant curses that are not activated in the mod menu! Spending 300 Geo removes the curse. 300 more geo is needed each time the curse is casted again. Hint: If you suffer from this curse, Iselda will sell the item "Generosity" which can be bought each time you enter the shop to spend infinite geo (so the curse is always removable). The greed curse also progresses this curse.
- Maze (Temporarly): Each time you enter a room, there is a 7% chance that you will enter a known room instead. This curse vanishes after picking up 5 different items. Note that shop items (besides "Generosity") are not counted! A wrong warp is marked by "???" appearing.
- Midas (Permanent): Occasionally you'll turn to gold for a short period of time. Trying to obtain a not necessary item, while being under this effect, will turn it to 8 geo instead. The chance of this happening is at 0.5% each second. Increasing by 0.5% each time this is casted. Picking up geo and ceasing the golden status will grant you a bit of save time, where this effect cannot occur. **If you have problems seeing colors, you can enable the "Colorless Indicator" in the mod menu, which then will give a message each time the effect is enabled/disabled**
- Custom: Allows all curses which are created from other mods to be viable options. They will appear in the menu to manually setting them yourself.
- Despair (Temporarly): Prevents all other temporary curses from progressing further. Every 5 minutes you'll receive a random used curse (besides "Despair" itself). This curse vanishes after getting desperate enough (getting 7 despair point, increased by 7 each cast) and then dying (**if you're playing on steel soul the curse will just vanish**). Note that dying in areas, where the shade doesn't appear do not count! Despair points can be obtained from various sources, but are capped at certain interactions. These are all viable options to gain despair point (Note, that it is intended to figure this out by yourself, so I'd recommend to just try it out instead of looking it up. Anyway "Spoilers"):
    - Spending Geo (1 point per 500 Geo) (Capped at 10 points)
    - Entering the same rooms over and over again (Entering a room 5 times in between 50 rooms grants 1 point) (Capped at 10 points)
    - Dying (3 points per Death) (Capped at 15 points)
    - Killing a new enemy type (1 point) (Unlimited) If this curse is cast multiple times, all known enemies flags will reset. Simply put: You kill a vengefly -> 1 Point -> You pick up another despair -> you can get 1   
      point from vengeflies again.
    - Obtaining a curse (1 point) (Unlimited). Curses applied by despair itself and other curses like omen also count.
    - Dealing damage with spells (1 point per 200 damage) (Capped at 20 points)

**Choosing certain curses will override the logic to remove skip logic, since they may break specific skips. The application of the curses takes priority! Be aware of that.**

### Main Settings

#### Perfect Mimics
If enabled, even the names will match the original items, giving the player no indicator if an item might be a curse instead. Playing with this is not recommended. (Unless you like pain... I guess) If you are using the AllMajorItemsByArea mod, items which mimic skills will be considered as major items, trying to trick you even more.

#### Cap Effects
If enabled, the "cap" of curses can be determined by the player. These are used to evaluate if a curse can applied at all. For example, if you set the "Pain Cap" to 2, this curse cannot deal damage to you if you have 2 or less masks remaining. Here's what the caps do:

- Amnesia: Determines how much times the spell damage can be lowered. If you select 3, the spell damage can be lowered by 30%. **If you set the cap to lower than 5, spell upgrades cannot be taken away.**
- Darkness: Determines how stacked the vision range can be. If you select 3, the vision range can be lowered up to 45%. **Doesn't affected the needed rooms or if the curse can be casted.**
- Diminish: Determines how stacked the nail decrease can be. If you select 3, the nail range can be lowered by 0.3 (Slightly above 20%).
- Disorientation: The cap does nothing, but since otherwise the curse would feel excluded from the rest of the group, it also has cap button. ^-^
- Emptiness: Determines how low your BASE max health can be.
- Greed: Determines how much geo can be taken at max from a single cast.
- Lost: Determines what the amount of relics/notches you need to posess is. If you select 3, it can only take relics/notches from which you have at least 3. Although this can be set to 0. The notch check will still evalute as if the cap would be one, since otherwise equipping charms is impossible.
- Normality: Determines how much charms can be made useless. 
- Omen: Determines the max amount of needed enemy type kills.
- Pain: Determines the min health you need for the curse to be applied.
- Sloth: Determines the max amount of slows that can be applied. If you select 3, you nail slash and dash can only have an additional cooldown of 0.3 seconds.
- Stupidity: Determines the max amount of soul a spell/focus can cost.
- Thirst: Determines the min amount of soul which you should get from hits.
- Unknown: Determines how many visuals can be taken from you.
- Weakness: Determines the min amount of damage you nail should deal.

Custom: To check what the cap does for custom curses, you'd need to look it up in their respective readme.

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
- Custom: A player defined range between 0 and 200.

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
