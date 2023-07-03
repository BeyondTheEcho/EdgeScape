using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        //Config
        [Tooltip("How far drops an be scattered from the transform location in Unity units")]
        [SerializeField] private float m_DropScatterDistance = 1f;

        //Constants
        private const int c_MaxAttempts = 30;

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