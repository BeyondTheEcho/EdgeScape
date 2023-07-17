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
                DialogueNode rootNode = new();
                rootNode.m_UID = Guid.NewGuid().ToString();

                m_Nodes.Add(rootNode);
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

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = new();
            newNode.m_UID = Guid.NewGuid().ToString();
            m_Nodes.Add(newNode);
            parentNode.m_Children.Add(newNode.m_UID);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            m_Nodes.Remove(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in m_Nodes)
            {
                node.m_Children.Remove(nodeToDelete.m_UID);
            }
        }
    }
}