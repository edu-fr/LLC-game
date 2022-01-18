using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class ProductionsBox : FillableBox
    {
        [SerializeField] private GameObject productionBoxes;
        public List<GrammarScript.Production> productionList;
        public List<Transform> productionBoxList;
        public Transform productionBoxPrefab;
        private float _productionBoxHeight;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _productionBoxHeight = productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y * Utils.ScreenDif;
        }

        protected override void SetGrayScale(bool option)
        {
            base.SetGrayScale(option);
            

        }

        public void Fill(List<GrammarScript.Production> productions)
        {
            var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            var productionCounter = 0;
            foreach (var production in productions)
            {
                // Add production to the box list
                productionList.Add(production);
                // Increase the scroll rect size
                productionBoxesRectTransform.sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);
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

    }
}
