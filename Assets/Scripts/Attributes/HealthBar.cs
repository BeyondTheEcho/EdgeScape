using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private RectTransform m_HealthBar;
        [SerializeField] private Health m_Health;
        [SerializeField] private Canvas m_RootCanvas;
        
        void Update()
        {
            float healthFraction = m_Health.GetFraction();

            if (Mathf.Approximately(healthFraction, 0f) || Mathf.Approximately(healthFraction, 1f))
            {
                m_RootCanvas.enabled = false;
                return;
            }

            m_RootCanvas.enabled = true;
            m_HealthBar.localScale = new Vector3(healthFraction, 1, 1);
        }
    }
}