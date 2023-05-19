using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable
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
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(m_FadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(m_SceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;


            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(m_PauseBetweenFades);
            fader.FadeIn(m_FadeInTime);

            newPlayerController.enabled = true;
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

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.m_SpawnPoint.position;
            player.transform.rotation = otherPortal.m_SpawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        public enum PortalID
        {
            Alpha,
            Bravo,
            Charlie,
            Delta,
            Echo
        }

        public CursorType GetCursorType()
        {
            return CursorType.Portal;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerController.MoveToDestination(transform.position);
            }

            return true;
        }
    }
}
