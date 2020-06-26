using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class DialogueGraph : EditorWindow
    {
        private const string WINDOW_TITLE = "Edna:Dialogue Graph";

        private DialogueGraphView m_graphView;
        private string m_fileName = "New Narrative";

        [MenuItem("Edna/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent(WINDOW_TITLE);
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_graphView);
        }

        private void ConstructGraphView()
        {
            m_graphView = new DialogueGraphView
            {
                name = WINDOW_TITLE
            };

            m_graphView.StretchToParentSize();
            rootVisualElement.Add(m_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(m_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => m_fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

            var nodeCreateButton = new Button(() => { m_graphView.CreateNode("Dialogue Node"); });
            nodeCreateButton.text = "Create Node";
            toolbar.Add(nodeCreateButton);
                        
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(m_fileName))
            {
                var saveUtility = DialogueGraphSaveUtility.GetInstance(m_graphView);
                if (save)
                {
                    saveUtility.SaveGraph(m_fileName);
                } else
                {
                    saveUtility.LoadGraph(m_fileName);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }
    }
}
