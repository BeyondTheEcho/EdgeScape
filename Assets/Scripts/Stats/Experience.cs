using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float m_ExperiencePoints = 0;

        public event Action a_OnExperienceGained;

        public void GainExperience(float experience)
        {
            m_ExperiencePoints += experience;
            a_OnExperienceGained.Invoke();
        }

        public float GetExperience()
        {
            return m_ExperiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(m_ExperiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            m_ExperiencePoints = state.ToObject<float>();
        }
    }

}