using System;
using UnityEngine;

namespace JamesOR.Edna.UI
{
    public class LoadMenu : MonoBehaviour
    {
        public GameObject Content;
        public GameObject NewSavegamePrefab;
        public GameObject LoadSavePrefab;
        public GameObject EndGameButton;

        private void OnEnable()
        {
            DisplaySavegameChoices();
            EndGameButton.SetActive(GameManager.HasGameBeenStarted());
        }

        private void OnDisable()
        {
            ClearSavegameChoices();
        }

        private void ClearSavegameChoices()
        {
            foreach (Transform child in Content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void DisplaySavegameChoices()
        {
            // TODO: Load Content Pane with Real Savegame Data.
            bool hasGameBeenStarted = GameManager.HasGameBeenStarted();

            if (hasGameBeenStarted)
            {
                var newSavegamePrefab = Instantiate(NewSavegamePrefab);
                newSavegamePrefab.transform.SetParent(Content.transform, false);
            }

            for (var i = 0; i < 5; i++)
            {
                var loadSavePrefab = Instantiate(LoadSavePrefab);
                loadSavePrefab.transform.SetParent(Content.transform, false);

                LoadSaveData loadSaveData = new LoadSaveData();
                loadSaveData.Title = "Save Game " + i;
                loadSaveData.DateTime = DateTime.Now.ToString();
                loadSaveData.IsGameInProgress = hasGameBeenStarted;

                LoadSavePanel pill = loadSavePrefab.GetComponent<LoadSavePanel>();
                pill.Init(loadSaveData);
            }
        }
    }
}
