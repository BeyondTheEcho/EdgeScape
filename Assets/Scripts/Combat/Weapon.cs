using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "EdgeScape/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController m_AnimatorOverride;
        [SerializeField] private GameObject m_WeaponPrefab;
        [SerializeField] private Projectile m_ProjectilePrefab;

        //Weapon Stats
        [SerializeField] private float m_WeaponRange = 1.5f;
        [SerializeField] private float m_WeaponDamage = 5.0f;
        [SerializeField] private bool m_IsRightHanded = true;


        public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            Transform spawnPos = GetTransform(rightHandTransform, leftHandTransform);

            if (m_WeaponPrefab != null)
            {
                Instantiate(m_WeaponPrefab, spawnPos);
            }

            if (m_AnimatorOverride != null)
            {
                animator.runtimeAnimatorController = m_AnimatorOverride;
            }
        }

        //Returns the transform of the correct hand based on the status of the m_IsRightHanded Bool
        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            return m_IsRightHanded ? rightHandTransform : leftHandTransform;
        }

        public bool HasProjectile()
        {
            return m_ProjectilePrefab != null;
        }

        public void LaunchProjectile(Transform rHand, Transform lHand, Health target)
        {
            Projectile projectileInst = Instantiate(m_ProjectilePrefab, GetTransform(rHand, lHand).position, Quaternion.identity);
            projectileInst.SetTarget(target);
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
