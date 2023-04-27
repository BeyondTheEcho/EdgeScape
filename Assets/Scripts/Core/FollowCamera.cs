using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform m_Target;

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = m_Target.position;
        }
    }
}