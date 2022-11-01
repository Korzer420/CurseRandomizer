using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseRandomizer.Randomizer;

internal class RandomizerMenu
{
    private static RandomizerMenu _instance;

    private MenuPage _mainPage;

    private MenuElementFactory<RandoSettings> _factory;

    private GridItemPanel _curseOption;

    private NumericEntryField<int> _curseAmount;

    /// <summary>
    /// Gets the instance of the menu.
    /// </summary>
    public static RandomizerMenu Instance => _instance ??= new();

    #region Event handler

    private void ConstructMenu(MenuPage previousPage)
    {
        _mainPage = new("Curse Randomizer", previousPage);
        _factory = new(_mainPage, CurseRandomizer.Instance.Settings);
        IValueElement[][] elements = new IValueElement[][]{
            // Enable Button
            new IValueElement[]{_factory.Elements[0]},
            // Settings
            _factory.Elements.Skip(1).Take(5).ToArray(),
            // Curse main settings
            _factory.Elements.Skip(6).Take(3).ToArray(),
            // Allowed pools
            _factory.Elements.Skip(10).Take(10).ToArray(),
            // Allowed curses
            _factory.Elements.Skip(20).ToArray()
        };
        // Change curse option visibility
        elements[1][2].SelfChanged += ChangeCurse;
        elements[2][2].SelfChanged += ToggleCustomCurseAmount;
        _curseAmount = _factory.Elements[9] as NumericEntryField<int>;
        _curseAmount.MoveTo(new(0f, -200f));
        if (CurseRandomizer.Instance.Settings.CurseAmount == Amount.Custom)
            _curseAmount.Show();
        else
            _curseAmount.Hide();
        GridItemPanel mainSettings = new(_mainPage, new(0, 320f), 5, 0, 300, false, elements[1]);
        new VerticalItemPanel(_mainPage, new(0f, 400f), 140f, false, new IMenuElement[] { elements[0][0], mainSettings});
        VerticalItemPanel[] curseSettings = new VerticalItemPanel[3];
        curseSettings[1] = new VerticalItemPanel(_mainPage, new(0, 0f), 50f, false, 
            new IMenuElement[] { new MenuLabel(_mainPage, "Replacable Pools", MenuLabel.Style.Body) }.Concat(elements[2]).ToArray());
        curseSettings[0] = new VerticalItemPanel(_mainPage, new(0f, 0f), 50f, false, elements[3]);
        curseSettings[2] = new VerticalItemPanel(_mainPage, new(0f, 0f), 50f, false, 
            new IMenuElement[] { new MenuLabel(_mainPage, "Placable Curses", MenuLabel.Style.Body)}.Concat(elements[4]).ToArray());
        _curseOption = new(_mainPage, new(0f, 100f), 3, 0f, 400f, true, curseSettings);
        if (!CurseRandomizer.Instance.Settings.UseCurses)
            _curseOption.Hide();
    }

    private void ToggleCustomCurseAmount(IValueElement obj)
    {
        if ((Amount)obj.Value == Amount.Custom)
            _curseAmount.Show();
        else
            _curseAmount.Hide();
    }

    private void ChangeCurse(IValueElement obj)
    {
        if ((bool)obj.Value)
            _curseOption.Show();
        else
            _curseOption.Hide();
    }

    private bool HandleButton(MenuPage previousPage, out SmallButton connectionButton)
    {
        SmallButton button = new(previousPage, "Curse Randomizer");
        button.AddHideAndShowEvent(previousPage, _mainPage);
        _mainPage.BeforeGoBack += () => button.Text.color = !CurseRandomizer.Instance.Settings.Enabled ? Colors.FALSE_COLOR : Colors.TRUE_COLOR;
        button.Text.color = !CurseRandomizer.Instance.Settings.Enabled ? Colors.FALSE_COLOR : Colors.TRUE_COLOR;
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
}
