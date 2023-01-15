using RandomizerCore.Logic;
using RandomizerMod.RC;
using System.Linq;

namespace CurseRandomizer.Randomizer;

internal class CurseVariableResolver : RandoVariableResolver
{
    private readonly string[] _states = new string[]
    {
                "$EQUIPPEDCHARM[Spell_Twister]",
                "$EQUIPPEDCHARM[Lifeblood_Heart]",
                "$EQUIPPEDCHARM[Lifeblood_Core]",
                "$EQUIPPEDCHARM[Joni's_Blessing]",
                "$EQUIPPEDCHARM[Hiveblood]",
                "$EQUIPPEDCHARM[Deep_Focus]",
                "$TAKEDAMAGE[2]",
                "$EQUIPPEDCHARM[Glowing_Womb]",
                "$EQUIPPEDCHARM[Weaversong]",
                "$EQUIPPEDCHARM[Spore_Shroom]",
                "$EQUIPPEDCHARM[Grubberfly's_Elegy]",
                "$EQUIPPEDCHARM[Mark_of_Pride]",
                "$EQUIPPEDCHARM[Dashmaster]",
                "$CASTSPELL[2,before:AREASOUL,after:ITEMSOUL]",
                "$TAKEDAMAGE",
                "$CASTSPELL[2]",
                "$CASTSPELL[3,before:AREASOUL]",
                "$CASTSPELL[2,before:ROOMSOUL]",
                "$CASTSPELL[2,before:ROOMSOUL,after:ROOMSOUL]",
                "$CASTSPELL[2,1,3]",
                "$CASTSPELL[2,before:AREASOUL,after:AREASOUL]",
                "$EQUIPPEDCHARM[Sharp_Shadow]",
                "$CASTSPELL[2,before:ITEMSOUL,after:AREASOUL]",
                "$CASTSPELL[3,before:ITEMSOUL,after:MAPAREASOUL]",
                "$CASTSPELL[3,before:AREASOUL,after:ITEMSOUL]",
                "$CASTSPELL[3,before:ROOMSOUL,after:ROOMSOUL]",
                "$CASTSPELL[3,1,before:ROOMSOUL,after:ROOMSOUL]",
                "$CASTSPELL[before:ROOMSOUL,after:MAPAREASOUL]",
                "$CASTSPELL[3]",
                "$EQUIPPEDCHARM[Sprintmaster]",
                "$CASTSPELL[2,1]",
                "$NotchCost[31,35,37]",
                "$NotchCost[22,31,37]",
                "$CASTSPELL[3,before:ITEMSOUL,after:AREASOUL]",
                "$EQUIPPEDCHARM[Defender's_Crest]",
                "$TAKEDAMAGE[3]",
                "$CASTSPELL[3,before:ROOMSOUL]",
                "$CASTSPELL[2,before:AREASOUL]"
    };

    public override bool TryMatch(LogicManager lm, string term, out LogicVariable variable)
    {
        if (_states.Contains(term))
            return Inner.TryMatch(lm, "FALSE", out variable);
        else
            return Inner.TryMatch(lm, term, out variable);
    }
}