using UnityEngine;
using RPG.Movement;


namespace RPG.Combat
{
    [RequireComponent(typeof(Mover))]
    public class Fighter : MonoBehaviour
    {
        [SerializeField] private float m_WeaponRange = 2.0f;

        Transform m_Target;
        Mover m_Mover;

        void Start ()
        {
            m_Mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (m_Target != null)
            {
                bool isInRange = Vector3.Distance(transform.position, m_Target.position) < m_WeaponRange;

                if (!isInRange)
                {
                    m_Mover.MoveTo(m_Target.position);
                }
                else
                {
                    m_Mover.StopMovement();
                }
            }
        }

        public void Attack(CombatTarget target)
        {
            m_Target = target.transform;
        }

        public void Cancel()
        {
            m_Target = null;
        }
    }
}
