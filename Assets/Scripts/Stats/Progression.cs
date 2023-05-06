using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "EdgeScape/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] m_CharacterClass;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] public CharacterClass m_CharacterClass;
            [SerializeField] public ProgressionStat[] m_Stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            [SerializeField] public Stat m_Stat;
            [SerializeField] public float[] m_Levels;
        }

        public float GetStat(Stat stat, CharacterClass charClass, int level)
        {
            foreach (var progressionClass in m_CharacterClass) 
            {
                if (progressionClass.m_CharacterClass != charClass) continue;

                foreach (var progressionStat in progressionClass.m_Stats)
                {
                    if (progressionStat.m_Stat != stat) continue;

                    if(progressionStat.m_Levels.Length < level) continue;

                    return progressionStat.m_Levels[level - 1];
                }
            }

            return 30;
        }
    }
}
