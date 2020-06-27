using System.Collections.Generic;
using System.IO;
using System.Linq;
using JamesOR.Edna.Dialogue;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class DialogueGraphSaveUtility
    {
        private DialogueGraphView m_dialogueGraphView;
        private DialogueContainer m_dialogueContainerSavedData;

        private List<Edge> Edges => m_dialogueGraphView.edges.ToList();
        private List<DialogueNode> Nodes => m_dialogueGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static DialogueGraphSaveUtility GetInstance(DialogueGraphView dialogueGraphView)
        {
            return new DialogueGraphSaveUtility
            {
                m_dialogueGraphView = dialogueGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            if (!SaveNodes(dialogueContainer))
            {
                return;
            }

            SaveExposedProperties(dialogueContainer);

            CreatePathIfNeeeded(fileName);

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            m_dialogueContainerSavedData = Resources.Load<DialogueContainer>(fileName);

            if (m_dialogueContainerSavedData == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist.", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
            CreateExposedProperties();
        }

        private bool SaveNodes(DialogueContainer dialogueContainer)
        {
            if (!Edges.Any())
            {
                return false;
            }

            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as DialogueNode;
                var inputNode = connectedPorts[i].input.node as DialogueNode;

                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

            foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
            {
                dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = dialogueNode.GUID,
                    DialogueText = dialogueNode.DialogueText,
                    Position = dialogueNode.GetPosition().position
                });
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.AddRange(m_dialogueGraphView.ExposedProperties);
        }

        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).GUID = m_dialogueContainerSavedData.NodeLinks[0].BaseNodeGUID;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => m_dialogueGraphView.RemoveElement(edge));
                m_dialogueGraphView.RemoveElement(node);
            }
        }

        private void CreateNodes()
        {
            foreach (var nodeData in m_dialogueContainerSavedData.DialogueNodeData)
            {
                var tempNode = m_dialogueGraphView.CreateDialogueNode(nodeData.DialogueText, Vector2.zero);
                tempNode.GUID = nodeData.NodeGUID;
                m_dialogueGraphView.AddElement(tempNode);

                var nodePorts = m_dialogueContainerSavedData.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
                nodePorts.ForEach(x => m_dialogueGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = m_dialogueContainerSavedData.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        m_dialogueContainerSavedData.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        m_dialogueGraphView.DefaultNodeSize));
                }
            }
        }

        private void CreateExposedProperties()
        {
            m_dialogueGraphView.ClearBlackboardAndExposedProperties();

            foreach (var exposedProperty in m_dialogueContainerSavedData.ExposedProperties)
            {
                m_dialogueGraphView.AddPropertyToBlackboard(exposedProperty);
            }
        }

        private void LinkNodes(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);

            m_dialogueGraphView.Add(tempEdge);
        }

        private void CreatePathIfNeeeded(string fileName)
        {
            string parent = "Assets";
            string folder;
            string[] folders = $"Resources/{fileName}".Split('/');

            for (var i = 0; i < folders.Length - 1; i++)
            {
                folder = folders[i];
                if (!AssetDatabase.IsValidFolder($"{parent}/{folder}"))
                {
                    AssetDatabase.CreateFolder(parent, folder);
                    parent = $"{parent}/{folder}";
                }
            }
        }
    }
}
