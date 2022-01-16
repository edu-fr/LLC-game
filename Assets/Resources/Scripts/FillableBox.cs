using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class FillableBox : MonoBehaviour
    {
        private RectTransform _fillableBoxTransform;
        [SerializeField] private GameObject titleBox;
        [SerializeField] private GameObject productionBoxes;
        [SerializeField] private GameObject scrollBar;
        private ColorBlock _scrollBarDefaultColorBlock;
        public List<GrammarScript.Production> productionList;
        public List<Transform> productionBoxList;
        public Transform productionBoxPrefab;   

        // fillable box attributes
        [SerializeField] private float titleBoxOffset;
        
        // production box attributes
        private float _productionBoxHeight;
        private void Awake()
        {
            _fillableBoxTransform = GetComponent<RectTransform>();
        }
    
        private void Start()
        {
            productionList = new List<GrammarScript.Production>();
            _productionBoxHeight = productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y;
            _scrollBarDefaultColorBlock = scrollBar.GetComponent<Scrollbar>().colors;
        }
        
        public void FillWithProductions(List<GrammarScript.Production> productions)
        {
            var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            var productionCounter = 0;
            foreach (var production in productions)
            {
                // Add production to the box list
                productionList.Add(production);
                // Increase the scroll rect size
                productionBoxesRectTransform.sizeDelta += new Vector2(0, 60);
                // Instantiate a new production box
                var productionsBoxTransform = productionBoxes.transform;
                var productionsBoxPosition = productionsBoxTransform.position;
                var newProductionBox = Instantiate(productionBoxPrefab, new Vector3(productionsBoxPosition.x, productionsBoxPosition.y, productionsBoxPosition.z), Quaternion.identity, productionBoxes.transform);
                productionBoxList.Add(newProductionBox);
                var newProductionTransform = newProductionBox.transform;
                var newProductionPosition = newProductionTransform.position;
                newProductionPosition -= new Vector3(0, _productionBoxHeight * productionCounter, 0);
                newProductionTransform.position = newProductionPosition;
                // Set new original position
                newProductionBox.GetComponent<Draggable>().OriginalPosition = newProductionTransform.localPosition;
                // Change it's text
                newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
                productionCounter++;
            }
            SetGrayScale(false);
        }

        private void SetGrayScale(bool option)
        {
            var currentEffectMode = option ? EffectMode.Grayscale : EffectMode.None;
            var currentColorBlock = option ? ColorBlock.defaultColorBlock : _scrollBarDefaultColorBlock;

            gameObject.GetComponent<UIEffect>().effectMode = currentEffectMode;
            titleBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            }

            scrollBar.GetComponent<Scrollbar>().colors = currentColorBlock;

        }
    }
}
