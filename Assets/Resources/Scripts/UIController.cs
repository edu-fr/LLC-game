using UnityEngine;

namespace Resources.Scripts
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        [SerializeField] private ModalWindowPanel modalWindow;

        public ModalWindowPanel ModalWindow => modalWindow;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }
    }
}
