using System;
using JetBrains.Annotations;
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
      [SerializeField] private Transform headerArea;
      [SerializeField] private TextMeshProUGUI titleField;

      [Header("Content")] 
      [SerializeField] private Transform contentArea;
      
      [SerializeField] private Transform verticalLayoutArea;
      [SerializeField] private RawImage verticalVideoTutorial;
      [SerializeField] private Image verticalImage;
      [SerializeField] private TextMeshProUGUI verticalText;
      
      [Space()] 
      [SerializeField] private Transform horizontalLayoutArea;
      [SerializeField] private RawImage horizontalVideoTutorial;
      [SerializeField] private Image horizontalImage;
      [SerializeField] private TextMeshProUGUI horizontalText;

      [Space()] 
      [SerializeField] private Transform horizontalLayoutIconArea;
      [SerializeField] private Transform iconArea;
      [SerializeField] private RawImage horizontalIconVideoTutorial;
      [SerializeField] private Image horizontalIcon;
      [SerializeField] private TextMeshProUGUI horizontalIconText;

      [Header("Footer")] 
      [SerializeField] private Transform footerArea;
      [SerializeField] private Button confirmButton;
      [SerializeField] private Button declineButton;
      [SerializeField] private Button alternateButton;

      private Action _onConfirmAction;
      private Action _onDeclineAction;
      private Action _onAlternateAction;

      public void Confirm()
      {
         _onConfirmAction?.Invoke();
         // Close();
      }

      public void Decline()
      {
         _onDeclineAction?.Invoke();
         // Close();
      }

      public void Alternate()
      {
         _onAlternateAction?.Invoke();
         // Close();
      }

      public void ShowMessage(WindowType windowType, string title, string message, string confirmText, 
         string declineText, string alternateText, Action confirmAction, Action declineAction = null,
         Action alternateAction = null, Sprite imageToShow = null, RawImage videoTutorial = null)
      {
         gameObject.SetActive(true);
         
         SetWindowType(windowType);
         
         // Hide the header if there's no title
         var hasTitle = !string.IsNullOrEmpty(title);
         headerArea.gameObject.SetActive(hasTitle);
         titleField.text = title;
         
         SetContent(windowType, message, imageToShow, videoTutorial);

         // Setting buttons
         confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;
         _onConfirmAction = confirmAction;

         var hasDecline = (declineAction != null);
         declineButton.gameObject.SetActive(hasDecline);
         declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineText;
         _onDeclineAction = declineAction;

         var hasAlternate = (alternateAction != null);
         alternateButton.gameObject.SetActive(hasAlternate);
         alternateButton.GetComponentInChildren<TextMeshProUGUI>().text = alternateText;
         _onAlternateAction = alternateAction;
      }

      private void SetWindowType(WindowType windowType)
      {
         switch (windowType)
         {
            case WindowType.Vertical:
               horizontalLayoutArea.gameObject.SetActive(false);
               horizontalLayoutIconArea.gameObject.SetActive(false);
               verticalLayoutArea.gameObject.SetActive(true);
               break;
            
            case WindowType.Horizontal:
               horizontalLayoutArea.gameObject.SetActive(true);
               horizontalLayoutIconArea.gameObject.SetActive(false);
               verticalLayoutArea.gameObject.SetActive(false);
               break;
            
            case WindowType.HorizontalIcon:
               horizontalLayoutArea.gameObject.SetActive(false);
               horizontalLayoutIconArea.gameObject.SetActive(true);
               verticalLayoutArea.gameObject.SetActive(false);
               break;
            
            default:
               print("Unrecognized window type!");
               break; 
         }
      }
      
      private void SetContent(WindowType windowType, string message, Sprite imageToShow = null, RawImage videoTutorial = null)
      {
         switch (windowType)
         {
            case WindowType.Vertical:
               if (imageToShow != null)
               {
                  verticalVideoTutorial.gameObject.SetActive(false);
                  verticalImage.gameObject.SetActive(true);
                  verticalImage.sprite = imageToShow;
               }
               if (videoTutorial != null)
               {
                  verticalImage.gameObject.SetActive(false);
                  verticalVideoTutorial.gameObject.SetActive(true);
                  verticalVideoTutorial.texture = videoTutorial.texture;
               }
               verticalText.text = message;
               break;
            
            case WindowType.Horizontal:
               if (imageToShow != null)
               {
                  horizontalVideoTutorial.gameObject.SetActive(false);
                  horizontalImage.gameObject.SetActive(true);
                  horizontalImage.sprite = imageToShow;
               }
               if (videoTutorial != null)
               {
                  horizontalVideoTutorial.gameObject.SetActive(true);
                  horizontalVideoTutorial.texture = videoTutorial.texture;
                  horizontalImage.gameObject.SetActive(false);
               }
               horizontalText.text = message;
               break;
            
            case WindowType.HorizontalIcon:
               if (imageToShow != null)
               {
                  horizontalVideoTutorial.gameObject.SetActive(false);
                  horizontalIcon.gameObject.SetActive(true);
                  horizontalIcon.sprite = imageToShow;
               }
               else if (videoTutorial != null)
               {
                  horizontalIcon.gameObject.SetActive(false);
                  horizontalIconVideoTutorial.gameObject.SetActive(true);
                  horizontalIconVideoTutorial.texture = videoTutorial.texture;
               }
               horizontalIconText.text = message;
               break;
            
            default:
               print("Unrecognized window type!");
               break; 
         }
      }

   }
}
