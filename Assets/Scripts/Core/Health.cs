using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float m_Health = 100f;

        private bool m_IsDead = false;
        private Animator m_Animator;
        private ActionScheduler m_Scheduler;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Scheduler = GetComponent<ActionScheduler>();
        }

        public void TakeDamage(float damage)
        {
            m_Health = Mathf.Max(m_Health - damage, 0);

            if (m_Health == 0)
            {
                Die();
            }

            Debug.Log($"[{gameObject.name}] - Health: {m_Health}");
        }

        private void Die()
        {
            if (m_IsDead) return;

            m_IsDead = true;
            m_Animator.SetTrigger("death");
            m_Scheduler.CancelCurrentAction();
        }

        public bool IsDead()
        {
            return m_IsDead;
        }
    }
}
