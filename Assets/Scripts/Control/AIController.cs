using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float m_ChaseDistance = 5.0f;

        private GameObject m_Player;
        private Fighter m_Fighter;

        private void Start()
        {
            m_Player = GameObject.FindWithTag("Player");
            m_Fighter = GetComponent<Fighter>();
        }

        private void Update()
        {
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
