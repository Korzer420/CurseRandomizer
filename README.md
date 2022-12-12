# CurseRandomizer
Hollow Knight Randomizer Connection for more cursed stuff.

## Curses

Adds items which grant debuffs upon obtaining them. They mimic the appearance of normal items with the only distinction, that they have an incorrect name, giving the player a chance (in some contexts) to avoid those.

Upon pickup, the curse evaluates if it can be applied in the first place. If it cannot, the default curse will be checked as well. If even the default curse fails, the "Disorientation" curse is applied.

Basically:
Can normal curse be applied? If not -> Can default curse be applied? If not -> Apply disorientation.

With this method it is ensured, that a curse is applied regardless of the context. Curses will only be applied, if you have control of the knight (otherwise, the curse will wait for you, to have control again and block the pause menu).

### Curses
Here's a list of available curses:
- Amnesia: Lowers the damage of your spells by 10% or takes away a spell upgrade (20% chance).
- Darkness: Lowers the vision range by 15% until you traverse 10 different rooms ("Traverse" as in you need to exit left if you entered right etc.). Repeated cast increase the vision range penalty by 15% each (up to 90% if not capped). The needed room amount stays the same.
- Diminish: Lowers your nail range by 0.1. For perspective, your base nail range is around 1.4. Do the math yourself :c
- Disorientation: Warps you back to your bench.
- Emptiness: Removes a mask. (Lowers you max hp)
- Greed: Takes 50% of you geo.
- Lost: Remove a relic or notch (It is ensured, that you have at least one notch to equip quest charms)
- Normality: Makes a charm useless, removing all it's effect, but it heals you to full health. This curse cannot be applied to quest charms. 
- Omen: Upon taking a hit, apply a random permanent curse onto you. The curse vanishes after killing 5 different TYPES of enemies. With each cast, 5 additional enemies types are needed (up to 50 if uncapped). Taking a hit reduces the needed kill amount by 10. If you have less 10 ten kills remaining, it sets you to 1 instead. If no permanent curse can be applied, you'll be killed instantly instead. Permanent curses include all besides: Pain, Greed, Disorientation, Darkness and Omen.
- Pain: Take 1 to 3 damage. (60% for 1, 30% for 2 and 10% for 3 damage)
- Sloth: Add a 0.1 seconds cooldown to your dash and nail swing.
- Stupidity: Spells cost 3 more soul. Also applies to focus. (Since focus works different, the extra amount is taken after the cast is finished.)
- Thirst: Hits on enemies grant 1 soul less.
- Unknown: Disables the health, soul, geo, essence or map display.
- Weakness: Reduce your base nail damage by 1.
- Custom: Allows all curses which are created from other mods to be viable options. They will appear in the menu to manually setting them yourself.

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

## Cursed Wallet
Limits the amount of geo the player can hold at a time. The base capacity is 200.
Four wallet items will be added, increasing the max amount to 500, 1000, 5000, 9999999 respectivly.

### Special Interactions
- **This settings does overwrite the cost cap which items in shops normally have, to match the requirement.** For example, normally major items are capped at 500 geo. If it is placed in the third stage (requiring two wallets), it's price will be randomized between 501 and 1000 geo. This causes many junk checks placed in shop to be mostly pointless (e.g certain geo amounts or soul totems). **Even though the second to last wallet allows 5000 geo to be stored, shop items will never cost more than 1.8k geo!**
- If Start Geo is enabled, the mod will grant you a percentage of it upon acquiring a wallet. You get 50% times the amount of wallets you have. So for example, if your starting geo was 700 (from which you would only get 200 due to the cap), the first wallet grants 350, the second 700, the third 1050 and the last 1400 geo.

## Cursed Dreamnail
Adds two extra progressive items, which deny you the access to dream warriors and dream bosses.
Obtaining one unlocks dream warriors, the second dream bosses.

## Cursed Colo
Adds 3 extra items, which lock the entrance to the trials in colosseum of fools.

## Cursed Vessel
Similiar to "Cursed Mask" this does add additional soul vessel fragments to the pool and let you only have 33/66 at a time.
This does affect the soul vessel in the UI by the fill amount but not the actual graphic. It will be adjusted, to fit percentage-wise with the amount you can store.

## Integration
This section is about how other (rando connection) mod developer can integrate their own items into this mod. If you are just a regular player, feel free to skip this part.

This mod does offer support for other connections to add their own mimics, curses and replacable items.

First off, a few general important things to note:
- **Your suggested replacements will only be taken into account, if the player chooses the "Custom" option under replaceable items!**
- Try to do your request stuff before the curse randomizer executes (usually it should be automatically granted, since the curse interaction does happen with priority 9999 in the request)
- For testing, use the debug build attached to the release. In debug, only connection items will be considered for mimics and replacements (if Custom is active).
This should make it easier to test if mimics and replacements work as intended.

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
