using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class DialogueEditorWindow : EditorWindow
{
    [MenuItem("Window/New Dialogue Editor")]
    public static void ShowExample()
    {
        GetWindow<DialogueEditorWindow>("New Dialogue Editor");
    }

    private void CreateGUI()
    {
        AddGraphView();

        AddStyles();
    }

    private void AddStyles()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Dialogue/StyleVariables.uss");

        rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void AddGraphView()
    {
        DialogueGraphView graphView = new();

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }
}