using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private bool m_UseLookAtRotation = true;

        void LateUpdate()
        {
            if (m_UseLookAtRotation)
            {
                transform.LookAt(Camera.main.transform.position);
            }
            else
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}
