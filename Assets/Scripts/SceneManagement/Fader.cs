using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;

        private void Start()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
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
