using UnityEngine;

namespace JamesOR.Edna
{
    public class GameManager : Singleton<GameManager>
    {
        public MenuManager MenuManager;
        
        private bool m_hasGameBeenStarted = false;

        protected override void OnAwake()
        {
            InitGame();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
                MenuManager.ShowMainMenu();
            }
        }

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
            MenuManager.ShowMainMenu();
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
    }
}
