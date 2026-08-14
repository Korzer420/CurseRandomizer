using CurseRandomizer.Randomizer.Settings;
using KorzUtils.Helper;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Modding;
using RandomizerMod.Menu;
using System.Collections.Generic;
using System.Linq;
using PoolSettings = CurseRandomizer.Randomizer.Settings.PoolSettings;

namespace CurseRandomizer.Randomizer;

internal class RandomizerMenu
{
    private static RandomizerMenu _instance;

    private MenuPage _mainPage;

    private MenuPage _cursePage;

    /// <summary>
    /// Gets the instance of the menu.
    /// </summary>
    public static RandomizerMenu Instance => _instance ??= new();

    private MenuElementFactory<GeneralSettings> _generalFactory;

    private MenuElementFactory<CurseControlSettings> _controlFactory;

    private MenuElementFactory<PoolSettings> _poolFactory;

    private MenuElementFactory<CurseSettings> _curseSettings;

    #region Event handler

    private void ConstructMenu(MenuPage previousPage)
    {
        // Generate pages and setting elements
        _mainPage = new("Curse Randomizer", previousPage);
        _generalFactory = new(_mainPage, CurseRandomizer.Instance.Settings.GeneralSettings);
        _controlFactory = new MenuElementFactory<CurseControlSettings>(_mainPage, CurseRandomizer.Instance.Settings.CurseControlSettings);
        _poolFactory = new MenuElementFactory<PoolSettings>(_mainPage, CurseRandomizer.Instance.Settings.Pools);
        _cursePage = new("Available Curses", _mainPage);

        // Places the general settings in a row (besides the enable button, which should be above).
        GridItemPanel generalPanel = new(_mainPage, new(0f, 400f), 3, 500, 400, false, _generalFactory.ElementLookup["CursedWallet"], 
            _generalFactory.ElementLookup["CursedVessel"],
            _generalFactory.ElementLookup["CursedDreamNail"]);
        new VerticalItemPanel(_mainPage, new(0f, 450f), 120f, true,
        [
            _generalFactory.ElementLookup["Enabled"],
            generalPanel,
            _generalFactory.ElementLookup["UseCurses"]
        ]);

        // Place the elements for curse settings.
        SmallButton cursePageButton = new(_mainPage, "Available Curses");
        cursePageButton.AddHideAndShowEvent(_cursePage);
        cursePageButton.MoveTo(new(-600f, 0f));
        MenuLabel replaceableItemsLabel = new(_mainPage, "Replacable Items", MenuLabel.Style.Title);
        replaceableItemsLabel.MoveTo(new(300f, 300f));
        VerticalItemPanel controlSettings = new(_mainPage, new(0f, 100f), 80f, true, _controlFactory.Elements);
        VerticalItemPanel replacableSettings = new(_mainPage, new(400f, 200f), 40f, true, new IMenuElement[] { replaceableItemsLabel }.Concat(_poolFactory.Elements).ToArray());

        _generalFactory.ElementLookup["UseCurses"].SelfChanged += (self) =>
        {
            if ((bool)self.Value)
            {
                cursePageButton.Show();
                controlSettings.Show();
                if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.Add)
                    replacableSettings.Show();
                else
                    replacableSettings.Hide();
            }
            else
            {
                cursePageButton.Hide();
                controlSettings.Hide();
                replacableSettings.Hide();
            }
        };
        _controlFactory.ElementLookup["CurseMethod"].SelfChanged += (self) =>
        {
            if ((RequestMethod)self.Value != RequestMethod.Add)
            {
                replacableSettings.Show();
                _controlFactory.ElementLookup["TakeReplaceGroup"].Show();
            }
            else
            { 
                replacableSettings.Hide();
                _controlFactory.ElementLookup["TakeReplaceGroup"].Hide();
            }
        };
        _controlFactory.ElementLookup["CurseAmount"].SelfChanged += (self) =>
        {
            if ((Amount)self.Value == Amount.Custom && CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
                _controlFactory.ElementLookup["CurseItems"].Show();
            else
                _controlFactory.ElementLookup["CurseItems"].Hide();
        };

        // Adjust the view
        if (CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
        {
            cursePageButton.Show();
            controlSettings.Show();
            if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.Add)
                replacableSettings.Show();
            else
                replacableSettings.Hide();
        }
        else
        {
            cursePageButton.Hide();
            controlSettings.Hide();
            replacableSettings.Hide();
        }

        if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod != RequestMethod.Add)
        {
            replacableSettings.Show();
            _controlFactory.ElementLookup["TakeReplaceGroup"].Show();
        }
        else
        {
            replacableSettings.Hide();
            _controlFactory.ElementLookup["TakeReplaceGroup"].Hide();
        }

        if (CurseRandomizer.Instance.Settings.CurseControlSettings.CurseAmount == Amount.Custom && CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses)
            _controlFactory.ElementLookup["CurseItems"].Show();
        else
            _controlFactory.ElementLookup["CurseItems"].Hide();

        _curseSettings = new(_cursePage, CurseRandomizer.Instance.Settings.Curses);
        new GridItemPanel(_cursePage, new(0f, 400f), 5, 150, 400, true, [.. _curseSettings.Elements]);
    }

    private bool HandleButton(MenuPage previousPage, out SmallButton connectionButton)
    {
        SmallButton button = new(previousPage, "Curse Randomizer");
        button.AddHideAndShowEvent(previousPage, _mainPage);
        _mainPage.BeforeGoBack += () => button.Text.color = !CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? Colors.DEFAULT_COLOR : Colors.TRUE_COLOR;
        button.Text.color = !CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? Colors.DEFAULT_COLOR : Colors.TRUE_COLOR;
        connectionButton = button;
        return true;
    }

    #endregion

    /// <summary>
    /// Attach the menu to the randomizer.
    /// </summary>
    public static void AttachMenu()
    {
        RandomizerMenuAPI.AddMenuPage(Instance.ConstructMenu, Instance.HandleButton);
        MenuChangerMod.OnExitMainMenu += () => _instance = null;
    }

    internal void UpdateMenuSettings(RandoSettings settings)
    {
        if (settings == null)
            _generalFactory.ElementLookup[nameof(CurseRandomizer.Instance.Settings.GeneralSettings.Enabled)].SetValue(false);
        else
        {
            _generalFactory.SetMenuValues(settings.GeneralSettings);
            _controlFactory.SetMenuValues(settings.CurseControlSettings);
            _poolFactory.SetMenuValues(settings.Pools);
            _curseSettings.SetMenuValues(settings.Curses);
        }
    }
}
