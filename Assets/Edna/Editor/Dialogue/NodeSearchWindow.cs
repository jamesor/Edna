using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow m_editorWindow;
        private DialogueGraphView m_dialogueGraphView;

        private Texture2D m_indentationIcon;

        public void Configure(EditorWindow window, DialogueGraphView graphView)
        {
            m_editorWindow = window;
            m_dialogueGraphView = graphView;

            //Transparent 1px indentation icon as a hack
            m_indentationIcon = new Texture2D(1, 1);
            m_indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue Node", m_indentationIcon))
                {
                    level = 2, userData = new DialogueNode()
                },
                new SearchTreeEntry(new GUIContent("Comment Block",m_indentationIcon))
                {
                    level = 1,
                    userData = new Group()
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = m_editorWindow.rootVisualElement.ChangeCoordinatesTo(
                m_editorWindow.rootVisualElement.parent,
                context.screenMousePosition - m_editorWindow.position.position);

            var graphMousePosition = m_dialogueGraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            switch (SearchTreeEntry.userData)
            {
                case DialogueNode dialogueNode:
                    m_dialogueGraphView.CreateNewDialogueNode("Dialogue Node", graphMousePosition);
                    return true;
                case Group group:
                    //var rect = new Rect(graphMousePosition, m_dialogueGraphView.DefaultCommentBlockSize);
                    //m_dialogueGraphView.CreateCommentBlock(rect);
                    return true;
            }
            return false;
        }
    }
}