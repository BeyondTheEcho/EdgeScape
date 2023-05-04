using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;


namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float m_Health = 100f;
        [SerializeField] private Transform m_CenterMass;

        private bool m_IsDead = false;
        private Animator m_Animator;
        private ActionScheduler m_Scheduler;
        private BaseStats m_BaseStats;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_BaseStats = GetComponent<BaseStats>();
        }

        void Start()
        {
            m_Health = m_BaseStats.GetHealth();
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

        public Transform GetCenterMass()
        {
            return m_CenterMass;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(m_Health);
        }

        public void RestoreFromJToken(JToken state)
        {
            m_Health = state.ToObject<float>();

            if (m_Health <= 0)
            {
                Die();
            }
        }
    }
}
