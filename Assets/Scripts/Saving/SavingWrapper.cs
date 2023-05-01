using System.Collections;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        const string m_SaveFileName = "save";
        private SavingSystem m_SavingSystem;

        private void Awake()
        {
            m_SavingSystem = GetComponent<SavingSystem>();
        }

        private IEnumerator Start()
        {
            yield return m_SavingSystem.LoadLastScene(m_SaveFileName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            m_SavingSystem.Load(m_SaveFileName);
        }

        public void Save()
        {
            m_SavingSystem.Save(m_SaveFileName);
        }
    }
}