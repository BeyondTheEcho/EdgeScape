using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private Modifier[] m_AdditiveModifiers;
        [SerializeField] private Modifier[] m_PercentageModifiers;

        //------------------------------------------------------------
        //                Custom Data Types
        //------------------------------------------------------------

        [System.Serializable]
        private struct Modifier
        {
            public Stat m_Stat;
            public float m_Value;
        }

        //------------------------------------------------------------
        //         IModifierProvider Implemented Functions
        //------------------------------------------------------------

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var modifier in m_AdditiveModifiers)
            {
                if (modifier.m_Stat == stat)
                {
                    yield return modifier.m_Value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in m_PercentageModifiers)
            {
                if (modifier.m_Stat == stat)
                {
                    yield return modifier.m_Value;
                }
            }
        }
    }
}