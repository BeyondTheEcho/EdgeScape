using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        //Config
        [Tooltip("How far drops an be scattered from the transform location in Unity units")]
        [SerializeField] private float m_DropScatterDistance = 1f;
        [SerializeField] private DropLibrary m_DropLibrary;
        [SerializeField] private int m_NumberOfDrops = 2;

        //Constants
        private const int c_MaxAttempts = 30;

        //------------------------------------------------------------
        //                   Public
        //------------------------------------------------------------

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();

            for (int i = 0; i < m_NumberOfDrops; i++)
            {
                var drops = m_DropLibrary.GetRandomDrops(baseStats.GetLevel());

                foreach (var drop in drops)
                {
                    DropItem(drop.m_Item, drop.m_Number);
                }
            }
        }

        //------------------------------------------------------------
        //                   Protected
        //------------------------------------------------------------

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < c_MaxAttempts; i++)
            {
                Vector3 randomPosition = transform.position;
                randomPosition += Random.insideUnitSphere * m_DropScatterDistance;

                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}