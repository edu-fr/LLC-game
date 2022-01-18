using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public abstract class FillableBox : MonoBehaviour
    {
        protected RectTransform FillableBoxTransform;
        [SerializeField] protected GameObject titleBox;
        [SerializeField] protected float titleBoxOffset;
        [SerializeField] protected GameObject scrollBar;
        protected ColorBlock ScrollBarDefaultColorBlock;
        
        protected virtual void Awake()
        {
            FillableBoxTransform = GetComponent<RectTransform>();
        }
    
        protected virtual void Start()
        {
            ScrollBarDefaultColorBlock = scrollBar.GetComponent<Scrollbar>().colors;
        }
        
        protected virtual void SetGrayScale(bool option)
        {
            var currentEffectMode = option ? EffectMode.Grayscale : EffectMode.None;
            var currentColorBlock = option ? ColorBlock.defaultColorBlock : ScrollBarDefaultColorBlock;

            gameObject.GetComponent<UIEffect>().effectMode = currentEffectMode;
            titleBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            scrollBar.GetComponent<Scrollbar>().colors = currentColorBlock;
        }
    }
}
