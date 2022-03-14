using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Resources.Scripts
{
    public class WindowTrigger : MonoBehaviour
    {
        public string title;
        public Sprite sprite;
        [TextArea(10, 30)] public string message;
        public string confirmText;
        public string declineText;
        public string alternateText;
        public bool triggerOnEnable;
        public ModalWindowPanel.WindowType windowType;

        public UnityEvent onContinueEvent;
        public UnityEvent onCancelEvent;
        public UnityEvent onAlternateEvent;
        
        public void OnEnable()
        {
            if (!triggerOnEnable) { return; }

            Action continueCallback = null;
            Action cancelCallback = null;
            Action alternateCallback = null;

            if (onContinueEvent.GetPersistentEventCount() > 0)
            {
                continueCallback = onContinueEvent.Invoke; 
            }

            if (onCancelEvent.GetPersistentEventCount() > 0)
            {
                cancelCallback = onCancelEvent.Invoke;
            }

            if (onAlternateEvent.GetPersistentEventCount() > 0)
            {
                alternateCallback = onAlternateEvent.Invoke;
            }
            
            UIController.Instance.ModalWindow.ShowMessage(windowType, title, sprite, message,
                confirmText, declineText, alternateText, continueCallback, cancelCallback, alternateCallback);
        }
    }
}
