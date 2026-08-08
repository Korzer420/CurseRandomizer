using CurseRandomizer.Curses;
using System.Collections;
using UnityEngine;

namespace CurseRandomizer.Components;

internal class TraumaCounter : MonoBehaviour
{
    private TraumaCurse _curse => CurseManager.GetCurse<TraumaCurse>();
    private int _passedTime = 0;

    public int NeededTime => Mathf.Max(10, 61 - _curse.Data.CastedAmount * 2);

    public static GameObject AspidPrefab { get; set; }

    void Start() => StartCoroutine(PassTime());

    private IEnumerator PassTime()
    {
        while (true)
        {
            if (GameManager.instance?.IsGameplayScene() == false || GameManager.instance?.IsGamePaused() == true)
                yield return new WaitUntil(() => GameManager.instance?.IsGameplayScene() == true && GameManager.instance?.IsGamePaused() == false);
            yield return new WaitForSeconds(1f);
            _passedTime++;
            _curse.CurrentAmount++;
            _curse.UpdateProgression();
            if (_curse.CurrentAmount == -1)
                Destroy(this);
            else if (_passedTime >= NeededTime)
            {
                GameObject aspid = GameObject.Instantiate(AspidPrefab, HeroController.instance.transform.position + new Vector3(0f, 4f), Quaternion.identity);
                aspid.SetActive(true);
                _passedTime = 0;
            }
        }
    }
}
