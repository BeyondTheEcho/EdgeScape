using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string m_UID;
        public string m_Content;
        public List<string> m_Children = new();
        public Rect m_NodePosition = new Rect(0, 0, 200, 100);
    }
}