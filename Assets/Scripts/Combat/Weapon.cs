using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "EdgeScape/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController m_AnimatorOverride;
        [SerializeField] private GameObject m_WeaponPrefab;

        //Weapon Stats
        [SerializeField] private float m_WeaponRange = 0.1f;
        [SerializeField] private float m_WeaponDamage = 5.0f;

        public void Spawn(Transform handTransform, Animator animator)
        {
            if (m_WeaponPrefab != null)
            {
                Instantiate(m_WeaponPrefab, handTransform);
            }

            if (m_AnimatorOverride != null)
            {
                animator.runtimeAnimatorController = m_AnimatorOverride;
            }
        }

        public float GetWeaponRange()
        {
            return m_WeaponRange;
        }

        public float GetWeaponDamage()
        {
            return m_WeaponDamage;
        }
    }
}
