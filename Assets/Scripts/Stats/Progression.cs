using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            [SerializeField] public float[] m_Health;
        }

        public float GetHealth(CharacterClass charClass, int level)
        {
            foreach (var character in m_CharacterClass) 
            {
                if (character.m_CharacterClass == charClass)
                {
                    return character.m_Health[level - 1];
                }
            }

            return 30;
        }
    }
}
