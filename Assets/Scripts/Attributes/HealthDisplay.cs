using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text m_Text;
        Health m_Health;

        private void Awake()
        {
            m_Health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            //Percent Health
            //m_Text.text = string.Format("{0:0}%", m_Health.GetHealthPercentage());

            //Value Health
            m_Text.text = $"{m_Health.GetHealthPoints():0}/{m_Health.GetMaxHealthPoints():0}";
        }
    }
}