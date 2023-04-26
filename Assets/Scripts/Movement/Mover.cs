using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;

namespace RPG.Movement
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private Animator m_Animator;
        private Fighter m_Fighter;

        // Start is called before the first frame update
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Fighter = GetComponent<Fighter>();
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = m_Agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;

            m_Animator.SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination)
        {
            m_Fighter.Cancel();
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            m_Agent.destination = destination;
            m_Agent.isStopped = false;
        }

        public void StopMovement()
        {
            m_Agent.isStopped = true;
        }
    }

}