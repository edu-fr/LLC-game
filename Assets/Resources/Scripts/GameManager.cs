using UnityEngine;

namespace Resources.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                print("Trying to load saved data");
                LoadGame();
            }
        }
        
        public PlayerData playerData;
        
        public int reachedLevel;
        
        public int[] score;
    

        public void SaveGame()
        {
            UpdatePlayerData();
            SaveSystem.SaveProgress(playerData);
        }

        public void LoadGame()
        {
            var loadedData = SaveSystem.LoadPlayer();
            if (loadedData == null) return;
            UpdateGameManagerData(loadedData);
            UpdatePlayerData();
        }

        public void UpdatePlayerData()
        {
            playerData.reachedLevel = reachedLevel;
            playerData.score = score;
        }

        public void UpdateGameManagerData(PlayerData givenPlayerData)
        {
            reachedLevel = givenPlayerData.reachedLevel;
            score = givenPlayerData.score;
        }
    }
}
