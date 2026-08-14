using CurseRandomizer.Curses;
using System.Collections;
using UnityEngine;

namespace CurseRandomizer.Components;

internal class TraumaCounter : MonoBehaviour
{
    private TraumaCurse TraumaCurse => CurseManager.GetCurse<TraumaCurse>();
    private int _passedTime = 0;

    public int NeededTime => Mathf.Max(10, 61 - TraumaCurse.Data.CastedAmount * 2);

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
            TraumaCurse.CurrentAmount++;
            TraumaCurse.UpdateProgression();
            if (TraumaCurse.CurrentAmount == -1)
                Destroy(this);
            else if (_passedTime >= NeededTime)
            {
                for (int i = 0; i < TraumaCurse.Data.DespairEnhanced + 1; i++)
                {
                    GameObject aspid = GameObject.Instantiate(AspidPrefab, HeroController.instance.transform.position + new Vector3(0f, 4f), Quaternion.identity);
                    aspid.SetActive(true);
                }
                
                _passedTime = 0;
            }
        }
    }
}
