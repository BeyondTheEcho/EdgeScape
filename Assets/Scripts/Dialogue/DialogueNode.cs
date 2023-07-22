using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public string m_Content;
        public List<string> m_Children = new();
        public Rect m_NodePosition = new Rect(0, 0, 200, 100);
    }
}