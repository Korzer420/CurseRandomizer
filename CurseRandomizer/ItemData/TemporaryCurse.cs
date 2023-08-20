using CurseRandomizer.Curses;
using CurseRandomizer.Enums;
using ItemChanger;
using KorzUtils.Helper;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CurseRandomizer.ItemData;

/// <summary>
/// A curse which vanishes after a certain condition is met.
/// </summary>
internal abstract class TemporaryCurse : Curse
{
    #region Members

    protected GameObject _tracker;
    private static GameObject _trackerContainer;
    private static Coroutine _previewRoutine;

    #endregion

    #region Properties

    public abstract int CurrentAmount { get; set; }

    public abstract int NeededAmount { get; }

    public override CurseTag Tag => CurseTag.Temporarly;

    public static Vector3 TrackerPosition { get; set; }

    public static float Scale { get; set; } = 1f;

    public static GameObject TrackerContainer 
    {
        get 
        { 
            if (_trackerContainer == null)
            {
                GameObject hudCanvas = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas").gameObject;
                _trackerContainer = new("Container");
                _trackerContainer.transform.SetParent(hudCanvas.transform);
                _trackerContainer.transform.localScale = new(Scale, Scale, 1f);
                _trackerContainer.transform.position = TrackerPosition;
                _trackerContainer.layer = hudCanvas.layer;
                _trackerContainer.SetActive(true);
                _trackerContainer.isStatic = false;
            }
            return _trackerContainer;
        }
    }

    public GameObject Tracker
    {
        get
        {
            if (_tracker == null)
            {
                GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo").gameObject;
                _tracker = GameObject.Instantiate(prefab, TrackerContainer.transform, true);
                _tracker.name = Name + " Tracker";
                _tracker.transform.localPosition = new(0f, 0f, 0);
                _tracker.transform.localScale = new(1.3824f, 1.3824f, 1.3824f);
                _tracker.GetComponent<DisplayItemAmount>().playerDataInt = _tracker.name;
                _tracker.GetComponent<DisplayItemAmount>().textObject.text = "";
                _tracker.GetComponent<DisplayItemAmount>().textObject.fontSize = 3;
                _tracker.GetComponent<DisplayItemAmount>().textObject.gameObject.name = "Counter";
                _tracker.GetComponent<SpriteRenderer>().sprite = SpriteHelper.CreateSprite<CurseRandomizer>(Name == "Despair" ? "Sprites.Fool" : "Sprites." + Name);
                _tracker.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1f);
                _tracker.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0f);
                _tracker.SetActive(IsActive() || (OmenCurse.OmenMode && Type == CurseType.Omen));
            }
            return _tracker;
        }
    }

    public static CurseCounterPosition Position { get; set; }

    public static bool EasyLift { get; set; }

    #endregion

    #region Event handler

    private void DisplayItemAmount_OnEnable(On.DisplayItemAmount.orig_OnEnable orig, DisplayItemAmount self)
    {
        orig(self);
        if (self.playerDataInt == Name)
            UpdateProgression();
    }

    #endregion

    #region Control

    public override void ApplyCurse()
    {
        Tracker.SetActive(true);
        UpdateProgression();
    }

    public override void ApplyHooks()
    {
        On.DisplayItemAmount.OnEnable += DisplayItemAmount_OnEnable;
        CurseManager.Handler.StartCoroutine(Wait());
        
    }

    public override void Unhook()
    {
        if (_tracker != null)
            GameObject.Destroy(_tracker);
        On.DisplayItemAmount.OnEnable -= DisplayItemAmount_OnEnable;
    }

    #endregion

    #region Methods

    protected abstract bool IsActive();

    /// <summary>
    /// Updates the progression and the tracker. Calls <see cref="LiftCurse"/>.
    /// </summary>
    internal virtual void UpdateProgression()
    {
        if (GameManager.instance?.IsGameplayScene() == true)
        {
            RepositionTracker();
            TextMeshPro currentCounter = _tracker.GetComponent<DisplayItemAmount>().textObject;
            if (DespairCurse.DespairActive && Type != CurseType.Despair)
                currentCounter.text = $"<color={TextColor}>{CurrentAmount}/{NeededAmount}</color>";
            else
                currentCounter.text = $"{CurrentAmount}/{NeededAmount}";
            if (CurrentAmount >= NeededAmount && !DespairCurse.DespairActive)
                LiftCurse();
        }
    }

    /// <summary>
    /// Disables the tracker and displays the message in a box.
    /// </summary>
    protected virtual void LiftCurse()
    {
        _tracker.SetActive(false);
        GameHelper.DisplayMessage("The curse of " + Type + " vanished");
    }

    internal void RepositionTracker() => Tracker.transform.localPosition = MoveToPosition(Position);

    private IEnumerator Wait()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput
        && GameManager.instance != null && GameManager.instance.soulVessel_fsm != null && GameManager.instance.soulVessel_fsm.Active);
        Tracker.SetActive(IsActive() || (OmenCurse.OmenMode && Type == CurseType.Omen));
        if (Tracker.activeSelf)
            UpdateProgression();
    }

    protected abstract Vector2 MoveToPosition(CurseCounterPosition position);

    internal static void AdjustTracker()
    {
        TrackerContainer.transform.position = TrackerPosition;
        TrackerContainer.transform.localScale = new(Scale, Scale, 1f);
        if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
        {
            foreach (TemporaryCurse curse in CurseManager.GetCurses().Where(x => x is TemporaryCurse))
            {
                curse.RepositionTracker();
                if (!curse.IsActive())
                {
                    curse.Tracker.SetActive(true);
                    curse.Tracker.GetComponent<DisplayItemAmount>().textObject.text = "7777/7777";
                }
            }
            if (_previewRoutine is not null)
                CurseManager.Handler.StopCoroutine(_previewRoutine);
            _previewRoutine = CurseManager.Handler?.StartCoroutine(DisablePreview());
        }
    }

    private static IEnumerator DisablePreview()
    {
        float passedTime = 0f;
        while (passedTime <= 3f)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        foreach (TemporaryCurse curse in CurseManager.GetCurses().Where(x => x is TemporaryCurse))
        {
            try
            {
                if (!curse.IsActive())
                    curse.Tracker.SetActive(false);
            }
            catch (Exception exception)
            {
                LogHelper.Write<CurseRandomizer>("Failed to disable preview for curse: " + curse.Name, exception);
            }
        }
    }

    #endregion
}
