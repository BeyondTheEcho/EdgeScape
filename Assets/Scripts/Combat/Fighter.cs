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
        [SerializeField] private float m_WeaponRange = 0.1f;
        [SerializeField] private float m_TimeBetweenAttacks = 1f;
        [SerializeField] private float m_WeaponDamage = 5.0f;


        private float m_TimeSinceLastAttack = Mathf.Infinity;
        private Health m_Target;
        private Mover m_Mover;
        private ActionScheduler m_Scheduler;
        private Animator m_Animator;

        void Start ()
        {
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Animator = GetComponent<Animator>();
        }

        private void Update()
        {
            m_TimeSinceLastAttack += Time.deltaTime;

            if (m_Target == null) return;
            if (m_Target.IsDead()) return;

            if (!GetIsInRange())
            {
                m_Mover.MoveTo(m_Target.transform.position, 1f);
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

            m_Target.TakeDamage(m_WeaponDamage);
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, m_Target.transform.position) < m_WeaponRange;
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
    }
}
