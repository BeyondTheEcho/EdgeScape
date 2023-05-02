using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediately()
        {
            m_CanvasGroup.alpha = 1.0f;
        }

        public IEnumerator FadeOut(float time)
        {
            while (m_CanvasGroup.alpha < 1)
            {
                m_CanvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (m_CanvasGroup.alpha > 0)
            {
                m_CanvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
