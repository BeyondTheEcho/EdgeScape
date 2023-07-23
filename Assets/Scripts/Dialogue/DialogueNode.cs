using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string m_Content;

        [SerializeField] private List<string> m_Children = new();

        [SerializeField] private Rect m_NodePosition = new(0, 0, 200, 100);

#if UNITY_EDITOR
        public void SetNodePosition(Vector2 newPos) 
        {
            Undo.RecordObject(this, "Moved Dialogue Node");
            m_NodePosition.position = newPos;
        }

        public void SetContent(string newContent)
        {
            if (newContent != m_Content) 
            {
                Undo.RecordObject(this, "Updated Dialogue Text");
                m_Content = newContent;
            }
        }

        public void AddChild(string child)
        {
            Undo.RecordObject(this, "Added Dialogue Link");
            m_Children.Add(child);
        }
        public void RemoveChild(string child)
        {
            Undo.RecordObject(this, "Removed Dialogue Link");
            m_Children.Remove(child);
        }
#endif

        public Rect GetNodeRect()
        {
            return m_NodePosition;
        }

        public string GetContent()
        {
            return m_Content;
        }

        public List<string> GetChildren()
        {
            return m_Children;
        }
    }
}