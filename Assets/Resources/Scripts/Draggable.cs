using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup; 
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
            if (Input.GetMouseButtonDown(1))         // Botão direito
            { 
                if (CanBeDeleted)
                {
                    var boxRef = AttachedTo.GetComponent<InternalBox>().FillableBoxImAttachedTo;
                    boxRef.GetComponent<FillableBox>().RemoveAndReconstructList(gameObject, draggable: true, deletable: true, grayscale: false, destroy: true);
                }
                else 
                {
                    print("Cannot be deleted! pois deletable: " + CanBeDeleted);
                }
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
            AttachedTo.GetComponentInParent<FillableBox>().RemoveAndReconstructList(gameObject, draggable: gameObject.GetComponent<Draggable>().CanBeDragged, deletable: gameObject.GetComponent<Draggable>().CanBeDeleted, grayscale: false, destroy: false);
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
                if (gameObject.GetComponent<BoxContent>().Production == null)
                {
                    print("VARIÁVEL voltando porque o drop deu errado!");
                    AttachedTo.transform.parent.GetComponentInParent<FillableBox>().InsertAndReconstructList(GetComponent<BoxContent>().Variable, draggable: gameObject.GetComponent<Draggable>().CanBeDragged, deletable: gameObject.GetComponent<Draggable>().CanBeDeleted, grayscale: false);
                    print("Variavel: " + GetComponent<BoxContent>().Variable + " inserida novamente na lista");
                    Destroy(gameObject);
                }
                else
                {
                    print("PRODUÇÃO voltando porque o drop deu errado!");
                    AttachedTo.transform.parent.GetComponentInParent<FillableBox>().InsertAndReconstructList(GetComponent<BoxContent>().Production, draggable: gameObject.GetComponent<Draggable>().CanBeDragged, deletable: gameObject.GetComponent<Draggable>().CanBeDeleted, grayscale: false);
                    print("Produção: " + GetComponent<BoxContent>().Production + " inserida novamente na lista");
                    Destroy(gameObject);
                }
            }
            else
            {
                print("Tava em valid position");
            }
        }
    }
}
