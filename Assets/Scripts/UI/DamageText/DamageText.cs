using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        private TMP_Text m_Text;

        private void Awake()
        {
            m_Text = GetComponentInChildren<TMP_Text>();
        }

        public void SetDamage(float damage)
        {
            m_Text.text = damage.ToString();
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}