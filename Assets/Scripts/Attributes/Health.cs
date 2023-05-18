using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private Transform m_CenterMass;
        [SerializeField] private UnityEvent<float> a_TakeDamage;

        private LazyValue<float> m_Health;
        private bool m_IsDead = false;
        private Animator m_Animator;
        private ActionScheduler m_Scheduler;
        private BaseStats m_BaseStats;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_BaseStats = GetComponent<BaseStats>();
            m_Health = new LazyValue<float>(GetInitialHealth);
        }

        void Start()
        {
            m_Health.ForceInit();
            m_BaseStats.a_OnLevelUp += RestoreHealth;
        }

        private float GetInitialHealth()
        {
            return m_BaseStats.GetStat(Stat.Health);
        }

        private void RestoreHealth()
        {
            m_Health.value = m_BaseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject attacker, float damage)
        {
            m_Health.value = Mathf.Max(m_Health.value - damage, 0);

            a_TakeDamage.Invoke(damage);

            if (m_Health.value == 0)
            {
                Die();
                AwardExperience(attacker);
            }

            Debug.Log($"[{gameObject.name}] - Health: {m_Health.value} - Took {damage} damage.");
        }

        private void AwardExperience(GameObject attacker)
        {
            if(attacker.TryGetComponent(out Experience experience))
            {
                experience.GainExperience(m_BaseStats.GetStat(Stat.ExperienceReward));
            }
        }

        public float GetFraction()
        {
            return m_Health.value / m_BaseStats.GetStat(Stat.Health);
        }

        public float GetHealthPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetHealthPoints()
        {
            return m_Health.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
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
            return JToken.FromObject(m_Health.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            m_Health.value = state.ToObject<float>();

            if (m_Health.value <= 0)
            {
                Die();
            }
        }
    }
}
