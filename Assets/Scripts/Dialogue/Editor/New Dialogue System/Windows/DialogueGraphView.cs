using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView() 
    {
        AddGridBackground();
        AddStyles();
        AddManipulators();
    }

    private void AddManipulators()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(CreateNodeContextualMenu("Add Node - Single Choice", DialogueType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add Node - Multiple Choice", DialogueType.MultipleChoice));
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
    {
        ContextualMenuManipulator contextManipulator = new(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, actionEvent.eventInfo.mousePosition)))
            );

        return contextManipulator;
    }

    private BaseNode CreateNode(DialogueType dialogueType, Vector2 position)
    {
        Type nodeType = Type.GetType($"{dialogueType}Node");

        BaseNode node = (BaseNode)Activator.CreateInstance(nodeType);

        node.Initialize(position);
        node.Draw();

        return node;
    }

    private void AddStyles()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Dialogue/DialogueEditor.uss");

        styleSheets.Add(styleSheet);
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }
}
