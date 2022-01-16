using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        public Vector3 OriginalPosition { get; set; }
        public bool CanBeDragged;
        private void Awake()
        {
            _canvas = FindObjectOfType<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            CanBeDragged = false;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            print("test");
        }
     
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            print("Begin dragging");
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            print("end dragging");
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
            transform.localPosition = OriginalPosition;
        }

    }
}
