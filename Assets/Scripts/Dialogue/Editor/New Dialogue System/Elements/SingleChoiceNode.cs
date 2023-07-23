using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SingleChoiceNode : BaseNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        m_DialogueType = DialogueType.SingleChoice;

        m_Choices.Add("Next Dialogue");
    }

    public override void Draw()
    {
        base.Draw();

        //Output container

        foreach (string choice in m_Choices) 
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.portName = choice;

            outputContainer.Add(choicePort);
        }

        RefreshExpandedState();
    }

}
