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
        public string[] m_Children;
        public Rect m_NodePosition = new Rect(0, 0, 200, 100);
    }
}