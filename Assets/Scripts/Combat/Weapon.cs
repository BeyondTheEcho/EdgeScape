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
        [SerializeField] private float m_WeaponRange = 1.5f;
        [SerializeField] private float m_WeaponDamage = 5.0f;
        [SerializeField] private bool m_isRightHanded = true;

        public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            Transform spawnPos = m_isRightHanded ? rightHandTransform : leftHandTransform;

            if (m_WeaponPrefab != null)
            {
                Instantiate(m_WeaponPrefab, spawnPos);
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
