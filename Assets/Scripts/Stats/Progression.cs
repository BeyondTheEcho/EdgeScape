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
            [SerializeField] CharacterClass m_CharacterClass;
            [SerializeField] float[] m_Health;
        }
    }
}
