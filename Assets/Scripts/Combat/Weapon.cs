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

        public void Spawn(Transform handTransform, Animator animator)
        {
            Instantiate(m_WeaponPrefab, handTransform);

            animator.runtimeAnimatorController = m_AnimatorOverride;
        }
    }
}
