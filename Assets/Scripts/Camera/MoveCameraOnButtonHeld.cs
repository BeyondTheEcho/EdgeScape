using Cinemachine;
using UnityEngine;

namespace UI_UX
{
    public class MoveCameraOnButtonHeld : MonoBehaviour
    {
        private float m_Sensitivity = 1.5f;
        //Min Radius
        private float m_TopMinRadius = 5.0f;
        private float m_MidMinRadius = 10.0f;
        private float m_BotMinRadius = 5.0f;
        //Max Radius
        private float m_TopMaxRadius = 20.0f;
        private float m_MidMaxRadius = 25.0f;
        private float m_BotMaxRadius = 20.0f;

        private CinemachineFreeLook m_Camera;

        enum CameraOrbits
        {
            Top,
            Mid,
            Bottom
        }

        void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
            m_Camera = GetComponent<CinemachineFreeLook>();
        }
        void Update()
        {
           UpdateCamera();
        }

        private void UpdateCamera()
        {
            var orbitScroll = Input.mouseScrollDelta.y * m_Sensitivity;

            for (int i = 0; i < m_Camera.m_Orbits.Length; i++)
            {
                m_Camera.m_Orbits[i].m_Radius += orbitScroll;
            }

            #region Clamps

            //Top Min Clamp
            if (m_Camera.m_Orbits[(int) CameraOrbits.Top].m_Radius < m_TopMinRadius) 
                m_Camera.m_Orbits[(int) CameraOrbits.Top].m_Radius = m_TopMinRadius;
            //Mid Min Clamp
            if (m_Camera.m_Orbits[(int)CameraOrbits.Mid].m_Radius < m_MidMinRadius) 
                m_Camera.m_Orbits[(int)CameraOrbits.Mid].m_Radius = m_MidMinRadius;
            //Bot Min Clamp
            if (m_Camera.m_Orbits[(int)CameraOrbits.Bottom].m_Radius < m_BotMinRadius) 
                m_Camera.m_Orbits[(int)CameraOrbits.Bottom].m_Radius = m_BotMinRadius;

            //Top Max Clamp
            if (m_Camera.m_Orbits[(int)CameraOrbits.Top].m_Radius > m_TopMaxRadius)
                m_Camera.m_Orbits[(int)CameraOrbits.Top].m_Radius = m_TopMaxRadius;
            //Mid Max Clamp
            if (m_Camera.m_Orbits[(int)CameraOrbits.Mid].m_Radius > m_MidMaxRadius)
                m_Camera.m_Orbits[(int)CameraOrbits.Mid].m_Radius = m_MidMaxRadius;
            //Bot Max Clamp
            if (m_Camera.m_Orbits[(int)CameraOrbits.Bottom].m_Radius > m_BotMaxRadius)
                m_Camera.m_Orbits[(int)CameraOrbits.Bottom].m_Radius = m_BotMaxRadius;

            #endregion
        }

        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
            {
                if (Input.GetMouseButton(2))
                {
                    return UnityEngine.Input.GetAxis("Mouse X");
                }
                else
                {
                    return 0;
                }
            }
            else if (axisName == "Mouse Y")
            {
                if (Input.GetMouseButton(2))
                {
                    return Input.GetAxis("Mouse Y");
                }
                else
                {
                    return 0;
                }
            }
            return Input.GetAxis(axisName);
        }
    }
}