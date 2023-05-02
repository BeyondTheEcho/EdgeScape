using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float m_FadeInTime = 3f;
        const string m_SaveFileName = "save";
        private SavingSystem m_SavingSystem;

        private void Awake()
        {
            m_SavingSystem = GetComponent<SavingSystem>();
        }

        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();

            fader.FadeOutImmediately();
            yield return m_SavingSystem.LoadLastScene(m_SaveFileName);
            yield return fader.FadeIn(m_FadeInTime);
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