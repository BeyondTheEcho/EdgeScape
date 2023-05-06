using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] private float m_ExperiencePoints = 0;

        public void GainExperience(float experience)
        {
            m_ExperiencePoints += experience;
        }
    }

}