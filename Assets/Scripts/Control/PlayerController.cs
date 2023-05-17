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
        //Array of Cursor Mappings
        [SerializeField] private CursorMapping[] m_CursorMappings;

        //Component Refs
        private Mover m_Mover;
        private Fighter m_Fighter;
        private Health m_Health;

        //Constants
        private const float c_MaxSpeed = 1f; //Val MUST be 1f
        private const int c_LeftMouseButton = 0;


        //Cursor Mapping Struct
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

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastable = hit.transform.GetComponents<IRaycastable>();

                foreach (IRaycastable castable in raycastable)
                {
                    if (castable.HandleRaycast(this))
                    {
                        SetCursor(castable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
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

        private bool InteractWithMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
            { 
                if (Input.GetMouseButton(c_LeftMouseButton))
                {
                    m_Mover.StartMoveAction(hit.point, c_MaxSpeed);
                }

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        public void MoveToDestination(Vector3 position)
        {
            m_Mover.StartMoveAction(position, c_MaxSpeed);
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