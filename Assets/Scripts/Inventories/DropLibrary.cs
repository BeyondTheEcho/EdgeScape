using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] private DropConfig[] m_DropConfigs;
        [SerializeField] private float[] m_DropChance;
        [SerializeField] private int[] m_MinDrops;
        [SerializeField] private int[] m_MaxDrops;

        //------------------------------------------------------------
        //                Public Functions
        //------------------------------------------------------------

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        //------------------------------------------------------------
        //                Private Functions
        //------------------------------------------------------------

        private Dropped GetRandomDrop(int level)
        {
            var drop = GetRandomItem(level);
            var droppedItem = new Dropped();

            droppedItem.m_Item = drop.m_Item;
            droppedItem.m_Number = drop.GetRandomNumber(level);

            return droppedItem;
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0,100) < GetByLevel(m_DropChance, level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(m_MinDrops, level);
            int max = GetByLevel(m_MaxDrops, level);

            return Random.Range(min, max);
        }

        private DropConfig GetRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;

            foreach (var drop in m_DropConfigs)
            {
                chanceTotal += GetByLevel(drop.m_RelativeChance, level);

                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }

            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;

            foreach (var drop in m_DropConfigs)
            {
                total += GetByLevel(drop.m_RelativeChance, level);
            }

            return total;
        }

        //------------------------------------------------------------
        //                Custom Data Types
        //------------------------------------------------------------


        [System.Serializable]
        class DropConfig
        {
            public InventoryItem m_Item;
            public float[] m_RelativeChance;
            public int[] m_MinNumber;
            public int[] m_MaxNumber;

            public int GetRandomNumber(int level)
            {
                if (!m_Item.IsStackable()) return 1;

                int min = GetByLevel(m_MinNumber, level);
                int max = GetByLevel(m_MaxNumber, level);

                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem m_Item;
            public int m_Number;
        }

        //------------------------------------------------------------
        //                Static Methods
        //------------------------------------------------------------

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }

            if (level > values.Length)
            {
                return values[values.Length - 1];
            }

            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }
    }
}
