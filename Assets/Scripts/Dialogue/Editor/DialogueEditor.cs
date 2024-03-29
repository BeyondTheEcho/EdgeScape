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
        //Non Serialized Vars
        [NonSerialized] private GUIStyle m_NodeStyle;
        [NonSerialized] private DialogueNode m_DraggingNode = null;
        [NonSerialized] private Vector2 m_DragOffset = Vector2.zero;
        [NonSerialized] private DialogueNode m_CreatingNode = null;
        [NonSerialized] private DialogueNode m_DeletingNode = null;
        [NonSerialized] private DialogueNode m_LinkingParentNode = null;
        [NonSerialized] private bool m_DraggingCanvas = false;
        [NonSerialized] private Vector2 m_DraggingCanvasOffset;

        //Private Vars
        private Vector2 m_ScrollPosition = Vector2.zero;
        private Dialogue m_SelectedDialogue = null;

        //Constants
        const float c_CanvasSize = 4000f;
        const float c_BackgroundSize = 50f;

        //------------------------------------------------------------
        //            Unity GUI / Event Functions
        //------------------------------------------------------------

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
                return true;
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

                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(c_CanvasSize, c_CanvasSize);

                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;

                Rect textureCoords = new Rect(0, 0, canvas.width / backgroundTexture.width, canvas.height / backgroundTexture.height);

                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);

                foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (m_CreatingNode != null)
                {
                    m_SelectedDialogue.CreateNode(m_CreatingNode);
                    m_CreatingNode = null;
                }

                if (m_DeletingNode != null)
                {
                    m_SelectedDialogue.DeleteNode(m_DeletingNode);
                    m_DeletingNode = null;
                }
            }
        }

        //------------------------------------------------------------
        //               Handle Events Switch Functions
        //------------------------------------------------------------
        
        private void ResetDraggingCanvas()
        {
            m_DraggingCanvas = false;
        }
        
        private void ResetDraggingNode()
        {
            m_DraggingNode = null;
        }
        
        private void ClickDragScrollCanvas()
        {
            m_ScrollPosition = m_DraggingCanvasOffset - Event.current.mousePosition;
            Repaint();
        }
        
        private void MoveDraggingNode()
        {
            m_DraggingNode.SetNodePosition(Event.current.mousePosition + m_DragOffset);
            Repaint();
        }
        
        private void CheckSetDragState()
        {
            m_DraggingNode = GetNodeAtPoint(Event.current.mousePosition + m_ScrollPosition);
            if (m_DraggingNode != null)
            {
                m_DragOffset = m_DraggingNode.GetNodeRect().position - Event.current.mousePosition;
                Selection.activeObject = m_DraggingNode;
            }
            else
            {
                m_DraggingCanvas = true;
                m_DraggingCanvasOffset = Event.current.mousePosition + m_ScrollPosition;
                Selection.activeObject = m_SelectedDialogue;
            }
        }

        //------------------------------------------------------------
        //               Private Functions
        //------------------------------------------------------------

        private void HandleEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when m_DraggingNode == null:
                    CheckSetDragState();
                    break;
                case EventType.MouseDrag when m_DraggingNode != null:
                    MoveDraggingNode();
                    break;
                case EventType.MouseDrag when m_DraggingCanvas:
                    ClickDragScrollCanvas();
                    break;
                case EventType.MouseUp when m_DraggingNode != null:
                    ResetDraggingNode();
                    break;
                case EventType.MouseUp when m_DraggingCanvas:
                    ResetDraggingCanvas();
                    break;
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector3(node.GetNodeRect().xMax, node.GetNodeRect().center.y);

            foreach (DialogueNode childNode in m_SelectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector3(childNode.GetNodeRect().xMin, childNode.GetNodeRect().center.y);

                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;

                Handles.DrawBezier(startPosition, endPosition,
                startPosition + controlPointOffset, endPosition - controlPointOffset,
                Color.white, null, 3f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode dialogueNode = null;

            foreach (DialogueNode node in m_SelectedDialogue.GetDialogueNodes())
            {
                if (node.GetNodeRect().Contains(mousePosition))
                {
                    dialogueNode = node;
                }
            }

            return dialogueNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetNodeRect(), m_NodeStyle);

            node.SetContent(EditorGUILayout.TextField(node.GetContent()));

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
                    if (m_LinkingParentNode.GetChildren().Contains(node.name))
                    {
                        if (GUILayout.Button("Remove Link"))
                        {
                            m_LinkingParentNode.RemoveChild(node.name);
                        }          
                    }
                    else
                    {
                        if (GUILayout.Button("Add Link"))
                        {
                            m_LinkingParentNode.AddChild(node.name);
                        }          
                    }
                }
            }

            GUILayout.EndArea();
        }
    }
}