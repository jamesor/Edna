using UnityEngine;

namespace JamesOR.Edna
{
    public class MenuManager : MonoBehaviour
    {
        [Header("MenuScreens")]
        public Canvas MainMenu;
        public Canvas LoadMenu;
        public Canvas OptionsMenu;

        private void Awake()
        {
            InitMenus();
        }

        private void InitMenus()
        {
            if (MainMenu == null)
            {
                Debug.LogError("A Main Menu is Required.");
            }
            else
            {
                ShowMainMenu();
            }

            if (LoadMenu == null)
            {
                Debug.LogError("A Load Menu is Required.");
            }
            else
            {
                LoadMenu.gameObject.SetActive(false);
            }

            if (OptionsMenu == null)
            {
                Debug.LogError("An Options Menu is Required.");
            }
            else
            {
                OptionsMenu.gameObject.SetActive(false);
            }
        }

        public void ShowMainMenu()
        {
            if (MainMenu != null)
            {
                MainMenu.gameObject.SetActive(true);
            }
        }
    }
}
