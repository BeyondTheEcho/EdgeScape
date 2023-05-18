using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1,99)] private int m_StartingLevel = 1;
        [SerializeField] private CharacterClass m_CharacterClass;
        [SerializeField] private Progression m_Progression;
        [SerializeField] private GameObject m_LevelUpParticleEffect;
        [SerializeField] private bool m_ShouldUseModifiers = false;

        private LazyValue<int> m_CurrentLevel;
        private Experience m_Experience;

        public event Action a_OnLevelUp;

        private void Awake()
        {
            m_Experience = GetComponent<Experience>();
            m_CurrentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable()
        {
            if (m_Experience != null)
            {
                m_Experience.a_OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (m_Experience != null)
            {
                m_Experience.a_OnExperienceGained -= UpdateLevel;
            }
        }

        private void Start()
        {
            m_CurrentLevel.ForceInit();
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > m_CurrentLevel.value)
            {
                m_CurrentLevel.value = newLevel;
                LevelUpEffect();
                a_OnLevelUp.Invoke();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(m_LevelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return m_Progression.GetStat(stat, m_CharacterClass, GetLevel());
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if (!m_ShouldUseModifiers) return 0f;

            float total = 0;

            foreach(IModifierProvider provider in GetComponents<IModifierProvider>()) 
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!m_ShouldUseModifiers) return 0f;

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }


        public int GetLevel()
        {
            return m_CurrentLevel.value;
        }

        public int CalculateLevel()
        {
            if (m_Experience == null) return m_StartingLevel;

            float currentXP = m_Experience.GetExperience();
            int MaxLevel = m_Progression.GetLevel(Stat.ExperienceToLevelUp, m_CharacterClass);

            for (int level = 1; level <= MaxLevel; level++)
            {
                float XPToLevelUp = m_Progression.GetStat(Stat.ExperienceToLevelUp, m_CharacterClass, level);

                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return MaxLevel + 1;
        }
    }
}