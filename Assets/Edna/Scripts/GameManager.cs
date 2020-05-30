using UnityEngine;

namespace JamesOR.Edna
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("MenuScreens")]
        public Canvas MainMenu;
        public Canvas LoadMenu;
        public Canvas OptionsMenu;
        
        private bool m_hasGameBeenStarted = false;

        protected override void OnAwake()
        {
            InitGame();
            InitMenus();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
                ShowMainMenu();
            }
        }

        #region Menus
        private void InitMenus()
        {
            if (MainMenu == null)
            {
                Debug.LogWarning("A Main Menu is Required.");
            }
            else
            {
                MainMenu.gameObject.SetActive(true);
            }

            if (LoadMenu == null)
            {
                Debug.LogWarning("A Load Menu is Required.");
            }
            else
            {
                LoadMenu.gameObject.SetActive(false);
            }

            if (OptionsMenu == null)
            {
                Debug.LogWarning("An Options Menu is Required.");
            }
            else
            {
                OptionsMenu.gameObject.SetActive(false);
            }
        }
        private void ShowMainMenu()
        {
            MainMenu.gameObject.SetActive(true);
        }
        #endregion

        #region Game
        private void InitGame()
        {
            PauseGame();
        }
        public void PauseGame()
        {
            Time.timeScale = 0;
        }
        public void UnPauseGame()
        {
            Time.timeScale = 1;
        }
        public void StartNewGame()
        {
            m_hasGameBeenStarted = true;
            UnPauseGame();
            // Reset Game To Starting State
        }
        public void ContinueGame()
        {
            UnPauseGame();
        }
        public void SaveGame()
        {
            // TODO: Save the game
            Debug.Log("SAVE GAME");
        }
        public void EndGame()
        {
            PauseGame();
            SaveGame();
            m_hasGameBeenStarted = false;
            MainMenu.gameObject.SetActive(true);
        }
        public void QuitApplication()
        {
            PauseGame();
            SaveGame();
            Application.Quit();
        }
        internal static bool HasGameBeenStarted()
        {
            return Instance.m_hasGameBeenStarted;
        }
        #endregion
    }
}
