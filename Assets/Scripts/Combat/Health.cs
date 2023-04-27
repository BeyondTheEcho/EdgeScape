using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float m_Health = 100f;

        private bool m_IsDead = false;
        private Animator m_Animator;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            m_Health = Mathf.Max(m_Health - damage, 0);

            if (m_Health == 0 && !m_IsDead)
            {
                Death();
            }

            Debug.Log($"[{gameObject.name}] - Health: {m_Health}");
        }

        private void Death()
        {
            m_IsDead = true;
            m_Animator.SetTrigger("death");
        }

        public bool IsDead()
        {
            return m_IsDead;
        }
    }
}
