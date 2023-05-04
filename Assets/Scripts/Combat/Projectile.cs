using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 3f;
        [SerializeField] private bool m_IsHoming = true;
        [SerializeField] private GameObject m_HitEffect;

        private Health m_Target;
        private CapsuleCollider m_CapsuleCollider = null;
        private float m_Damage = 0;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Target == null) return;

            if (m_IsHoming && !m_Target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

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

        public void SetTarget(Health target, float damage)
        {
            m_Target = target;
            m_Damage = m_Damage = damage;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject != m_Target.gameObject) return;
            if (m_Target.IsDead()) return;

            m_Target.TakeDamage(m_Damage);

            if (m_HitEffect != null)
            {
                Instantiate(m_HitEffect, GetAimLocation(), transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
