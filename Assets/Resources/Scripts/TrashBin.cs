using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.gameObject.CompareTag("Production") &&
                eventData.pointerDrag.gameObject.GetComponent<Draggable>().CanBeDeleted)
            {
                eventData.pointerDrag.gameObject.GetComponent<Draggable>().IsOnValidPositionToDrop = true;
                Destroy(eventData.pointerDrag.gameObject);
                print("Deletado com sucesso!");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!eventData.pointerDrag) return; 
            eventData.pointerDrag.gameObject.GetComponent<UIEffect>().effectMode = EffectMode.Sepia;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.pointerDrag) return;
            eventData.pointerDrag.gameObject.GetComponent<UIEffect>().effectMode = EffectMode.None;
        }
    }
}
