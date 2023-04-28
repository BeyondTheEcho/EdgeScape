using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject m_PersistentObjectPrefab;

        static bool m_HasSpawned = false;

        void Awake()
        {
            if (m_HasSpawned) return;

            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            var persistentObject = Instantiate(m_PersistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
            m_HasSpawned = true;
        }
    }
}
