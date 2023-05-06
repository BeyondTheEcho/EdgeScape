using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int m_SceneToLoad = -1;
        [SerializeField] public Transform m_SpawnPoint;
        [SerializeField] public PortalID m_PortalID;
        [SerializeField] private float m_FadeOutTime = 1.5f;
        [SerializeField] private float m_FadeInTime = 1.5f;
        [SerializeField] private float m_PauseBetweenFades = 1.5f;

        private SavingWrapper m_SavingWrapper;

        void Awake()
        {
            m_SavingWrapper = FindObjectOfType<SavingWrapper>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (m_SceneToLoad < 0)
            {
                Debug.LogError("Invalid SceneToLoad");
                yield break;
            }

            Fader fader = FindObjectOfType<Fader>();
            DontDestroyOnLoad(gameObject);

            yield return StartCoroutine(fader.FadeOut(m_FadeOutTime));
            m_SavingWrapper.Save();
            yield return SceneManager.LoadSceneAsync(m_SceneToLoad);
            m_SavingWrapper.Load();

            Portal destination = GetOtherPortal();
            UpdatePlayer(destination);

            m_SavingWrapper.Save();

            yield return new WaitForSeconds(m_PauseBetweenFades);
            yield return StartCoroutine(fader.FadeIn(m_FadeInTime));

            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;

                if (portal.m_PortalID == m_PortalID) return portal;
            }

            Debug.LogError("No Matching Portal ID Found");

            return null;
        }

        private void UpdatePlayer(Portal destination)
        {
            var player = GameObject.FindWithTag("Player").GetComponent<NavMeshAgent>();
            player.Warp(destination.m_SpawnPoint.position);
        }

        public enum PortalID
        {
            Alpha,
            Bravo,
            Charlie,
            Delta,
            Echo
        }
    }
}
