using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText m_DamageTextPrefab;

        public void Spawn(float damageValue)
        {
            DamageText instance = Instantiate<DamageText>(m_DamageTextPrefab, transform);
            instance.SetDamage(damageValue);
        }

    }

}