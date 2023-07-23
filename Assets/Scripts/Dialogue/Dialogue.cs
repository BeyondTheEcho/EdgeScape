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
            foreach (string childUID in node.GetChildren())
            {
                if(m_NodesDict.ContainsKey(childUID))
                {
                    yield return m_NodesDict[childUID];
                }
            }
        }


#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Removed Dialogue Node");
            m_Nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in m_Nodes)
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }

        private void AddNode(DialogueNode newNode)
        {
            m_Nodes.Add(newNode);
            OnValidate();
        }

        private static DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
            }

            return newNode;
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (m_Nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
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
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}