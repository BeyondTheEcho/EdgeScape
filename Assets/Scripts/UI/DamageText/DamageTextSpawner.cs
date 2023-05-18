using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText m_DamageTextPrefab;

        void Start()
        {
            Spawn(11f);
        }

        public void Spawn(float damageValue)
        {
            DamageText instance = Instantiate<DamageText>(m_DamageTextPrefab, transform);
            instance.SetDamage(damageValue);
        }

    }

}