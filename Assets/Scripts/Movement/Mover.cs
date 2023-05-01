using UnityEngine;
using UnityEngine.AI;

//RPG
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float m_MaxSpeed = 6f;

        private NavMeshAgent m_Agent;
        private Animator m_Animator;
        private ActionScheduler m_Scheduler;
        private Health m_Health;
        private SaveableEntity m_Saveable;

        // Start is called before the first frame update
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Scheduler = GetComponent<ActionScheduler>();
            m_Health = GetComponent<Health>();
            m_Saveable = GetComponent<SaveableEntity>();
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

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            var pos = (SerializableVector3)state;

            if (m_Agent.Warp(pos.ToVector()))
            {
                m_Scheduler.CancelCurrentAction();
                print("Restored state for: " + m_Saveable.GetUniqueIdentifier());
            }
        }
    }

}