using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Curses;

internal class WeaknessCurse : Curse
{
    #region Constructor

    public WeaknessCurse()
    {
        ModHooks.GetPlayerIntHook += ModifyNailDamage;
    }

    #endregion

    #region Properties

    public int Stacks { get; set; } 

    #endregion

    #region Event handler

    private int ModifyNailDamage(string name, int originalValue)
    {
        if (name == "nailDamage")
            originalValue = Math.Max(1, originalValue - Stacks);
        return originalValue;
    }

    #endregion

    #region Methods

    public override bool CanApplyCurse()
    {
        int cap = CurseRandomizer.Instance.Settings.CapEffects ? Cap : 1;
        return 5 + 4 * PlayerData.instance.GetInt(nameof(PlayerData.instance.nailSmithUpgrades)) - Stacks > cap;
    }

    public override void ApplyCurse()
    {
        Stacks--;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    public override object ParseData() => Stacks;

    public override void LoadData(object data) => Stacks = (int)data;

    #endregion
}
