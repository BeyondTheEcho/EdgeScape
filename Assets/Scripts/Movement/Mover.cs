using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private Animator m_Animator;

        // Start is called before the first frame update
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
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

        public void MoveTo(Vector3 destination)
        {
            m_Agent.destination = destination;
        }
    }

}