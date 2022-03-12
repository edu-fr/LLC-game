using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
   public class ModalWindowPanel : MonoBehaviour
   {
      public enum WindowType
      {
         Vertical, Horizontal, HorizontalIcon
      }
      
      [Header("Header")] 
      [SerializeField] private Transform _headerArea;
      [SerializeField] private TextMeshProUGUI _titleField;

      [Header("Content")] 
      [SerializeField] private Transform _contentArea;
      
      [SerializeField] private Transform _verticalLayoutArea;
      [SerializeField] private Image _verticalImage;
      [SerializeField] private TextMeshProUGUI _verticalText;
      
      [Space()] 
      [SerializeField] private Transform _horizontalLayoutArea;
      [SerializeField] private Image _horizontalImage;
      [SerializeField] private TextMeshProUGUI _horizontalText;

      [Space()] 
      [SerializeField] private Transform _horizontalLayoutIconArea;
      [SerializeField] private Transform _iconArea;
      [SerializeField] private Image _horizontalIcon;
      [SerializeField] private TextMeshProUGUI _horizontalIconText;

      [Header("Footer")] 
      [SerializeField] private Transform _footerArea;
      [SerializeField] private Button _confirmButton;
      [SerializeField] private Button _declineButton;
      [SerializeField] private Button _alternateButton;

      private Action onConfirmAction;
      private Action onDeclineAction;
      private Action onAlternateAction;

      public void Confirm()
      {
         print("Tentando confirmar!");
         onConfirmAction?.Invoke();
         // Close();
      }

      public void Decline()
      {
         
         print("Tentando declinar!");
         onConfirmAction?.Invoke();
         // Close();
      }

      public void Alternate()
      {
         
         print("Tentando alternar!");
         onAlternateAction?.Invoke();
         // Close();
      }

      public void ShowMessage(WindowType windowType, string title, Sprite imageToShow, string message, string confirmText, string declineText, string alternateText, Action confirmAction,
         Action declineAction = null, Action alternateAction = null)
      {
         gameObject.SetActive(true);
         
         SetWindowType(windowType);
         
         // Hide the header if there's no title
         var hasTitle = string.IsNullOrEmpty(title);
         _headerArea.gameObject.SetActive(hasTitle);
         _titleField.text = title;
         
         SetContent(windowType, imageToShow, message);

         // Setting buttons
         _confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;
         onConfirmAction = confirmAction;

         var hasDecline = (declineAction != null);
         _declineButton.gameObject.SetActive(hasDecline);
         _declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineText;
         onDeclineAction = declineAction;

         var hasAlternate = (alternateAction != null);
         _alternateButton.gameObject.SetActive(hasAlternate);
         _alternateButton.GetComponentInChildren<TextMeshProUGUI>().text = alternateText;
         onAlternateAction = alternateAction;

      }

      private void SetWindowType(WindowType windowType)
      {
         switch (windowType)
         {
            case WindowType.Vertical:
               _horizontalLayoutArea.gameObject.SetActive(false);
               _horizontalLayoutIconArea.gameObject.SetActive(false);
               _verticalLayoutArea.gameObject.SetActive(true);
               break;
            
            case WindowType.Horizontal:
               _horizontalLayoutArea.gameObject.SetActive(true);
               _horizontalLayoutIconArea.gameObject.SetActive(false);
               _verticalLayoutArea.gameObject.SetActive(false);
               break;
            
            case WindowType.HorizontalIcon:
               _horizontalLayoutArea.gameObject.SetActive(false);
               _horizontalLayoutIconArea.gameObject.SetActive(true);
               _verticalLayoutArea.gameObject.SetActive(false);
               break;
            
            default:
               print("Unrecognized window type!");
               break; 
         }
      }
      
      private void SetContent(WindowType windowType, Sprite imageToShow, string message)
      {
         switch (windowType)
         {
            case WindowType.Vertical:
               _verticalImage.sprite = imageToShow;
               _verticalText.text = message;
               break;
            
            case WindowType.Horizontal:
               _horizontalImage.sprite = imageToShow;
               _horizontalText.text = message;
               break;
            
            case WindowType.HorizontalIcon:
               _horizontalIcon.sprite = imageToShow;
               _horizontalIconText.text = message;
               break;
            
            default:
               print("Unrecognized window type!");
               break; 
         }
      }

   }
}
