using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text m_Text;
        BaseStats m_Stats;

        private void Awake()
        {
            m_Stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            m_Text.text = m_Stats.GetLevel().ToString();
        }
    }
}