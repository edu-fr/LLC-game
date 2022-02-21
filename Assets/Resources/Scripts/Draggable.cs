using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        public Vector3 LastValidPosition { get; set; }
        public Vector3 OriginalPosition { get; set; }
        public bool CanBeDragged;
        public bool CanBeDeleted;
        public GameObject AttachedTo;
        public GameObject OriginalAttachedObject;
        public bool IsOnValidPositionToDrop;
        private void Awake()
        {
            _canvas = FindObjectOfType<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanBeDeleted)
            {
                if (Input.GetMouseButtonDown(1))         // Botão direito
                { 
                    var productionsBoxRef = AttachedTo.GetComponent<InternalBox>().FillableBoxImAttachedTo;
                    productionsBoxRef.GetComponent<ProductionsBox>().RemoveProductionAndReconstructList(gameObject);
                }
            }
            else
            {
                print("Cannot be deleted! POIS: " + CanBeDeleted);
            }
        }

        private void Start()
        {
            IsOnValidPositionToDrop = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            IsOnValidPositionToDrop = false;
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
            // Remove from current box and set the Canvas as new parent
            AttachedTo.GetComponentInParent<FillableBox>().RemoveFromLists(gameObject);
            gameObject.transform.SetParent(_canvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanBeDragged) return;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;

            if (!IsOnValidPositionToDrop)
            {
                print("Not on valid position!");
                transform.SetParent(AttachedTo.transform);
                transform.localPosition = LastValidPosition;
                AttachedTo.GetComponentInParent<FillableBox>().AddToLists(gameObject);
            }

        }
        
    }
}
