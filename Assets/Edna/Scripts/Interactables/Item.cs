using UnityEngine;
using UnityEngine.Localization;

namespace JamesOR.Edna.Interactables
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Edna/Interactables/Item")]
    public class Item : ScriptableObject
    {
        [Header("Item Info")]
        public Sprite Icon = null;
        public bool IsReusable = true;
        public bool IsDefaultItem = false;

        [Header("Localization")]
        public LocalizedString StringReferenceName = new LocalizedString();
        public LocalizedString StringReferenceLook = new LocalizedString();

        private string m_name = "Unnamed";
        public string Name { get => m_name; }

        private string m_description = "";
        public string Description { get => m_description; }

        private void OnEnable()
        {
            if (StringReferenceName != null)
            {
                StringReferenceName.RegisterChangeHandler(UpdateName);
            }
            if (StringReferenceLook != null)
            {
                StringReferenceLook.RegisterChangeHandler(UpdateDescription);
            }
        }

        private void OnDisable()
        {
            if (StringReferenceName != null)
            {
                StringReferenceName.ClearChangeHandler();
            }
            if (StringReferenceLook != null)
            {
                StringReferenceLook.ClearChangeHandler();
            }
        }

        private void UpdateName(string value)
        {
            m_name = value;
        }

        private void UpdateDescription(string value)
        {
            m_description = value;
        }
    }
}
