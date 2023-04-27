using UnityEngine;

//RPG
using RPG.Core;

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