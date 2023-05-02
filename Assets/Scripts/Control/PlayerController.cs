using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        private Mover m_Mover;
        private Fighter m_Fighter;
        private Health m_Health;

        // Start is called before the first frame update
        void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_Fighter = GetComponent<Fighter>();
            m_Health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Health.IsDead()) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            //Debug.Log("No Possible Actions");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            
            foreach (RaycastHit hit in hits) 
            {
                if (hit.transform.gameObject.TryGetComponent(out CombatTarget target))
                {
                    Fighter fighter = target.gameObject.GetComponent<Fighter>();

                    if (!fighter.CanAttack(target.gameObject)) continue;

                    if (Input.GetMouseButton(0))
                    {
                        m_Fighter.Attack(target.gameObject);                       
                    }

                    return true;
                }
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
            { 
                if (Input.GetMouseButton(0))
                {
                    m_Mover.StartMoveAction(hit.point, 1f);
                }

                return true;
            }

            return false;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}