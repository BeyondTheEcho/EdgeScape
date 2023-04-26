using RPG.Core;
using UnityEngine;
using RPG.Movement;


namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float m_WeaponRange = 2.0f;

        Transform m_Target;
        Mover m_Mover;
        ActionScheduler m_Scheduler;

        void Start ()
        {
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            if (m_Target == null) return;

            if (!GetIsInRange())
            {
                m_Mover.MoveTo(m_Target.position);
            }
            else
            {
                m_Mover.Cancel();
            }
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, m_Target.position) < m_WeaponRange;
            return isInRange;
        }

        public void Attack(CombatTarget target)
        {
            m_Scheduler.StartAction(this);
            m_Target = target.transform;
        }

        public void Cancel()
        {
            m_Target = null;
        }
    }
}
