using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="Dialogue", menuName="Dialogue/New Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> m_Nodes = new();

        private Dictionary<string, DialogueNode> m_NodesDict = new();


        private void Awake()
        {
            //On Validate is not called in built game. Must be manually called.
            OnValidate();
        }

        private void OnValidate() 
        {
            m_NodesDict.Clear();

            foreach(DialogueNode node in m_Nodes)
            {
                m_NodesDict[node.name] = node;
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
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");

            m_Nodes.Add(newNode);

            if (parentNode != null) 
            {
                parentNode.m_Children.Add(newNode.name);
            }

            //AssetDatabase.AddObjectToAsset(newNode, this);

            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            m_Nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in m_Nodes)
            {
                node.m_Children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if (m_Nodes.Count == 0)
            {
                CreateNode(null);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in m_Nodes)
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
        }
    }
}