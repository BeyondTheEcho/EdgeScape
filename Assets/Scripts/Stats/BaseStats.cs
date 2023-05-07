using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEditorInternal;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField][Range(1,99)] private int m_StartingLevel = 1;
        [SerializeField] private CharacterClass m_CharacterClass;
        [SerializeField] private Progression m_Progression;

        private Experience m_Experience;

        private void Awake()
        {
            m_Experience = GetComponent<Experience>();
        }

        private void Update()
        {
            if (gameObject.tag == "Player")
            {
                Debug.Log($"Player Level: {GetLevel()}");
            }
        }

        public float GetStat(Stat stat)
        {
            return m_Progression.GetStat(stat, m_CharacterClass, GetLevel());
        }

        public int GetLevel()
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