using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        const string m_SaveFileName = "save";
        private SavingSystem m_SavingSystem;

        private void Start()
        {
            m_SavingSystem = GetComponent<SavingSystem>();
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