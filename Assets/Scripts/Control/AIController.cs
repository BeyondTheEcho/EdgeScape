using UnityEngine;

//RPG
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float m_ChaseDistance = 5.0f;

        private GameObject m_Player;
        private Fighter m_Fighter;
        private Health m_Health;

        private void Start()
        {
            m_Player = GameObject.FindWithTag("Player");
            m_Fighter = GetComponent<Fighter>();
            m_Health = GetComponent<Health>();
        }

        private void Update()
        {
            if (m_Health.IsDead()) return;

            AttackBehaviour();
        }

        private void AttackBehaviour()
        {
            if (InAttackRangeOfPlayer() && m_Fighter.CanAttack(m_Player))
            {
                m_Fighter.Attack(m_Player);
            }
            else
            {
                m_Fighter.Cancel();
            }
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer =  Vector3.Distance(transform.position, m_Player.transform.position);
            return distanceToPlayer < m_ChaseDistance;
        }
    }
}
