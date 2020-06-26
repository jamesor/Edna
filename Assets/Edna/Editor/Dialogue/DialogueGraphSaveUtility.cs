using System.Collections.Generic;
using System.Linq;
using JamesOR.Edna.Dialogue;
using JamesOR.EdnaEditor.Dialogue;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphSaveUtility
{
    private DialogueGraphView m_targetGraphView;
    private DialogueContainer m_dialogueContainerCache;

    private List<Edge> Edges => m_targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => m_targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static DialogueGraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new DialogueGraphSaveUtility
        {
            m_targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!Edges.Any())
        {
            return;
        }

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

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

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        m_dialogueContainerCache = Resources.Load<DialogueContainer>(fileName);

        if (m_dialogueContainerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist.", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = m_dialogueContainerCache.NodeLinks[0].BaseNodeGUID;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => m_targetGraphView.RemoveElement(edge));
            m_targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in m_dialogueContainerCache.DialogueNodeData)
        {
            var tempNode = m_targetGraphView.CreateDialogueNode(nodeData.DialogueText);
            tempNode.GUID = nodeData.NodeGUID;
            m_targetGraphView.AddElement(tempNode);

            var nodePorts = m_dialogueContainerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => m_targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = m_dialogueContainerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count(); j++)
            {
                var targetNodeGUID = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                    m_dialogueContainerCache.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                    m_targetGraphView.DefaultNodeSize));
            }
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
        m_targetGraphView.Add(tempEdge);
    }
}
