using FStats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurseRandomizer.ModInterop;

/// <summary>
/// Provides stats for FStats
/// </summary>
internal static class FStatsInterop
{
    internal static void HookFStats() => API.OnGenerateScreen += RegisterPage;
    
    private static void RegisterPage(Action<DisplayInfo> registerPage)
    {
        if (!CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)
            return;
        try
        {
            DisplayInfo displayInfo = new()
            {
                Title = "Curse Stats",
                MainStat = "Total afflicted curses: " + CurseManager.GetCurses().Select(x => x.Data.CastedAmount).Aggregate((x, y) => x + y),
                Priority = -4,
                StatColumns = []
            };

            List<Curse> curses = CurseManager.GetCurses();
            string column = string.Empty;
            for (int i = 0; i < 8; i++)
                column += $"{curses[i].Name}: {curses[i].Data.CastedAmount}\n";
            displayInfo.StatColumns.Add(column);

            column = string.Empty;
            for (int i = 8; i < 16; i++)
                column += $"{curses[i].Name}: {curses[i].Data.CastedAmount}\n";
            displayInfo.StatColumns.Add(column);

            column = string.Empty;
            for (int i = 16; i < curses.Count; i++)
                column += $"{curses[i].Name}: {curses[i].Data.CastedAmount}\n";
            displayInfo.StatColumns.Add(column);
            registerPage.Invoke(displayInfo);
        }
        catch (Exception exception)
        {
            CurseRandomizer.Instance.LogError("An error occured while trying to generate FStat page: " + exception.StackTrace);
        }
    }
}