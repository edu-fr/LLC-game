using System;
using UnityEngine;

namespace Resources.Scripts
{
    public class MainMenuController: MonoBehaviour
    {
        public GameObject introductionPanel;
        private GameManager _gameManager;

        private void Awake()
        {
            Screen.SetResolution(1280, 720, false, 60);
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        private void Start()
        {
            SoundManager.instance.PlayBGM("menu-music");
            if (!_gameManager.alreadyShowedIntroduction)
            {   
                introductionPanel.SetActive(true);
                _gameManager.alreadyShowedIntroduction = true;
            }
        }
        
        public void QuitGame()
        {
            print("Quit");
            Application.Quit();
        }
    }
}
