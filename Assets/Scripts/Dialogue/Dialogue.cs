using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="Dialogue", menuName="Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> m_Nodes = new();

#if UNITY_EDITOR
        private void Awake()
        {
            if(m_Nodes.Count == 0)
            {
                m_Nodes.Add(new DialogueNode());
            }
        }
#endif

        public IEnumerable<DialogueNode> GetDialogueNodes() 
        {
            return m_Nodes;
        }

        public DialogueNode GetRootNode()
        {
            return m_Nodes[0];
        }
    }
}