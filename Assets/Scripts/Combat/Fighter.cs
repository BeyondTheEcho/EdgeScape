using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float m_TimeBetweenAttacks = 1f;
        [SerializeField] private Transform m_HandPosition;
        [SerializeField] private Weapon m_Weapon;

        private float m_TimeSinceLastAttack = Mathf.Infinity;
        private Health m_Target;
        private Mover m_Mover;
        private ActionScheduler m_Scheduler;
        private Animator m_Animator;

        void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (gameObject.tag == "Player")
            {
                SpawnWeapon();
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
            m_Animator.ResetTrigger("stopAttack");
            m_Animator.SetTrigger("attack");
        }

        //Animation Event - DON'T REMOVE
        private void Hit()
        {
            if (m_Target == null) return;

            m_Target.TakeDamage(m_Weapon.GetWeaponDamage());
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, m_Target.transform.position) < m_Weapon.GetWeaponRange();
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

        private void SpawnWeapon()
        {
            if (m_Weapon == null) return;

            m_Weapon.Spawn(m_HandPosition, m_Animator);
        }
    }
}
