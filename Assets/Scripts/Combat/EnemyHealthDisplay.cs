using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text m_Text;
        Fighter m_PlayerFighter;

        private void Awake()
        {
            m_PlayerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Health enemyHealth = m_PlayerFighter.GetTarget();

            if (enemyHealth != null) 
            {
                float enemyHealthValue = enemyHealth.GetHealthPercentage();
                m_Text.text = string.Format("{0:0}%", enemyHealthValue);
            }
            else
            {
                m_Text.text = "No Target";
            }

        }
    }
}
