using UnityEngine;

//RPG
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float m_ChaseDistance = 5.0f;
        [SerializeField] private float m_SuspisionTime = 5.0f;

        private GameObject m_Player;
        private ActionScheduler m_Scheduler;
        private Fighter m_Fighter;
        private Health m_Health;
        private Mover m_Mover;
        private Vector3 m_GuardPosition;
        private Vector3 m_LastKnownPlayerPosition;
        private float m_TimeSincePlayerLastSeen = Mathf.Infinity;

        private void Start()
        {
            m_GuardPosition = transform.position;
            m_Player = GameObject.FindWithTag("Player");
            m_Fighter = GetComponent<Fighter>();
            m_Health = GetComponent<Health>();
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            if (m_Health.IsDead()) return;

            if (InAttackRangeOfPlayer() && m_Fighter.CanAttack(m_Player))
            {
                AttackBehaviour();
            }
            else if (m_TimeSincePlayerLastSeen <= m_SuspisionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            m_TimeSincePlayerLastSeen += Time.deltaTime;
        }

        private void GuardBehaviour()
        {
            m_Mover.StartMoveAction(m_GuardPosition);
        }

        private void SuspicionBehaviour()
        {
            m_Scheduler.CancelCurrentAction();
            m_Mover.StartMoveAction(m_LastKnownPlayerPosition);
        }

        private void AttackBehaviour()
        {
            m_Fighter.Attack(m_Player);
            m_LastKnownPlayerPosition = m_Player.transform.position;
            m_TimeSincePlayerLastSeen = 0;
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer =  Vector3.Distance(transform.position, m_Player.transform.position);
            return distanceToPlayer < m_ChaseDistance;
        }

        //Called by Unity
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, m_ChaseDistance);
        }
    }
}
