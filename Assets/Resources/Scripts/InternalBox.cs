using UnityEngine;

namespace Resources.Scripts
{
   public class InternalBox : MonoBehaviour
   {
      [SerializeField] private GameObject fillableBoxImAttachedToGameObject;
      public FillableBox FillableBoxImAttachedTo { get; private set; }

      private void Start()
      {
         FillableBoxImAttachedTo = fillableBoxImAttachedToGameObject.GetComponent<FillableBox>();
      }
   }
}
