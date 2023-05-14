using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {
        [SerializeField] private float m_TimeBetweenAttacks = 1f;
        [SerializeField] private Transform m_RightHand;
        [SerializeField] private Transform m_LeftHand;
        [SerializeField] private Weapon m_DefaultWeapon;

        private float m_TimeSinceLastAttack = Mathf.Infinity;
        private Health m_Target;
        private Mover m_Mover;
        private ActionScheduler m_Scheduler;
        private Animator m_Animator;
        private Weapon m_CurrentWeapon;
        private AudioSource m_AudioSource;
        private BaseStats m_BaseStats;
        private bool m_PlaySFX = true;
        private bool m_IsPlayer = false;

        void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Animator = GetComponent<Animator>();
            m_AudioSource = GetComponent<AudioSource>();
            m_BaseStats = GetComponent<BaseStats>();
        }

        void Start()
        {
            if (gameObject.tag == "Player") m_IsPlayer = true;

            if (m_CurrentWeapon == null)
            {
                EquipWeapon(m_DefaultWeapon);
            }
        }

        private void Update()
        {
            m_TimeSinceLastAttack += Time.deltaTime;

            if (m_Target == null) return;
            if (m_Target.IsDead()) return;

            if (!GetIsInRange())
            {
                var speed = 1.0f;

                if (gameObject.tag == "Enemy") speed = 0.75f;

                m_Mover.MoveTo(m_Target.transform.position, speed);
            }
            else
            {
                m_Mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(m_Target.transform);

            if (m_TimeSinceLastAttack < m_TimeBetweenAttacks) return;
            m_TimeSinceLastAttack = 0;

            //This will trigger the hit event
            TriggerAttack();
        }

        private void TriggerAttack()
        {
            if (m_PlaySFX && m_IsPlayer)
            {
                PlayAttackSFX();
            }

            m_PlaySFX = !m_PlaySFX;

            m_Animator.ResetTrigger("stopAttack");
            m_Animator.SetTrigger("attack");
        }

        private void PlayAttackSFX()
        {
            var sfx = m_CurrentWeapon.GetSFX();

            if (sfx == null) return;

            m_AudioSource.PlayOneShot(sfx);
        }

        //Animation Event - DON'T REMOVE
        private void Hit()
        {
            if (m_Target == null) return;

            float damage = m_BaseStats.GetStat(Stat.Damage);

            if (m_CurrentWeapon.HasProjectile())
            {
                m_CurrentWeapon.LaunchProjectile(m_RightHand, m_LeftHand, m_Target, gameObject, damage);
                return;
            }

            m_Target.TakeDamage(gameObject, damage);
        }

        //Animation Event - DON'T REMOVE
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, m_Target.transform.position) < m_CurrentWeapon.GetWeaponRange();
            return isInRange;
        }

        public void Attack(GameObject target)
        {
            m_Scheduler.StartAction(this);
            m_Target = target.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            StopAttack();
            m_Target = null;
        }

        private void StopAttack()
        {
            m_Animator.ResetTrigger("attack");
            m_Animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return m_CurrentWeapon.GetWeaponDamage();
            }
        }

        public Health GetTarget()
        { 
            return m_Target; 
        }

        public void EquipWeapon(Weapon weapon)
        {
            m_CurrentWeapon = weapon;
            m_CurrentWeapon.Spawn(m_RightHand, m_LeftHand, m_Animator);
        }

        public void EquipWeapon(string weaponName)
        {
            Weapon weapon = Resources.Load<Weapon>(weaponName);

            m_CurrentWeapon = weapon;
            m_CurrentWeapon.Spawn(m_RightHand, m_LeftHand, m_Animator);
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(m_CurrentWeapon.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            var weaponName = state.ToObject<string>();
            EquipWeapon(weaponName);
        }

    }
}
