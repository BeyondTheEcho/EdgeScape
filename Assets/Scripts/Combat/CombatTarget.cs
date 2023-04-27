using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
        public Health m_Health;

        void Start()
        {
            m_Health = GetComponent<Health>();
        }
    }

}