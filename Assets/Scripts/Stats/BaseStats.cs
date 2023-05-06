using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField][Range(1,99)] private int m_StartingLevel = 1;
        [SerializeField] private CharacterClass m_CharacterClass;
        [SerializeField] private Progression m_Progression;

        public float GetStat(Stat stat)
        {
            return m_Progression.GetStat(stat, m_CharacterClass, m_StartingLevel);
        }
    }
}