using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [SerializeField] InventoryItem m_Item = null;
        [SerializeField] int m_Number = 1;

        //Constants
        private const int c_MaxNavMeshSpawnAttempts = 30;

        //------------------------------------------------------------
        //                Unity Monobehaviour Functions
        //------------------------------------------------------------

        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        //------------------------------------------------------------
        //                Public Functions
        //------------------------------------------------------------

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected() 
        { 
            return GetPickup() == null;
        }

        //------------------------------------------------------------
        //                Private Functions
        //------------------------------------------------------------

        private void SpawnPickup()
        {
            Pickup spawnedPickup;

            for (int i = 0; i < c_MaxNavMeshSpawnAttempts; i++)
            {
                Vector3 spawnPosition = transform.position;

                if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    spawnedPickup = m_Item.SpawnPickup(hit.position, m_Number);
                    spawnedPickup.transform.SetParent(transform);

                    return;
                }
            }

            spawnedPickup = m_Item.SpawnPickup(transform.position, m_Number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        //------------------------------------------------------------
        //            IJsonSaveable Implemented Functions
        //------------------------------------------------------------

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(isCollected());
        }

        public void RestoreFromJToken(JToken state)
        {
            bool shouldBeCollected = state.ToObject<bool>();

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }

        }
    }
}