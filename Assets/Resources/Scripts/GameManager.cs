using UnityEngine;

namespace Resources.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private SoundManager _soundManager;
        public PlayerData playerData;
        public int reachedLevel;
        public int[] score;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            
            DontDestroyOnLoad(gameObject);
            print("Trying to load saved data");
            LoadGame();
            RestorePlayerPrefs();
        }

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
        
        public void RestorePlayerPrefs()
        {
            print("RESTORING PLAYER PREFS");
            _soundManager.SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));
            _soundManager.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        }

    }
}
