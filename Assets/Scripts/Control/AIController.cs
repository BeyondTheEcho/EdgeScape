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
        [SerializeField] private PatrolPath m_PatrolPath = null;
        [SerializeField] private float m_WaypointTolerance = 1.0f;
        [SerializeField] private float m_DwellTime = 5.0f;
        [SerializeField] [Range(0,1)] private float m_PatrolSpeedFraction = 0.2f;


        private GameObject m_Player;
        private ActionScheduler m_Scheduler;
        private Fighter m_Fighter;
        private Health m_Health;
        private Mover m_Mover;
        private Vector3 m_GuardPosition;
        private Vector3 m_LastKnownPlayerPosition;
        private float m_TimeSincePlayerLastSeen = Mathf.Infinity;
        private float m_TimeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int m_CurrentWaypoint = 0;

        private void Awake()
        {
            m_Fighter = GetComponent<Fighter>();
            m_Health = GetComponent<Health>();
            m_Mover = GetComponent<Mover>();
            m_Scheduler = GetComponent<ActionScheduler>();
        }

        private void Start()
        {
            m_GuardPosition = transform.position;
            m_Player = GameObject.FindWithTag("Player");
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
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            m_TimeSincePlayerLastSeen += Time.deltaTime;
            m_TimeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = m_GuardPosition;

            if (m_PatrolPath != null)
            {
                if (AtWaypoint())
                {
                    m_TimeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (m_TimeSinceArrivedAtWaypoint > m_DwellTime)
            {
                m_Mover.StartMoveAction(nextPosition, m_PatrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return m_PatrolPath.GetWaypoint(m_CurrentWaypoint);
        }

        private void CycleWaypoint()
        {
            m_CurrentWaypoint = m_PatrolPath.GetNextIndex(m_CurrentWaypoint);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < m_WaypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            m_Scheduler.CancelCurrentAction();
            m_Mover.StartMoveAction(m_LastKnownPlayerPosition, 1f);
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
