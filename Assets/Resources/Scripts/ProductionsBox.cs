using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private GrammarScript.Production _lastProductionInserted;
        private Vector3 _emptyPosition = Vector3.zero;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _productionBoxHeight = (productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y)* Utils.ScreenDif;
        }

        public override void SetGrayScale(bool option)
        {
            base.SetGrayScale(option);
            var currentEffectMode = option ? EffectMode.Grayscale : EffectMode.None;

            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            }
        }
        
        public void FillWithProductions(List<GrammarScript.Production> productions)
        {
            var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            var productionCounter = 0;
            foreach (var production in productions)
            {
                // Expanding boxes container
                productionBoxesRectTransform.sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);

                // Instantiate a new production box
                var productionsBoxTransform = productionBoxes.transform;
                var productionsBoxPosition = productionsBoxTransform.position;
                var newProductionBox = Instantiate(productionBoxPrefab,
                    new Vector3(productionsBoxPosition.x, productionsBoxPosition.y, productionsBoxPosition.z),
                    Quaternion.identity, productionBoxes.transform);
                newProductionBox.GetComponent<Draggable>().CanBeDragged = false;
                newProductionBox.GetComponent<Draggable>().AttachedTo = productionBoxes;
                newProductionBox.GetComponent<BoxContent>().SetProduction(production);
                var newProductionTransform = newProductionBox.transform;
                var newProductionPosition = newProductionTransform.position;
                newProductionPosition -= new Vector3(0, _productionBoxHeight * productionCounter, 0);
                newProductionTransform.position = newProductionPosition;
                _emptyPosition = newProductionPosition - new Vector3(0, _productionBoxHeight, 0);
                // Set new original position
                newProductionBox.GetComponent<Draggable>().OriginalPosition = newProductionTransform.localPosition;
                newProductionBox.GetComponent<Draggable>().LastValidPosition =
                    newProductionBox.GetComponent<Draggable>().OriginalPosition;
                // Change it's text
                newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
                AddToLists(newProductionBox.gameObject);
                _lastProductionInserted = production;
                productionCounter++;
            }
            SetGrayScale(false);
        }
        
        public void AppendProduction(GrammarScript.Production production)
        {
             var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            productionBoxesRectTransform.sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);

            // Instantiate a new production box
            var productionsBoxTransform = productionBoxes.transform;
            var productionsBoxPosition = productionsBoxTransform.position;
            var newProductionBox = Instantiate(productionBoxPrefab,
                new Vector3(productionsBoxPosition.x, productionsBoxPosition.y, productionsBoxPosition.z),
                Quaternion.identity, productionBoxes.transform);
            newProductionBox.GetComponent<Draggable>().CanBeDragged = false;
            newProductionBox.GetComponent<Draggable>().AttachedTo = productionBoxes;
            newProductionBox.GetComponent<BoxContent>().SetProduction(production);
            var newProductionTransform = newProductionBox.transform;
            newProductionTransform.position = _emptyPosition;
            // Set new original position
            newProductionBox.GetComponent<Draggable>().OriginalPosition = newProductionTransform.localPosition;
            newProductionBox.GetComponent<Draggable>().LastValidPosition =
                newProductionBox.GetComponent<Draggable>().OriginalPosition;
            // Change it's text
            newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
            AddToLists(newProductionBox.gameObject);
            _emptyPosition = newProductionTransform.position - new Vector3(0, _productionBoxHeight, 0);
        }
        
        public void ScrollbarTop()
        {
            var bar = GetComponentInChildren<Scrollbar>();
        }

        
        public override void RemoveFromLists(GameObject box)
        {
            productionBoxList.Remove(box.transform);
            productionList.Remove(box.GetComponent<BoxContent>().Production);
        }

        public override void AddToLists(GameObject box)
        {
            productionBoxList.Add(box.transform);
            productionList.Add(box.GetComponent<BoxContent>().Production);
            printProductionList();
        }

        public void printProductionList()
        {
            foreach (var production in productionList)
            {
                print(production._in + "->" + production._out);
            }
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (!eventData.pointerDrag) return;
            

        }
    }
}
