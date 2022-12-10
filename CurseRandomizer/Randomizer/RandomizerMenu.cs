using CurseRandomizer.Randomizer.Settings;
using ItemChanger.Extensions;
using MenuChanger;
using MenuChanger.Attributes;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Modding;
using RandomizerMod.Menu;
using RandomizerMod.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private Dictionary<string, VerticalItemPanel> _curseSettings;

    private IMenuElement[] _additionalElements = new IMenuElement[3];

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
        GridItemPanel generalPanel = new(_mainPage, new(0f, 400f), 5, 500, 400, true, _generalFactory.Elements.Skip(1).ToArray());
        new VerticalItemPanel(_mainPage, new(0f, 400f), 120f, true, new IMenuElement[] { _generalFactory.ElementLookup["Enabled"], generalPanel });

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
                replacableSettings.Show();
            else
                replacableSettings.Hide();
        };
        _controlFactory.ElementLookup["CurseAmount"].SelfChanged += (self) =>
        {
            if ((Amount)self.Value == Amount.Custom)
                _controlFactory.ElementLookup["CurseItems"].Show();
            else
                _controlFactory.ElementLookup["CurseItems"].Hide();
        };

        // We trigger the event handler instantly to adjust the view.
        _generalFactory.ElementLookup["UseCurses"].SetValue(CurseRandomizer.Instance.Settings.GeneralSettings.UseCurses);
        _controlFactory.ElementLookup["CurseMethod"].SetValue(CurseRandomizer.Instance.Settings.CurseControlSettings.CurseMethod);
        _controlFactory.ElementLookup["CurseAmount"].SetValue(CurseRandomizer.Instance.Settings.CurseControlSettings.CurseAmount);

        //-------------------------- Create the sub curse page. -----------------------------------------

        _curseSettings = new();
        // Delete all curse settings which don't have a matching curse.
        CurseRandomizer.Instance.Settings.CurseSettings.RemoveAll(x => CurseManager.GetCurseByName(x.Name) is null);

        ToggleButton capEffects = new(_cursePage, "Cap Effects");
        capEffects.SelfChanged += (self) =>
        {
            if ((bool)self.Value)
                foreach (VerticalItemPanel verticalItemPanel in _curseSettings.Values)
                {
                    if ((verticalItemPanel.Items[0] as ToggleButton).Value)
                        verticalItemPanel.Items[1].Show();
                    else
                        verticalItemPanel.Items[1].Hide();
                }
            else
                foreach (VerticalItemPanel verticalItemPanel in _curseSettings.Values)
                    verticalItemPanel.Items[1].Hide();
            CurseRandomizer.Instance.Settings.CurseControlSettings.CapEffects = (bool)self.Value;
        };
        capEffects.MoveTo(new(-250f, 450f));
        capEffects.SetValue(CurseRandomizer.Instance.Settings.CurseControlSettings.CapEffects);

        MenuItem<string> defaultCurse = new(_cursePage, "Default Curse", CurseManager.GetCurses().Select(x => x.Name).ToArray());
        defaultCurse.SelfChanged += (self) =>
        {
            string selectedCurse = (string)self.Value;
            if (!CurseRandomizer.Instance.Settings.CurseSettings.First(x => x.Name == selectedCurse).Active)
            {
                if (!CurseRandomizer.Instance.Settings.CurseSettings.Any(x => x.Active))
                {
                    if (selectedCurse != "Pain")
                        defaultCurse.SetValue("Pain");
                    else
                        CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse = "Pain";
                }
                else
                    defaultCurse.MoveNext();
            }
            else
                CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse = selectedCurse;
        };
        defaultCurse.MoveTo(new(250f, 450f));
        defaultCurse.SetValue(CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse ?? "Pain");

        // Create a toggle button and a cap field for each curse. Curses unknown to the settings will be added.
        foreach (Curse curse in CurseManager.GetCurses())
        {
            CurseSettings settings = CurseRandomizer.Instance.Settings.CurseSettings.FirstOrDefault(x => x == curse);
            if (settings == null)
            {
                settings = new() { Name = curse.Name, Cap = curse.SetCap(1) };
                CurseRandomizer.Instance.Settings.CurseSettings.Add(settings);
            }

            ToggleButton curseEnable = new(_cursePage, curse.Name);
            EntryField<int> curseCap = new(_cursePage, "Cap");

            curseEnable.SelfChanged += (self) =>
            {
                if ((bool)self.Value && CurseRandomizer.Instance.Settings.CurseControlSettings.CapEffects)
                    curseCap.Show();
                else
                    curseCap.Hide();
                // Only allow active curses to be default. (Except pain curse)
                if (CurseRandomizer.Instance.Settings.CurseControlSettings.DefaultCurse == curseEnable.Name)
                    defaultCurse.MoveNext();
            };
            curseEnable.Bind(settings, ReflectionHelper.GetPropertyInfo(typeof(CurseSettings), "Active"));

            curseCap.ValueChanged += (value) =>
            {
                int valueToSet = curse.SetCap(value);
                if (value != valueToSet)
                    curseCap.SetValue(valueToSet);
            };
            curseCap.Bind(settings, ReflectionHelper.GetPropertyInfo(typeof(CurseSettings), "Cap"));
            _curseSettings.Add(curse.Name, new(_cursePage, new(0f, 0f), 100, false, new IMenuElement[2] { curseEnable, curseCap }));
        }
        new GridItemPanel(_cursePage, new(0f, 400f), 5, 150, 400, true, _curseSettings
            .Where(x => CurseManager.GetCurseByName(x.Key).Type != CurseType.Custom)
            .Select(x => x.Value)
            .ToArray());

        GridItemPanel customCursePanel = new(_cursePage, new(0f, -200f), 5, 200, 400, false, _curseSettings
            .Where(x => CurseManager.GetCurseByName(x.Key).Type == CurseType.Custom)
            .Select(x => x.Value)
            .ToArray());

        if (!customCursePanel.Items.Any())
        {
            MenuLabel label = new(_cursePage, "No custom curses available.", MenuLabel.Style.Title);
            label.MoveTo(new(0f, -300f));
        }

        ToggleButton customCurses = new(_cursePage, "Custom Curses");
        customCurses.SelfChanged += (self) =>
        {
            CurseRandomizer.Instance.Settings.CurseControlSettings.CustomCurses = (bool)self.Value;
            if ((bool)self.Value)
                customCursePanel.Show();
            else
                customCursePanel.Hide();
        };
        customCurses.SetValue(CurseRandomizer.Instance.Settings.CurseControlSettings.CustomCurses);
        customCurses.MoveTo(new(0f, -100f));

        _additionalElements[0] = capEffects;
        _additionalElements[1] = defaultCurse;
        _additionalElements[2] = customCurses;
    }

    private bool HandleButton(MenuPage previousPage, out SmallButton connectionButton)
    {
        SmallButton button = new(previousPage, "Curse Randomizer");
        button.AddHideAndShowEvent(previousPage, _mainPage);
        _mainPage.BeforeGoBack += () => button.Text.color = !CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? Colors.FALSE_COLOR : Colors.TRUE_COLOR;
        button.Text.color = !CurseRandomizer.Instance.Settings.GeneralSettings.Enabled ? Colors.FALSE_COLOR : Colors.TRUE_COLOR;
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

            foreach (CurseSettings curseSettings in settings.CurseSettings)
                if (_curseSettings.ContainsKey(curseSettings.Name))
                {
                    (_curseSettings[curseSettings.Name].Items[0] as ToggleButton).SetValue(curseSettings.Active);
                    (_curseSettings[curseSettings.Name].Items[1] as EntryField<int>).SetValue(curseSettings.Cap);
                }

            (_additionalElements[0] as ToggleButton).SetValue(settings.CurseControlSettings.CapEffects);
            (_additionalElements[1] as MenuItem<string>).SetValue(settings.CurseControlSettings.DefaultCurse);
            (_additionalElements[2] as ToggleButton).SetValue(settings.CurseControlSettings.CustomCurses);
        }
    }
}
