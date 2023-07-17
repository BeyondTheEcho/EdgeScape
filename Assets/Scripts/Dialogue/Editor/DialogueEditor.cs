using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue m_SelectedDialogue = null;
        private GUIStyle m_NodeStyle;
        private DialogueNode m_DraggingNode = null;
        private Vector2 m_DragOffset = Vector2.zero;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogue(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogue != null) 
            {
                ShowEditorWindow();
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;

            m_NodeStyle = new GUIStyle();
            m_NodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            m_NodeStyle.padding = new RectOffset(20, 20, 20, 20);
            m_NodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is Dialogue dialogue) 
            {
                m_SelectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (m_SelectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                HandleEvents();

                foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
                {
                    DrawNode(node);
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector3(node.m_NodePosition.xMax, node.m_NodePosition.center.y);

            foreach (DialogueNode childNode in m_SelectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition =  new Vector3(childNode.m_NodePosition.xMin, childNode.m_NodePosition.center.y);

                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;

                Handles.DrawBezier(startPosition, endPosition, 
                startPosition + controlPointOffset, endPosition - controlPointOffset, 
                Color.white, null, 3f);
            }
        }

        private void HandleEvents()
        {
            if (Event.current.type == EventType.MouseDown && m_DraggingNode == null)
            {
                m_DraggingNode = GetNodeAtPoint(Event.current.mousePosition);

                if (m_DraggingNode != null)
                {
                    m_DragOffset = m_DraggingNode.m_NodePosition.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && m_DraggingNode != null)
            {
                Undo.RecordObject(m_SelectedDialogue, "Move Dialogue Node");
                m_DraggingNode.m_NodePosition.position = Event.current.mousePosition + m_DragOffset;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && m_DraggingNode != null)
            {
                m_DraggingNode = null;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode dialogueNode = null;

            foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
            {
                if (node.m_NodePosition.Contains(mousePosition))
                {
                    dialogueNode = node;
                }
            }

            return dialogueNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.m_NodePosition, m_NodeStyle);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            string idText = EditorGUILayout.TextField(node.m_UID);
            string contentText = EditorGUILayout.TextField(node.m_Content);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_SelectedDialogue, "Update Dialogue Text");
                node.m_UID = idText;
                node.m_Content = contentText;
            }

            foreach (DialogueNode childNode in m_SelectedDialogue.GetAllChildren(node))
            {
                EditorGUILayout.LabelField(childNode.m_Content);
            }

            GUILayout.EndArea();
        }
    }
}