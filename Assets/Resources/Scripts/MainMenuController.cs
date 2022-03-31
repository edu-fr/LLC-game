using UnityEngine;

namespace Resources.Scripts
{
    public class MainMenuController: MonoBehaviour
    {
        private void Start()
        {
            SoundManager.instance.PlayBGM("menu-music");
        }
        
        public void QuitGame()
        {
            print("Quit");
            Application.Quit();
        }
    }
}
