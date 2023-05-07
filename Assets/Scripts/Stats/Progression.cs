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

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> m_LookupTable = null;

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
            BuildLookup();

            //[level] can be added to directly return the target float
            float[] levels = m_LookupTable[charClass][stat];

            if (levels.Length < level) return 30;

            return levels[level - 1];
        }

        private void BuildLookup()
        {
            if (m_LookupTable != null) return;

            m_LookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (var progressionClass in m_CharacterClass)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (var stat in progressionClass.m_Stats)
                {
                    statLookupTable[stat.m_Stat] = stat.m_Levels;
                }

                m_LookupTable[progressionClass.m_CharacterClass] = statLookupTable;
            }
        }
    }
}
