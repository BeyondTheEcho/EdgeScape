using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    public string m_DialogueName { get; set; }
    public List<string> m_Choices {  get; set; }
    public string m_Text { get; set; }
    public DialogueType m_DialogueType { get; set; }

    public virtual void Initialize(Vector2 position)
    {
        m_DialogueName = "Dialogue Name";
        m_Choices = new List<string>();
        m_Text = "Dialogue Text";

        SetPosition(new Rect(position, Vector2.zero));
    }

    public virtual void Draw()
    {
        // Title Container
        TextField dialogueNameText = new() { value = m_DialogueName };
        titleContainer.Insert(0, dialogueNameText);

        //Input Container
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "Dialogue Connection";
        inputContainer.Add(inputPort);

        //Extension Container
        VisualElement dataContainer = new VisualElement();

        Foldout textFoldout = new()
        {
            text = "Dialogue Text"
        };

        TextField secondTextField = new()
        {
            value = m_Text
        };

        textFoldout.Add(secondTextField);

        dataContainer.Add(textFoldout);

        extensionContainer.Add(dataContainer);
    }
}
