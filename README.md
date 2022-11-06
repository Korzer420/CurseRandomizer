# CurseRandomizer
Hollow Knight Randomizer Connection for more cursed stuff.

## Curses

Adds items which grant debuffs upon obtaining them. They mimic the appearance of normal items with the only distinction, that they have an incorrect name, giving the player a chance (in some contexts) to avoid those.

Upon pickup, the curse evaluates if it can be applied in the first place. If it cannot, the default curse will be checked as well. If even the default curse fails, the "Desorientation" Curse is applied.

Basically:
Can normal curse be applied? If not -> Can default curse be applied? If not -> Apply desorientation.

With this method it is ensured, that a curse is applied regardless of the context. Curses will only be applied, if you have control of the knight (otherwise, the curse will wait for you, to have control again).

### Curses
Here's a list of available curses:
- Pain: Take 1 damage.
- Desorientation: Warps you back to your bench.
- Emptyness: Removes a mask. (Lower you max hp)
- Lost: Remove a relic or notch (it is ensured, that you have at least one notch to equip quest charms)
- Normality: Makes a charm useless, removing all it's effect. This curse cannot be applied to quest charms and "Baldur Killers". But it heals you to full health.
- Thirst: Hits on enemies grant 1 soul less.
- Weakness: Reduce your base nail damage by 1.
- Greed: Takes 50% of you geo.
- Stupidity: Spells cost 1 more soul (*This curse cannot be applied if you enabled fireball skips to prevent locking you out of progress*)

### Main Settings

#### Perfect Mimics
If enabled, even the names will match the original items, giving the player no indicator if an item might be a curse instead. Playing with this is not recommended. (Unless you like pain... I guess)

#### Cap Effects
If enabled, the "cap" of curses can be determined by the player. These are used to evaluate if a curse can applied at all. For example, if you set the "Pain Cap" to 2, this curse cannot deal damage to you if you have 2 or less masks remaining. You cannot set the cap of "Desorientation" (since it doesn't have any) and Custom Curses.
Mostly the function as you would expect, but here are the special cases:
- Stupidity: The cap determines the MAX amount of soul a spell can cost (between 33 and 99)
- Normality: The cap determines HOW MANY charms can be useless.
- Weakness: The cap determines the MIN amount of damage, the nail should deal (this does not respect modifications done by other mods!)
- Thirst: The cap determines the MIN amount of soul, a hit on an enemy grant.
- Lose: The cap determines the MIN amount of RELICS, that the player has to have for this curse to be applyable. Notches are considered with +1 for this! E.g the cap is 3, which means that the curse can only take notches from you if you have more than 4 notches.

#### Default Curse
Determines which curse should be applied to be casted if the normal curse fails. If this fails as well, desorientation is cast instead.

#### Curse Method
Determines how the curses should be placed.
- Add: The mod simply adds the requested amount of curses as extra items.
- Replace: The mod tries to replace items, which the player allowed via "Replaceable Items". If no items are left to replace, the rest will be added as additional items.
- Force Replace: The mod tries to replace items, which the player allowed via "Replaceable Items". If no items are left to replace, the rest amount will be disposed.

#### Curse Amount
Determines how many curses should be placed.
- Few: 1 to 5 curses
- Some: 3 to 10 curses
- Medium: 5 to 15 curses
- Many: 7 to 20 curses
- Oh Oh: 10 to 30 curses will be placed.
- Custom: A player defined range between 0 and 60.

#### Replacable Items
- Defines which items the mod can replace with curses if the curse method is not "Add".


## Cursed Wallet
Limits the amount of geo the player can hold at a time. The base capacity is 200.
Four wallet items will be added, increasing the max amount to 500, 1000, 5000, 9999999 respectivly.

### Special Interactions
- **This settings does overwrite the cost cap which items in shops normally have, to match the requirement.** For example, normally major items are capped at 500 geo. If it is placed in the third stage (requiring two wallets), it's price will be randomized between 501 and 1000 geo. This causes geo checks placed in shop to be mostly pointless. **Even though the second to last wallet allows 5000 geo to be stored, shop items will never cost more than 1.8k geo!**
- If Start Geo is enabled, the mod will grant you a percentage of it upon acquiring a wallet. You get 50% times the amount of wallets you have. So for example, if your starting geo was 700 (from which you would only get 200 due to the cap), the first wallet grants 350 geo, the second 700, the third 1050 and the last 1400 geo.

## Cursed Dreamnail
Adds two extra progressive items, which deny you the access to dream warriors and dream bosses.
Obtaining one unlocks dream warriors, the second dream bosses.

## Cursed Colo
Adds 3 extra items, which lock the entrance to the trials in colosseum of fools.

## Cursed Vessel
Similiar to "Cursed Mask" this does add additional soul vessel fragments to the pool and let you only have 33/66 at a time.
This does affect the soul vessel in the UI though. It will be adjusted, to fit percentage-wise with the amount you can store.

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
- CanMimic (Func<bool>): The mod will evaluate this in the request to determine if mimics of your item can be created. In most cases this should be tied to a boolean on your setting. Use this to prevent mimics appear for items, which are not even randomized. If the mod cannot find a match with this, this item will be ignored.
- MimicNames (string[]): If your item is evaluated as a viable mimic, it will try to take a name from this array (if your item is rolled). This will be called, when Rando requests the items from IC. Note, that the "Perfect Mimics" option, will just take the original name.

**The UIDef of the item has to be inhert from MsgUIDef, for mimics to work!** The mod will clone your UIDef and adjust a few things.

Try to make the mimics names not impossible to figure out, but requiring a little thinking shouldn't hurt. I'd recommend substitute letters with numbers (e.g. C1aw), small typos, letter swaps or a different name that might also fit with the item (e.g. "Dash" for "Mothwing Cloak").

Generally speaking, only items which have a significant purpose should be mimicked to actually fool player.

### Replacements
Add this property to the Interop tag:
- CanReplace (Func<bool>): Evaluates in the request if an item can be replaced.

Only use this for real junk items, which have no purpose at all.
If it is important for your configuration, which items are actually removed, you can use RandoManager.RemovedItems to figure that out.
Note that your items are only considered if the player chooses "Custom" as viable "Replaceable Items".

### Curses
If you want to implement your own curses, that can be done as well:
- Create a class, which inherts from "Curse".
- Implement "ApplyCurse" and if needed "CanApplyCurse".
- If your curse does actually need save data, you can store/load them via overwriting "ParseData" and "LoadData".

This has a few limitations though:
- The cap can only be set by yourself, there is no support in the menu.
- All additional curses fall under "Custom". Which means, that if player disable that option, you curse cannot be applied.
- Custom curses cannot be chosen as default.

