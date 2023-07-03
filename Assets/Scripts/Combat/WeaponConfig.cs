using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using RPG.Inventories;
using RPG.Stats;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Inventory/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [Header("Weapon Config")]
        [SerializeField] private AnimatorOverrideController m_AnimatorOverride;
        [SerializeField] private Weapon m_WeaponPrefab;
        [SerializeField] private Projectile m_ProjectilePrefab;
        [SerializeField] private bool m_IsRightHanded = true;

        [Header("Weapon Stats Config")]
        [SerializeField] private float m_WeaponRange = 1.5f;
        [SerializeField] private float m_WeaponDamage = 5.0f;
        [SerializeField] private float m_WeaponPercentageBonus = 0f;

        [Header("Weapon SFX Config")]
        [SerializeField] private AudioClip[] m_Clips;

        //Private Vars
        private int m_ClipIndex = 0;
        private const string m_WeaponName = "Weapon";

        //------------------------------------------------------------
        //                Public Functions
        //------------------------------------------------------------

        public AudioClip GetSFX()
        {
            if (m_Clips.Length == 0) return null;

            if (m_ClipIndex > m_Clips.Length - 1) m_ClipIndex = 0;

            var sfx = m_Clips[m_ClipIndex];

            m_ClipIndex++;

            return sfx;
        }

        public Weapon Spawn(Transform rHand, Transform lHand, Animator animator)
        {
            DestroyOldWeapon(rHand, lHand);

            Weapon weapon = null;

            if (m_WeaponPrefab != null)
            {
                Transform spawnPos = GetTransform(rHand, lHand);
                weapon = Instantiate(m_WeaponPrefab, spawnPos);
                weapon.gameObject.name = m_WeaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (m_AnimatorOverride != null)
            {
                animator.runtimeAnimatorController = m_AnimatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        public bool HasProjectile()
        {
            return m_ProjectilePrefab != null;
        }

        public void LaunchProjectile(Transform rHand, Transform lHand, Health target, GameObject instigator, float calcedDamage)
        {
            Projectile projectileInst = Instantiate(m_ProjectilePrefab, GetTransform(rHand, lHand).position, Quaternion.identity);
            projectileInst.SetTarget(target, instigator, calcedDamage);
        }

        public float GetWeaponRange()
        {
            return m_WeaponRange;
        }

        public float GetWeaponDamage()
        {
            return m_WeaponDamage;
        }

        public float GetWeaponPercentageBonus()
        {
            return m_WeaponPercentageBonus;
        }

        //------------------------------------------------------------
        //                Private Functions
        //------------------------------------------------------------

        private void DestroyOldWeapon(Transform rHand, Transform lHand)
        {
            Transform oldWeapon = rHand.Find(m_WeaponName);

            if (oldWeapon == null)
            {
                oldWeapon = lHand.Find(m_WeaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        //Returns the transform of the correct hand based on the status of the m_IsRightHanded Bool
        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            return m_IsRightHanded ? rightHandTransform : leftHandTransform;
        }

        //------------------------------------------------------------
        //         IModifierProvider Implemented Functions
        //------------------------------------------------------------

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return m_WeaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return m_WeaponPercentageBonus;
            }
        }
    }
}
