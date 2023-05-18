using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private RectTransform m_HealthBar;
        [SerializeField] private Health m_Health;
        
        void Update()
        {
            m_HealthBar.localScale = new Vector3(m_Health.GetFraction(), 1, 1);
        }
    }
}