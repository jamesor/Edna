using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEditor;

namespace JamesOR.EdnaEditor
{
    [CreateAssetMenu(fileName = "New String Importer", menuName = "Edna/Localization/String Importer")]
    public class StringImporter : ScriptableObject
    {
        [Header("String Tables")]
        public SharedTableData SharedTable;
        public StringTable[] StringTables;

        [Header("Data Source Files (Tab Delimited)")]
        public TextAsset TabDelimitedFile;

        public void Import()
        {
            if (SharedTable == null)
            {
                Debug.LogError("A SharedTableData reference is required.");
                return;
            }

            if (StringTables == null || StringTables.Length == 0)
            {
                Debug.LogError("One or more StringTable references are required.");
                return;
            }

            if (TabDelimitedFile == null)
            {
                Debug.LogError("A tab delimited file reference is required.");
                return;
            }

            Dictionary<string, int> langNames = new Dictionary<string, int>();
            for (var i = 0; i < StringTables.Length; i++)
            {
                langNames.Add(StringTables[i].LocaleIdentifier.ToString(), i);
            }

            if (langNames.Count == 0)
            {
                Debug.LogError("At least one language needs to be configured in LocalizationSettings.");
                return;
            }

            Dictionary<string, int> fieldNames = new Dictionary<string, int>();

            var splitFile = new string[] { "\r\n", "\r", "\n" };
            var splitLine = new string[] { "\t" };
            var lines = TabDelimitedFile.text.Split(splitFile, StringSplitOptions.None);
            var isFieldNameRow = true;

            // We need at minimum the Field Names Row and a Data Row
            if (lines.Length < 2)
            {
                Debug.LogError("The tab delimited file needs to contain at minimum a field name row and a data row.");
                return;
            }

            foreach (var lang in langNames)
            {
                StringTables[lang.Value].Clear();
                Undo.RecordObject(StringTables[lang.Value], "Changed translated text");
                EditorUtility.SetDirty(StringTables[lang.Value]);
            }

            if (SharedTable != null)
            {
                Undo.RecordObject(SharedTable, "Changed translated text");
                EditorUtility.SetDirty(SharedTable);
            }

            for (uint i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Split(splitLine, StringSplitOptions.None);

                if (isFieldNameRow)
                {
                    for (int j = 0; j < line.Length; j++)
                    {
                        fieldNames.Add(line[j], j);
                    }
                    isFieldNameRow = false;
                }
                else if (line[fieldNames["key"]] == "!!_IGNORE_!!")
                {
                    // Ignore this row because it just contains a template forumla for Google Translate
                }
                else
                {
                    foreach (var lang in langNames)
                    {
                        StringTables[lang.Value].AddEntry(line[fieldNames["key"]], line[fieldNames[lang.Key]]);
                    }
                }
            }

            Debug.Log("Finished importing text");
        }
    }

    [CustomEditor(typeof(StringImporter))]
    public class TestScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (StringImporter)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Language Strings", GUILayout.Height(40)))
            {
                script.Import();
            }
            GUILayout.EndHorizontal();
        }
    }
}
