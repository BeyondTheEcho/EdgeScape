using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CursorMapping[] m_CursorMappings;

        private Mover m_Mover;
        private Fighter m_Fighter;
        private Health m_Health;

        enum CursorType
        {
            None,
            Movement,
            Combat,
            UI
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType m_Type;
            public Texture2D m_Texture;
            public Vector2 m_Hotspot;
        }

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
            if (InteractWithUI()) return;

            if (m_Health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            //Returns true if cursor is over UI gameobjects only
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
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

                    SetCursor(CursorType.Combat);
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

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.m_Texture, mapping.m_Hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) 
        {
            foreach (CursorMapping mapping in m_CursorMappings)
            {
                if (mapping.m_Type == type) return mapping;
            }

            return m_CursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}