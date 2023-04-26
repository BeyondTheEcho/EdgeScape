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
            InteractWithCombat();
            InteractWithMovement();
        }

        private void InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            
            foreach (RaycastHit hit in hits) 
            {
                if (hit.transform.gameObject.TryGetComponent(out CombatTarget target))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_Fighter.Attack(target);
                    }
                }
            }
        }

        private void InteractWithMovement() 
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }
        }

        private void MoveToCursor()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
            {
                m_Mover.MoveTo(hit.point);
            }
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}