using FStats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer.Randomizer;

/// <summary>
/// Provides stats for FStats
/// </summary>
internal static class CurseStats
{
    internal static void HookFStats() => API.OnGenerateScreen += RegisterPage;
    
    private static void RegisterPage(Action<DisplayInfo> registerPage)
    {
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
            return;
        try
        {
            FStats.DisplayInfo displayInfo = new()
            {
                Title = "Curse Stats",
                MainStat = "Total afflicted curses: " + CurseManager.GetCurses().Select(x => x.Data.CastedAmount).Aggregate((x, y) => x + y),
                Priority = -4,
                StatColumns = new()
            };

            List<Curse> curses = CurseManager.GetCurses();
            string column = string.Empty;
            for (int i = 0; i < 8; i++)
                column += $"{curses[i].Name}: {curses[i].Data.CastedAmount}\n";
            displayInfo.StatColumns.Add(column);

            column = string.Empty;
            for (int i = 8; i < 15; i++)
                column += $"{curses[i].Name}: {curses[i].Data.CastedAmount}\n";
            if (!curses.Any(x => x.Type == CurseType.Custom))
                column += "Custom Curses: -";
            else if (curses.Count(x => x.Type == CurseType.Custom) == 1)
                column += "Custom Curses: " + curses.First(x => x.Type == CurseType.Custom).Data.CastedAmount;
            else
                column += "Custom Curses: " + curses.Where(x => x.Type == CurseType.Custom).Select(x => x.Data.CastedAmount).Aggregate((x, y) => x + y);
            displayInfo.StatColumns.Add(column);
            registerPage.Invoke(displayInfo);
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.LogError("An error occured while trying to generate FStat page: " + exception.StackTrace);
        }
    }
}