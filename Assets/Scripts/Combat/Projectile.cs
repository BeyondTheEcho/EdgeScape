using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 3f;

        private Health m_Target;
        private CapsuleCollider m_CapsuleCollider = null;


        // Update is called once per frame
        void Update()
        {
            if (m_Target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
        }

        private Vector3 GetAimLocation()
        {
            if (m_CapsuleCollider == null)
            {
                m_CapsuleCollider = m_Target.GetComponent<CapsuleCollider>();
            }

            if (m_CapsuleCollider == null)
            {
                return m_Target.transform.position;
            }

            return m_Target.transform.position + Vector3.up * m_CapsuleCollider.height / 2;
        }

        public void SetTarget(Health target)
        {
            m_Target = target;
        }
    }
}
