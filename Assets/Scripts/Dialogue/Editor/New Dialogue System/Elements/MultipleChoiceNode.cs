using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MultipleChoiceNode : BaseNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        m_DialogueType = DialogueType.MultipleChoice;

        m_Choices.Add("New Choice");
    }

    public override void Draw()
    {
        base.Draw();

        //Main Container
        Button addChoiceButton = new()
        {
            text = "Add Choice"
        };

        mainContainer.Insert(1, addChoiceButton);

        //Output container

        foreach (string choice in m_Choices)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.portName = "";

            Button deleteChoiceButton = new()
            {
                text = "-"
            };

            TextField choiceTextField = new()
            {
                value = choice
            };

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            outputContainer.Add(choicePort);
        }

        RefreshExpandedState();
    }
}
