using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;
using RPG.Inventories;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {
        //Config
        [Header("Condig")]
        [SerializeField] private float m_TimeBetweenAttacks = 1f;
        [SerializeField] private Transform m_RightHand;
        [SerializeField] private Transform m_LeftHand;
        [SerializeField] private WeaponConfig m_DefaultWeapon;

        //Cached Components
        private Health m_Target;
        private Mover m_Mover;
        private ActionScheduler m_Scheduler;
        private Animator m_Animator;
        private AudioSource m_AudioSource;
        private BaseStats m_BaseStats;
        private WeaponConfig m_CurrentWeaponConfig;
        private Equipment m_Equipment;
        private LazyValue<Weapon> m_CurrentWeapon;

        //Private Vars
        private float m_TimeSinceLastAttack = Mathf.Infinity;
        private bool m_PlaySFX = true;
        private bool m_IsPlayer = false;

        //------------------------------------------------------------
        //                Unity Monobehaviour Functions
        //------------------------------------------------------------

        void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Animator = GetComponent<Animator>();
            m_AudioSource = GetComponent<AudioSource>();
            m_BaseStats = GetComponent<BaseStats>();
            m_CurrentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            m_CurrentWeaponConfig = m_DefaultWeapon;
            m_Equipment = GetComponent<Equipment>();

            if (m_Equipment != null)
            {
                m_Equipment.a_EquipmentUpdated += UpdateWeapon;
            }
        }

        void Start()
        {
            if (gameObject.tag == "Player") m_IsPlayer = true;

            AttachWeapon(m_CurrentWeaponConfig);
        }

        private void Update()
        {
            m_TimeSinceLastAttack += Time.deltaTime;

            if (m_Target == null) return;
            if (m_Target.IsDead()) return;

            if (!GetIsInRange(m_Target.transform))
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

        //------------------------------------------------------------
        //                Public Functions
        //------------------------------------------------------------

        public void Attack(GameObject target)
        {
            m_Scheduler.StartAction(this);
            m_Target = target.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            if (!m_Mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            StopAttack();
            m_Target = null;
        }

        public Health GetTarget()
        {
            return m_Target;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            m_CurrentWeaponConfig = weapon;
            m_CurrentWeapon.value = AttachWeapon(weapon);
        }

        public void EquipWeapon(string weaponName)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);

            m_CurrentWeaponConfig = weapon;
            m_CurrentWeaponConfig.Spawn(m_RightHand, m_LeftHand, m_Animator);
        }

        //------------------------------------------------------------
        //                Private Functions
        //------------------------------------------------------------

        private void UpdateWeapon()
        {
            var weapon = m_Equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            
            if (weapon == null)
            {
                EquipWeapon(m_DefaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(m_DefaultWeapon);
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
            var sfx = m_CurrentWeaponConfig.GetSFX();

            if (sfx == null) return;

            m_AudioSource.PlayOneShot(sfx);
        }

        //Animation Event - DON'T REMOVE
        private void Hit()
        {
            if (m_Target == null) return;

            float damage = m_BaseStats.GetStat(Stat.Damage);

            if (m_CurrentWeapon.value != null)
            {
                m_CurrentWeapon.value.OnHit();
            }

            if (m_CurrentWeaponConfig.HasProjectile())
            {
                m_CurrentWeaponConfig.LaunchProjectile(m_RightHand, m_LeftHand, m_Target, gameObject, damage);
                return;
            }

            m_Target.TakeDamage(gameObject, damage);
        }

        //Animation Event - DON'T REMOVE
        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targeTransform)
        {
            bool isInRange = Vector3.Distance(transform.position, targeTransform.position) < m_CurrentWeaponConfig.GetWeaponRange();
            return isInRange;
        }

        private void StopAttack()
        {
            m_Animator.ResetTrigger("attack");
            m_Animator.SetTrigger("stopAttack");
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(m_RightHand, m_LeftHand, m_Animator);
        }

        //------------------------------------------------------------
        //            IJsonSaveable Implemented Functions
        //------------------------------------------------------------

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(m_CurrentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            var weaponName = state.ToObject<string>();
            EquipWeapon(weaponName);
        }
    }
}
