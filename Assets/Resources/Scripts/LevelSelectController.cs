using System;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class LevelSelectController : MonoBehaviour
    {
        public Button[] _levelButtons;
        private GameManager _gameManager;

        private void Awake()
        {
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            var reachedLevel = _gameManager.reachedLevel;

            _levelButtons = new Button[transform.childCount];

            for (var i = 0; i < _levelButtons.Length; i++)
            {
                if (transform.GetChild(i).GetComponent<Button>() == null) continue;
                _levelButtons[i] = transform.GetChild(i).GetComponent<Button>();
                _levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                _levelButtons[i].interactable = i + 1 <= reachedLevel;
                if (!_levelButtons[i].interactable)
                {
                    _levelButtons[i].GetComponent<UIEffect>().effectMode = EffectMode.Grayscale;
                    _levelButtons[i].GetComponent<UIEffect>().colorFactor = 0;
                } else {
                    _levelButtons[i].GetComponent<UIEffect>().effectMode = EffectMode.None;
                    _levelButtons[i].GetComponent<UIEffect>().colorFactor = 1;
                }
            }
            
        }

        public void LoadLevel(int level)
        {
            SceneManager.LoadScene("Level " + level);
        }

        public void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public void NextLevel()
        {
            if (SceneManager.GetActiveScene().buildIndex < 6)
            {
                if (SceneManager.GetActiveScene().buildIndex + 1 > _gameManager.reachedLevel)
                {
                    _gameManager.reachedLevel = SceneManager.GetActiveScene().buildIndex + 1;
                    _gameManager.SaveGame();
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else 
                LoadMenu();
        }

    }
    
}
