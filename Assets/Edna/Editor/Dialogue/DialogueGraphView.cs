using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName));
        }

        public DialogueNode CreateDialogueNode(string nodeName)
        {
            var dialogueNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            var button = new Button(() => { AddChoicePort(dialogueNode); });
            button.text = "New Choice";
            dialogueNode.titleContainer.Add(button);

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(Vector3.zero, DefaultNodeSize));
            
            return dialogueNode;
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "")
        {
            var generatedPort = GeneratePort(dialogueNode, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

            var choicePortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Choice {outputPortCount}"
                : overriddenPortName;

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);

            var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);

            generatedPort.portName = choicePortName;
            dialogueNode.outputContainer.Add(generatedPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
        }

        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        private DialogueNode GenerateEntryPointNode()
        {
            var node = new DialogueNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true
            };

            var generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (!targetEdge.Any())
            {
                return;
            }

            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());

            dialogueNode.outputContainer.Remove(generatedPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
        }

    }
}
