using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup m_CanvasGroup;
        Coroutine m_CurrentActiveFade = null;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            m_CanvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            return Fade(1, time);
        }

        public IEnumerator FadeIn(float time)
        {
            return Fade(0, time);
        }

        public IEnumerator Fade(float target, float time)
        {
            if (m_CurrentActiveFade != null)
            {
                StopCoroutine(m_CurrentActiveFade);
            }
            m_CurrentActiveFade = StartCoroutine(FadeRoutine(target, time));
            yield return m_CurrentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(m_CanvasGroup.alpha, target))
            {
                m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
