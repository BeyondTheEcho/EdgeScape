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
        [NonSerialized] private GUIStyle m_NodeStyle;
        [NonSerialized] private DialogueNode m_DraggingNode = null;
        [NonSerialized] private Vector2 m_DragOffset = Vector2.zero;
        [NonSerialized] private DialogueNode m_CreatingNode = null;
        
        [NonSerialized] private DialogueNode m_DeletingNode = null;
        [NonSerialized] private DialogueNode m_LinkingParentNode = null;

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

                if (m_CreatingNode != null)
                {
                    Undo.RecordObject(m_SelectedDialogue, "Added Dialogue Node");
                    m_SelectedDialogue.CreateNode(m_CreatingNode);
                    m_CreatingNode = null;
                }

                if (m_DeletingNode != null)
                {
                     Undo.RecordObject(m_SelectedDialogue, "Removed Dialogue Node");
                    m_SelectedDialogue.DeleteNode(m_DeletingNode);
                    m_DeletingNode = null;
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

            string contentText = EditorGUILayout.TextField(node.m_Content);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_SelectedDialogue, "Update Dialogue Text");
                node.m_Content = contentText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
            {
                m_CreatingNode = node;
            }

            if (GUILayout.Button("-"))
            {
                m_DeletingNode = node;
            }

            GUILayout.EndHorizontal();

            if (m_LinkingParentNode == null)
            {
                if (GUILayout.Button("Edit Links"))
                {
                    m_LinkingParentNode = node;
                }      
            }
            else
            {
                if (m_LinkingParentNode == node)
                {
                    if (GUILayout.Button("Exit Edit Mode"))
                    {
                        m_LinkingParentNode = null;
                    }  
                }
                else
                {
                    if (m_LinkingParentNode.m_Children.Contains(node.m_UID))
                    {
                        if (GUILayout.Button("Remove Link"))
                        {
                            Undo.RecordObject(m_SelectedDialogue, "Removed Dialogue Link");
                            m_LinkingParentNode.m_Children.Remove(node.m_UID);
                        }          
                    }
                    else
                    {
                        if (GUILayout.Button("Add Link"))
                        {
                            Undo.RecordObject(m_SelectedDialogue, "Added Dialogue Link");
                            m_LinkingParentNode.m_Children.Add(node.m_UID);
                        }          
                    }
     
                }
            }

            GUILayout.EndArea();
        }
    }
}