using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float m_Speed = 3f;
        [SerializeField] private float m_ProjectileMaxLifetime = 10f;
        [SerializeField] private bool m_IsHoming = true;

        [Header("Hit Effect Settings")]
        [SerializeField] private GameObject m_HitEffect;

        [Header("Destroy Settings")] 
        [SerializeField] private GameObject[] m_DestroyOnHit;
        [SerializeField] private float m_LifetimeAfterImpact = 0f;

        [Header("Impale Settings")]
        [SerializeField] private bool m_DoesImpale = false;
        [SerializeField] private GameObject[] m_EffectsToDestroyOnImpale;
        [SerializeField] private float m_LifetimeAfterImpale = 5f;

        private Health m_Target;
        private CapsuleCollider m_CapsuleCollider = null;
        private float m_Damage = 0;
        private float m_ProjectileLifetime = 0f;
        private GameObject m_Instigator;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            LifetimeCountdown();

            if (m_Target == null) return;

            if (m_IsHoming && !m_Target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
        }

        private void LifetimeCountdown()
        {
            m_ProjectileLifetime += Time.deltaTime;

            if (m_ProjectileLifetime >= m_ProjectileMaxLifetime)
            {
                Destroy(gameObject);
            }
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

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            m_Target = target;
            m_Instigator = instigator;
            m_Damage = m_Damage = damage;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject != m_Target.gameObject) return;
            if (m_Target.IsDead()) return;

            m_Target.TakeDamage(m_Instigator, m_Damage);

            m_Speed = 0;

            if (m_HitEffect != null)
            {
                Instantiate(m_HitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in m_DestroyOnHit)
            {
                Destroy(toDestroy);
            }

            if (m_DoesImpale)
            {
                ImpaleBehaviour();
            }
            else
            {
                Destroy(gameObject, m_LifetimeAfterImpact);
            }
        }

        private void ImpaleBehaviour()
        {
            foreach (GameObject toDestroy in m_EffectsToDestroyOnImpale)
            {
                Destroy(toDestroy);
            }

            gameObject.transform.SetParent(m_Target.GetCenterMass(), true);

            Destroy(gameObject, m_LifetimeAfterImpale);
        }
    }
}
