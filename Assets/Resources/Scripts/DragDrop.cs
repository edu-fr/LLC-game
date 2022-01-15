using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Canvas canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            print("test");
        }
     
        public void OnBeginDrag(PointerEventData eventData)
        {
            print("Begin dragging");
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            print("end dragging");
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
        }


    }
}
