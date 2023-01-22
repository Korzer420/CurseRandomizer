using CurseRandomizer.Curses;
using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CurseRandomizer.Components;

internal class DespairCounter : MonoBehaviour
{
    private string _enemyName;
    private DespairCurse _curse = CurseManager.GetCurseByType(CurseType.Despair) as DespairCurse;

    void Start()
    {
        EnemyDeathEffects enemyDeathEffects = GetComponent<EnemyDeathEffects>();
        if (enemyDeathEffects == null)
        {
            Destroy(this);
            return;
        }
        _enemyName = ReflectionHelper.GetField<EnemyDeathEffects, string>(enemyDeathEffects, "playerDataName");
        On.HealthManager.TakeDamage += HealthManager_TakeDamage;
    }

    private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        orig(self, hitInstance);
        if (self.gameObject == gameObject)
        {
            if (_curse.CurrentAmount == -1)
            {
                Destroy(this);
                return;
            }
            Dictionary<string, int> enemyList = _curse.Data.AdditionalData as Dictionary<string, int>;
            if (!enemyList.ContainsKey(_enemyName))
                enemyList.Add(_enemyName, 0);
            if (hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.Spell)
            { 
                enemyList[_enemyName] = Math.Min(100, enemyList[_enemyName] + hitInstance.DamageDealt);
                _curse.UpdateProgression();
            }
        }
    }

    void OnDestroy()
    {
        On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
    }
}
