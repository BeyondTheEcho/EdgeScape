using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float m_Health = 100f;

        public void TakeDamage(float damage)
        {
            m_Health = Mathf.Max(m_Health - damage, 0);

            Debug.Log($"Health: {m_Health}");
        }
    }
}
