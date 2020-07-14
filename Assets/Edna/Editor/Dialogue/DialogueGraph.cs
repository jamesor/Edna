using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using JamesOR.Edna.DataContainers;
using System.Linq;

namespace JamesOR.EdnaEditor.Dialogue
{
    public class DialogueGraph : EditorWindow
    {
        private const string WINDOW_TITLE = "Edna:Dialogue Graph";

        private DialogueGraphView m_dialogueGraphView;
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
            GenerateMiniMap();
            GenerateBlackboard();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_dialogueGraphView);
        }

        private void ConstructGraphView()
        {
            m_dialogueGraphView = new DialogueGraphView(this)
            {
                name = WINDOW_TITLE
            };

            m_dialogueGraphView.StretchToParentSize();
            rootVisualElement.Add(m_dialogueGraphView);
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

            var nodeCreateButton = new Button(() =>
            {
                m_dialogueGraphView.CreateNewDialogueNode("Dialogue Node", Vector2.zero);
            });
            nodeCreateButton.text = "Create Node";
            toolbar.Add(nodeCreateButton);
                        
            rootVisualElement.Add(toolbar);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            var coords = m_dialogueGraphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
            m_dialogueGraphView.Add(miniMap);
        }

        private void GenerateBlackboard()
        {
            var blackboard = new Blackboard(m_dialogueGraphView);
            blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
            blackboard.addItemRequested = bb => { m_dialogueGraphView.AddPropertyToBlackboard(new ExposedProperty()); };
            blackboard.editTextRequested = (bb1, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;
                if (m_dialogueGraphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exisits, please choose another name.", "OK");
                    return;
                }

                var propertyIndex = m_dialogueGraphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                m_dialogueGraphView.ExposedProperties[propertyIndex].PropertyName = newValue;
                ((BlackboardField)element).text = newValue;
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            m_dialogueGraphView.Add(blackboard);

            m_dialogueGraphView.Blackboard = blackboard;
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(m_fileName))
            {
                var saveUtility = DialogueGraphSaveUtility.GetInstance(m_dialogueGraphView);
                if (save)
                {
                    saveUtility.SaveGraph($"Dialogue/{m_fileName}");
                } else
                {
                    saveUtility.LoadGraph($"Dialogue/{m_fileName}");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }
    }
}
