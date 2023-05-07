using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;


namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private Transform m_CenterMass;

        private float m_Health = -1f;
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
            //Prevents a race condition with the restoration from the save system
            if (m_Health < 0)
            {
                m_Health = m_BaseStats.GetStat(Stat.Health);
            }

            m_BaseStats.a_OnLevelUp += RestoreHealth;
        }

        private void RestoreHealth()
        {
            m_Health = m_BaseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject attacker, float damage)
        {
            m_Health = Mathf.Max(m_Health - damage, 0);

            if (m_Health == 0)
            {
                Die();
                AwardExperience(attacker);
            }

            Debug.Log($"[{gameObject.name}] - Health: {m_Health}");
        }

        private void AwardExperience(GameObject attacker)
        {
            if(attacker.TryGetComponent(out Experience experience))
            {
                experience.GainExperience(m_BaseStats.GetStat(Stat.ExperienceReward));
            }
        }

        public float GetHealthPercentage()
        {
            return 100 * (m_Health / m_BaseStats.GetStat(Stat.Health));
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
