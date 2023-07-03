using UnityEngine;

//RPG
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        //Config
        [SerializeField] private float m_MaxNavMeshProjectionDistance = 1f;
        [SerializeField] private float m_InteractSphereCastRadius = 1f;

        //Array of Cursor Mappings
        [SerializeField] private CursorMapping[] m_CursorMappings;

        //Component Refs
        private Mover m_Mover;
        private Fighter m_Fighter;
        private Health m_Health;

        //Private Vars
        private bool m_IsDraggingUI = false; //Must be initialized as false

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
            RaycastHit[] hits = RaycastAllSorted();

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
            if (Input.GetMouseButtonUp(0)) 
            {
                m_IsDraggingUI = false;
            }

            //Returns true if cursor is over UI gameobjects only
            if(EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_IsDraggingUI = true;
                }

                SetCursor(CursorType.UI);
                return true;
            }

            if (m_IsDraggingUI) return true;

            return false;
        }

        private bool InteractWithMovement()
        {
            if (RaycastNavMesh(out Vector3 target))
            {
                if (!m_Mover.CanMoveTo(target)) return false;

                if (Input.GetMouseButton(c_LeftMouseButton))
                {
                    m_Mover.StartMoveAction(target, c_MaxSpeed);
                }

                SetCursor(CursorType.Movement);
                return true;
            }
            
            return false;
        }

        private bool RaycastNavMesh(out Vector3 position)
        {
            position = new Vector3();

            if (!Physics.Raycast(GetMouseRay(), out RaycastHit hit)) return false;

            if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, m_MaxNavMeshProjectionDistance, NavMesh.AllAreas)) return false;

            position = navMeshHit.position;

            //Path is being populated when passed into Calculate Path (Read shitty out variable)
            //NavMeshPath path = new NavMeshPath();
            //if (!NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path)) return false;
            //if (path.status != NavMeshPathStatus.PathComplete) return false;
            //if (GetPathLength(path) > m_MaxNavPathLength) return false;

            return true;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), m_InteractSphereCastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        public void MoveToDestination(Vector3 position)
        {
            m_Mover.StartMoveAction(position, c_MaxSpeed);
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            UnityEngine.Cursor.SetCursor(mapping.m_Texture, mapping.m_Hotspot, CursorMode.Auto);
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