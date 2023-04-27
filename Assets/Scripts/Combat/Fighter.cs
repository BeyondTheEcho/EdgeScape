using RPG.Core;
using UnityEngine;
using RPG.Movement;


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


        private float m_TimeSinceLastAttack;
        private CombatTarget m_Target;
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
            if (m_Target.m_Health.IsDead()) return;

            if (!GetIsInRange())
            {
                m_Mover.MoveTo(m_Target.transform.position);
            }
            else
            {
                m_Mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            
            if (m_TimeSinceLastAttack < m_TimeBetweenAttacks) return;
            m_TimeSinceLastAttack = 0;

            //This will trigger the hit event
            m_Animator.SetTrigger("attack");
        }

        //Animation Event - DON'T REMOVE
        private void Hit()
        {
            m_Target.m_Health.TakeDamage(m_WeaponDamage);
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, m_Target.transform.position) < m_WeaponRange;
            return isInRange;
        }

        public void Attack(CombatTarget target)
        {
            m_Scheduler.StartAction(this);
            m_Target = target;
        }

        public void Cancel()
        {
            m_Animator.SetTrigger("stopAttack");
            m_Target = null;
        }

    }
}
