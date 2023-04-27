using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction m_CurrentAction;

        public void StartAction(IAction action)
        {
            if (m_CurrentAction == action) return;

            if (m_CurrentAction != null)
            {
                //Debug.Log(m_CurrentAction);
                m_CurrentAction.Cancel();
            }

            m_CurrentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}