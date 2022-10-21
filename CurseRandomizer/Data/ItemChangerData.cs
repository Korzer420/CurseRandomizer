using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Data;

internal static class ItemChangerData
{
    internal static void DefineItems()
    {
        for (int i = 0; i < 7; i++)
        {
            Finder.DefineCustomItem(new CurseItem()
            {

            });
        }
    }
}
