using CurseRandomizer.Curses;
using CurseRandomizer.Enums;
using CurseRandomizer.Helper;
using Modding;
using System.Collections;
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

    #endregion

    #region Properties

    public abstract int CurrentAmount { get; set; }

    public abstract int NeededAmount { get; }

    public override CurseTag Tag => CurseTag.Temporarly;

    public float TrackPosition { get; set; }

    public GameObject Tracker
    {
        get
        {
            if (_tracker == null)
            {
                GameObject prefab = GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Inv/Inv_Items/Geo").gameObject;
                GameObject hudCanvas = GameObject.Find("_GameCameras").transform.Find("HudCamera/Hud Canvas").gameObject;
                _tracker = GameObject.Instantiate(prefab, hudCanvas.transform, true);
                _tracker.name = Name + " Tracker";
                _tracker.transform.localPosition = new(7.7818f, 0.5418f, 0);
                _tracker.transform.localScale = new(1.3824f, 1.3824f, 1.3824f);
                _tracker.GetComponent<DisplayItemAmount>().playerDataInt = _tracker.name;
                _tracker.GetComponent<DisplayItemAmount>().textObject.text = "";
                _tracker.GetComponent<DisplayItemAmount>().textObject.fontSize = 3;
                _tracker.GetComponent<DisplayItemAmount>().textObject.gameObject.name = "Counter";
                _tracker.GetComponent<SpriteRenderer>().sprite = SpriteHelper.CreateSprite(Name == "Despair" ? "Fool" : Name);
                _tracker.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1f);
                _tracker.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0f);
                _tracker.SetActive(IsActive() || (OmenCurse.OmenMode && Type == CurseType.Omen));
            }
            return _tracker;
        }
    }

    public static CurseCounterPosition Position { get; set; }

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
        RepositionTracker();
        TextMeshPro currentCounter = _tracker.GetComponent<DisplayItemAmount>().textObject;
        if (DespairCurse.DespairActive && Type != CurseType.Despair)
            currentCounter.text = $"<color={TextColor}>{CurrentAmount}/{NeededAmount}</color>";
        else
            currentCounter.text = $"{CurrentAmount}/{NeededAmount}";
        if (CurrentAmount >= NeededAmount && (!DespairCurse.DespairActive || Type == CurseType.Despair))
            LiftCurse();
    }

    /// <summary>
    /// Disables the tracker and displays the message in a box.
    /// </summary>
    protected virtual void LiftCurse()
    {
        _tracker.SetActive(false);
        DisplayMessage("Vanish");
    }

    internal void RepositionTracker() => Tracker.transform.position = MoveToPosition(Position);

    private IEnumerator Wait()
    {
        yield return new WaitUntil(() => HeroController.instance != null && HeroController.instance.acceptingInput
        && GameManager.instance != null && GameManager.instance.soulVessel_fsm != null && GameManager.instance.soulVessel_fsm.Active);
        Tracker.SetActive(IsActive() || (OmenCurse.OmenMode && Type == CurseType.Omen));
        if (Tracker.activeSelf)
            UpdateProgression();
    }

    protected abstract Vector2 MoveToPosition(CurseCounterPosition position);

    #endregion
}
