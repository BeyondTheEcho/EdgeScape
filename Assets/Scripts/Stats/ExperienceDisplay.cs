using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text m_Text;
        Experience m_Experience;

        private void Awake()
        {
            m_Experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            m_Text.text = m_Experience.GetExperience().ToString();
        }
    }
}