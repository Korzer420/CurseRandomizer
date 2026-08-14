using UnityEngine;

namespace CurseRandomizer.Components;

internal class ConfusionCooldown : MonoBehaviour
{
    private float _timer = 0f;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 60f)
            Destroy(this);
    }
}
