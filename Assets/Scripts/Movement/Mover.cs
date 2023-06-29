using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AI;

//RPG
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using UnityEngine.UIElements;

namespace RPG.Movement
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        //Config
        [SerializeField] private float m_MaxSpeed = 6f;
        [SerializeField] private float m_MaxNavPathLength = 40f;

        private NavMeshAgent m_Agent;
        private Animator m_Animator;
        private ActionScheduler m_Scheduler;
        private Health m_Health;
        private JsonSaveableEntity m_Saveable;

        // Start is called before the first frame update
        void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Health = GetComponent<Health>();
            m_Saveable = GetComponent<JsonSaveableEntity>();
        }

        // Update is called once per frame
        private void Update()
        {
            m_Agent.enabled = !m_Health.IsDead();

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = m_Agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;

            m_Animator.SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            m_Scheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            //Path is being populated when passed into Calculate Path (Read shitty out variable)
            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path)) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > m_MaxNavPathLength) return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            m_Agent.destination = destination;
            m_Agent.speed = m_MaxSpeed * Mathf.Clamp01(speedFraction);
            m_Agent.isStopped = false;
        }

        public void Cancel()
        {
            m_Agent.isStopped = true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            Vector3 pos = state.ToVector3();

            //if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 5f, 1))
            //{
            //    m_Agent.Warp(hit.position);
            //}
            //else
            //{
            //    m_Agent.Warp(pos);
            //}

            m_Agent.enabled = false;
            transform.position = pos;
            m_Agent.enabled = true;

            m_Scheduler.CancelCurrentAction();
        }

    }
}