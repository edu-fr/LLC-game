using UnityEngine;

namespace Resources.Scripts
{
    public class MainMenuController: MonoBehaviour
    {
        public void QuitGame()
        {
            print("Quit");
            Application.Quit();
        }
    }
}
