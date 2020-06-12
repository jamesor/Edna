using UnityEngine;
using UnityEngine.UI;

namespace JamesOR.Edna.UI
{
    public class LoadSaveData
    {
        public Image Image;
        public string Title = "";
        public string DateTime = "";
        public bool IsGameInProgress = false;
    }

    public class LoadSavePanel : MonoBehaviour
    {
        public Image Image;
        public Text Title;
        public Text DateTime;
        public GameObject SaveButton;

        private LoadSaveData m_data;

        public void Init(LoadSaveData data)
        {
            if (data == null)
            {
                Destroy(gameObject);
            }
            else if (m_data == null)
            {
                m_data = data;

                if (Image == null)
                {
                    Debug.LogWarning("LoadSavePanel.Image is Required.");
                }
                else
                {
                    // TODO
                }

                if (Title == null)
                {
                    Debug.LogWarning("LoadSavePanel.Title is Required.");
                }
                else
                {
                    Title.text = m_data.Title;
                }

                if (DateTime == null)
                {
                    Debug.LogWarning("LoadSavePanel.DateTime is Required.");
                }
                else
                {
                    DateTime.text = m_data.DateTime;
                }

                if (SaveButton == null)
                {
                    Debug.LogWarning("LoadSavePanel.SaveButton is Required.");
                }
                else
                {
                    SaveButton.SetActive(m_data.IsGameInProgress);
                }
            }
        }
    }
}
