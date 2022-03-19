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
        public List<GrammarScript.Production> originalProductionList;
        public List<Transform> productionBoxList;
        public Transform productionBoxPrefab;
        private float _productionBoxHeight;
        private GrammarScript.Production _lastProductionInserted;
        private Vector2 _productionBoxesOriginalSize;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _productionBoxHeight = (productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y)* Utils.ScreenDif;
            _productionBoxesOriginalSize = productionBoxes.GetComponent<RectTransform>().sizeDelta;
        }

        public override void SetGrayScale(bool? option)
        {
            if (option == null) return;
            base.SetGrayScale(option);
            var currentEffectMode = (bool) option ? EffectMode.Grayscale : EffectMode.None;

            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            }
        }
        
        public void FillWithProductions(List<GrammarScript.Production> productions)
        {
            if (productionBoxList.Count > 0 || productionList.Count > 0)
            {
                ClearList();
            }
            var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            var productionCounter = 0;
            productions.Sort(ExtensionMethods.SortProductions);
            foreach (var production in productions)
            {
                // Expanding boxes container
                productionBoxesRectTransform.sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);
                // Instantiate a new production box
                var productionsBoxTransform = productionBoxes.transform;
                var productionsBoxPosition = productionsBoxTransform.position;
                var newProductionBox = Instantiate(productionBoxPrefab,
                    productionsBoxPosition, Quaternion.identity, productionBoxes.transform);
                newProductionBox.GetComponent<Draggable>().CanBeDragged = false;
                newProductionBox.GetComponent<Draggable>().AttachedTo = productionBoxes;
                newProductionBox.GetComponent<BoxContent>().SetProduction(production);
                var newProductionTransform = newProductionBox.transform;
                var newProductionPosition = productionsBoxPosition;
                newProductionPosition -= new Vector3(0, _productionBoxHeight * productionCounter, 0);
                newProductionTransform.position = newProductionPosition;
                // Change it's text
                newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
                AddToLists(newProductionBox.gameObject);
                _lastProductionInserted = production;
                productionCounter++;
            }
            SetGrayScale(false);
        }
        
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
            if (eventData.pointerDrag == null) return;
            if (eventData.pointerDrag.CompareTag("Production") && eventData.pointerDrag.GetComponent<Draggable>().CanBeDragged)
            {
                eventData.pointerDrag.GetComponent<Draggable>().IsOnValidPositionToDrop = true;
                InsertAndReconstructList(eventData.pointerDrag.GetComponent<BoxContent>().Production, draggable: eventData.pointerDrag.GetComponent<Draggable>().CanBeDragged, deletable:  eventData.pointerDrag.GetComponent<Draggable>().CanBeDeleted, grayscale: false);
                Destroy(eventData.pointerDrag);
            }
        }
        
        public override void ClearList()
        {
            while (productionBoxList.Count > 0)
            {
                var currentProductionBox = productionBoxList[0];
                RemoveFromLists(currentProductionBox.gameObject);
                Destroy(currentProductionBox.gameObject);
            }
            productionBoxes.GetComponent<RectTransform>().sizeDelta = _productionBoxesOriginalSize;
        }
       
        public override void InsertAndReconstructList(GrammarScript.Production productionToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            var productionsListCopy = productionList.DeepClone();
            productionsListCopy.Add(productionToBeInserted);
            ClearList();
            productionsListCopy.Sort(ExtensionMethods.SortProductions);
            FillWithProductions(productionsListCopy);
            SetAllProductionsDraggability(draggable);
            SetAllProductionsDeletability(deletable);
            SetGrayScale(grayscale);
        }
        
        public override void RemoveAndReconstructList(GameObject productionBoxToBeRemoved, bool? draggable, bool? deletable, bool? grayscale, bool destroy)
        {
            RemoveFromLists(productionBoxToBeRemoved);
            if(destroy)
                Destroy(productionBoxToBeRemoved.gameObject);
            var productionsListCopy = productionList.DeepClone();
            ClearList();
            productionsListCopy.Sort(ExtensionMethods.SortProductions);
            FillWithProductions(productionsListCopy);
            SetAllProductionsDeletability(deletable);
            SetAllProductionsDraggability(draggable);
        }
        

        public override void InsertAndReconstructList(char variableToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            throw new NotImplementedException();
        }

        public void SetAllProductionsDeletability(bool? boolean)
        {
            if (boolean == null) return;
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDeleted = (bool) boolean;
            }
            print("DELETABILITY SETADA PARA " + (bool) boolean);
        }
        
        public void SetAllProductionsDraggability(bool? boolean)
        {
            if (boolean == null) return; 
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDragged = (bool) boolean;
            }
            print("DRAGGABILITY SETADA PARA " + (bool) boolean);
        }
    }
}
