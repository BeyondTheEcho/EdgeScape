using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float m_ExperiencePoints = 0;

        public void GainExperience(float experience)
        {
            m_ExperiencePoints += experience;
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