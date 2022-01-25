using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.pointerDrag.GetComponent<Image>().tintColor = Color.red;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventData.pointerDrag.GetComponent<Image>().tintColor = Color.clear;
        }
    }
}
