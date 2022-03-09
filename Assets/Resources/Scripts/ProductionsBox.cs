using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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
        private Vector2 ProductionBoxesOriginalSize;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _productionBoxHeight = (productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y)* Utils.ScreenDif;
            ProductionBoxesOriginalSize = productionBoxes.GetComponent<RectTransform>().sizeDelta;
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
            if (productionBoxList.Count > 0 || productionList.Count > 0)
            {
                print("box list: " + productionBoxList.Count + "list: " + productionList.Count);
                return;
            }
            print("Numero de producoes que foram chamadas aqui: " + productions.Count);
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
        
        // public void AppendProduction(GrammarScript.Production production)
        // {
        //      var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
        //     productionBoxesRectTransform.sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);
        //
        //     // Instantiate a new production box
        //     var productionsBoxTransform = productionBoxes.transform;
        //     var productionsBoxPosition = productionsBoxTransform.position;
        //     var newProductionBox = Instantiate(productionBoxPrefab,
        //         new Vector3(productionsBoxPosition.x, productionsBoxPosition.y, productionsBoxPosition.z),
        //         Quaternion.identity, productionBoxes.transform);
        //     newProductionBox.GetComponent<Draggable>().CanBeDragged = false;
        //     newProductionBox.GetComponent<Draggable>().AttachedTo = productionBoxes;
        //     newProductionBox.GetComponent<BoxContent>().SetProduction(production);
        //     var newProductionTransform = newProductionBox.transform;
        //     newProductionTransform.position = _emptyPosition;
        //     // Set new original position
        //     newProductionBox.GetComponent<Draggable>().OriginalPosition = newProductionTransform.localPosition;
        //     newProductionBox.GetComponent<Draggable>().LastValidPosition =
        //         newProductionBox.GetComponent<Draggable>().OriginalPosition;
        //     // Change it's text
        //     newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
        //     AddToLists(newProductionBox.gameObject);
        //     _emptyPosition = newProductionTransform.position - new Vector3(0, _productionBoxHeight, 0);
        // }

        public override void RemoveFromLists(GameObject box)
        {
            if (!productionBoxList.Remove(box.transform))
            {
                print("Nâo conseguiu remover a caixa da producao: " 
                      + box.GetComponent<BoxContent>().Production._in + "->" 
                      + box.GetComponent<BoxContent>().Production._out);
            }
            
            if (!productionList.Remove(box.GetComponent<BoxContent>().Production))
            {
                print("Nâo conseguiu remover da lista a produção " 
                      + box.GetComponent<BoxContent>().Production._in + "->" 
                      + box.GetComponent<BoxContent>().Production._out);
            }
        }

        public override void AddToLists(GameObject box)
        {
            productionBoxList.Add(box.transform);
            productionList.Add(box.GetComponent<BoxContent>().Production);
            // printProductionList();
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

        public void RemoveProductionAndReconstructList(GameObject productionBoxToBeRemoved)
        {
            RemoveFromLists(productionBoxToBeRemoved);
            Destroy(productionBoxToBeRemoved.gameObject);
            var productionsListCopy = productionList.DeepClone();
           
            ClearList();
            FillWithProductions(productionsListCopy);
            SetAllProductionsDeletability(true);
        }
        
        
        public void InsertProductionAndReconstructList(GrammarScript.Production productionToBeInserted)
        {
            var productionsListCopy = productionList.DeepClone();
            productionsListCopy.Add(productionToBeInserted);
            ClearList();
            FillWithProductions(productionsListCopy);
            SetAllProductionsDeletability(true);
        }

        public override void ClearList()
        {
            while (productionBoxList.Count > 0)
            {
                var currentProductionBox = productionBoxList[0];
                RemoveFromLists(currentProductionBox.gameObject);
                Destroy(currentProductionBox.gameObject);
            }
            productionBoxes.GetComponent<RectTransform>().sizeDelta = ProductionBoxesOriginalSize;
        }

        public void SetAllProductionsDeletability(bool boolean)
        {
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDeleted = boolean;
            }
        }
        
        public void SetAllProductionsDraggability(bool boolean)
        {
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDragged = boolean;
            }
        }
    }
}
