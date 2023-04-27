using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        private Mover m_Mover;
        private Fighter m_Fighter;

        // Start is called before the first frame update
        void Start()
        {
            m_Mover = GetComponent<Mover>();
            m_Fighter = GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            Debug.Log("No Possible Actions");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            
            foreach (RaycastHit hit in hits) 
            {
                if (hit.transform.gameObject.TryGetComponent(out CombatTarget target))
                {
                    if (!target.m_Health.CanAttack()) continue;

                    if (Input.GetMouseButtonDown(0))
                    {
                        m_Fighter.Attack(target);                       
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
                    m_Mover.StartMoveAction(hit.point);
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