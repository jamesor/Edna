using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using JamesOR.Edna.DataContainers;
using JamesOR.Edna.Utils;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
        public float DefaultCommentBlockSize { get; internal set; }
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        public Blackboard Blackboard;

        private NodeSearchWindow m_nodeSearchWindow;

        public DialogueGraphView(EditorWindow editorWindow)
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
            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            m_nodeSearchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            m_nodeSearchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_nodeSearchWindow);
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

        public void CreateNewDialogueNode(string nodeName, Vector2 position)
        {
            AddElement(CreateDialogueNode(nodeName, position));
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 position)
        {
            var dialogueNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

            var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            var button = new Button(() => { AddChoicePort(dialogueNode); });
            button.text = "New Choice";
            dialogueNode.titleContainer.Add(button);

            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt => 
            {
                dialogueNode.DialogueText = evt.newValue;
                dialogueNode.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(dialogueNode.title);
            dialogueNode.mainContainer.Add(textField);

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(position, DefaultNodeSize));
            
            return dialogueNode;
        }

        public void ClearBlackboardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
        {
            List<string> existingNamesList = (from p in ExposedProperties select p.PropertyName).ToList();
            var localPropertyName = StringUtils.IncrementName(exposedProperty.PropertyName, existingNamesList);
            var localPropertyValue = exposedProperty.PropertyValue;

            var property = new ExposedProperty
            {
                PropertyName = localPropertyName,
                PropertyValue = localPropertyValue
            };
            ExposedProperties.Add(property);

            var container = new VisualElement();
            var blackboardField = new BlackboardField { text = property.PropertyName, typeText = "string" };
            container.Add(blackboardField);

            var propertyValueTextField = new TextField("Value:")
            {
                value = property.PropertyValue
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
                ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
            });

            var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
            container.Add(blackboardValueRow);

            Blackboard.Add(container);
        }

        public void CreateCommentBlock(Rect rect)
        {
            throw new NotImplementedException();
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "")
        {
            var generatedPort = GeneratePort(dialogueNode, Direction.Output);
            generatedPort.styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            generatedPort.AddToClassList("output-port");

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
            textField.AddToClassList("output-field");
            textField.multiline = true;
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

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }

            dialogueNode.outputContainer.Remove(generatedPort);
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
        }

    }
}
