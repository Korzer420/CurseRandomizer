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

    private GridItemPanel _curseOptions;

    private NumericEntryField<int> _curseAmount;

    private VerticalItemPanel _capPanel;

    private MenuLabel[] _curseLabels = new MenuLabel[2];

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
            _factory.Elements.Skip(6).Take(5).ToArray(),
            // Allowed curses
            _factory.Elements.Skip(12).Take(10).ToArray(),
            // Allowed pools
            _factory.Elements.Skip(22).Take(9).ToArray(),
            // Curse caps
            _factory.Elements.Skip(31).ToArray()
        };
        // Change curse options visibility
        elements[1][2].SelfChanged += ChangeCurse;
        elements[2][1].SelfChanged += ToggleCapVisibilty;
        elements[2][2].SelfChanged += element =>
        {
            if ((CurseType)element.Value == CurseType.Custom)
                element.SetValue(CurseType.Pain);
        };
        // Change custom curse amount visiblity
        elements[2][4].SelfChanged += ToggleCustomCurseAmount;

        _curseAmount = _factory.Elements[11] as NumericEntryField<int>;
        _curseAmount.MoveTo(new(0f, -200f));
        if (CurseRandomizer.Instance.Settings.CurseAmount == Amount.Custom)
            _curseAmount.Show();
        else
            _curseAmount.Hide();
        GridItemPanel mainSettings = new(_mainPage, new(0, 320f), 5, 0, 300, false, elements[1]);
        new VerticalItemPanel(_mainPage, new(0f, 400f), 140f, false, new IMenuElement[] { elements[0][0], mainSettings });
        VerticalItemPanel[] curseSettings = new VerticalItemPanel[3];
        // Available curses
        curseSettings[0] = new VerticalItemPanel(_mainPage, new(0f, 0f), 50f, false, elements[3]);
        // Main curse settings
        curseSettings[1] = new VerticalItemPanel(_mainPage, new(0, 0f), 50f, false, elements[2]);
        // Available item pools.
        curseSettings[2] = new VerticalItemPanel(_mainPage, new(0f, 0f), 50f, false, elements[4]);
        _capPanel = new(_mainPage, new(-700f, 100f), 75f, false, elements[5]);
        _curseOptions = new(_mainPage, new(0f, 100f), 3, 0f, 400f, true, curseSettings);

        MenuLabel poolLabel = new(_mainPage, "Replaceable Items", MenuLabel.Style.Body);
        poolLabel.MoveTo(new(975f, 150f));
        _curseLabels[0] = poolLabel;

        MenuLabel availableCurses = new(_mainPage, "Available Curses", MenuLabel.Style.Body);
        availableCurses.MoveTo(new(175f, 150f));
        _curseLabels[1] = availableCurses;

        if (!CurseRandomizer.Instance.Settings.UseCurses)
        {
            _curseOptions.Hide();
            _capPanel.Hide();
            availableCurses.Hide();
            poolLabel.Hide();
            _curseAmount.Hide();
        }
        else if (!CurseRandomizer.Instance.Settings.CapEffects)
            _capPanel.Hide();
    }

    private void ToggleCapVisibilty(IValueElement useCaps)
    {
        if ((bool)useCaps.Value)
            _capPanel.Show();
        else
            _capPanel.Hide();
    }

    private void ToggleCustomCurseAmount(IValueElement curseAmount)
    {
        if ((Amount)curseAmount.Value == Amount.Custom)
            _curseAmount.Show();
        else
            _curseAmount.Hide();
    }

    private void ChangeCurse(IValueElement useCurses)
    {
        if ((bool)useCurses.Value)
        {
            _curseOptions.Show();
            if (CurseRandomizer.Instance.Settings.CapEffects)
                _capPanel.Show();
            else
                _capPanel.Hide();
            _curseLabels[0].Show();
            _curseLabels[1].Show();
            if (CurseRandomizer.Instance.Settings.CurseAmount == Amount.Custom)
                _curseAmount.Show();
            else
                _curseAmount.Hide();
        }
        else
        {
            _curseOptions.Hide();
            _capPanel.Hide();
            _curseLabels[0].Hide();
            _curseLabels[1].Hide();
            _curseAmount.Hide();
        }
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
