using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="Dialogue", menuName="Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> m_Nodes = new();

        private Dictionary<string, DialogueNode> m_NodesDict = new();


        private void Awake()
        {
#if UNITY_EDITOR
            if(m_Nodes.Count == 0)
            {
                m_Nodes.Add(new DialogueNode());
            }
#endif

            //On Validate is not called in built game. Must be manually called.
            OnValidate();
        }

        private void OnValidate() 
        {
            m_NodesDict.Clear();

            foreach(DialogueNode node in m_Nodes)
            {
                m_NodesDict[node.m_UID] = node;
            }
        }

        public IEnumerable<DialogueNode> GetDialogueNodes() 
        {
            return m_Nodes;
        }

        public DialogueNode GetRootNode()
        {
            return m_Nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode node)
        {
            foreach (string childUID in node.m_Children)
            {
                if(m_NodesDict.ContainsKey(childUID))
                {
                    yield return m_NodesDict[childUID];
                }
            }
        }
    }
}