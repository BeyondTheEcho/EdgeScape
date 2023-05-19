using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float m_FadeInTime = 3f;
        const string m_SaveFileName = "save";
        private JsonSavingSystem m_SavingSystem;

        private void Awake()
        {
            m_SavingSystem = GetComponent<JsonSavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return m_SavingSystem.LoadLastScene(m_SaveFileName);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(m_FadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S)) Save();
            if (Input.GetKeyDown(KeyCode.L)) Load();
            if (Input.GetKeyDown(KeyCode.R)) Delete();
        }

        public void Load()
        {
            m_SavingSystem.Load(m_SaveFileName);
        }

        public void Save()
        {
            m_SavingSystem.Save(m_SaveFileName);
        }

        public void Delete()
        {
            m_SavingSystem.Delete(m_SaveFileName);
        }
    }
}