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
        public Button resetButton;
        public CanvasController canvasController;
        public LevelScript levelScript;
        public bool acceptDrop = true;
        public bool currentDraggability;
        public bool currentDeletability;
        
        protected override void Awake()
        {
            base.Awake();
            _productionBoxHeight = productionBoxPrefab.GetComponent<RectTransform>().sizeDelta.y * Utils.ScreenDif;
            _productionBoxesOriginalSize = productionBoxes.GetComponent<RectTransform>().sizeDelta;
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void SetGrayScale(bool? option)
        {
            if (option == null) return;
            base.SetGrayScale(option);
            var currentEffectMode = (bool) option ? EffectMode.Grayscale : EffectMode.None;
            var currentColorFactor = (bool) option ? 0 : 1;
           
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
                productionBox.GetComponent<UIEffect>().colorFactor = currentColorFactor;
            }
        }
        
        public void FillWithProductions(List<GrammarScript.Production> productions)
        {
            if (productionBoxList.Count > 0 || productionList.Count > 0)
            {
                ClearList();
            }
            var productionBoxesRectTransform = productionBoxes.GetComponent<RectTransform>();
            print("GAME OBJECT" + productionBoxesRectTransform.gameObject);
            var productionCounter = 0;
            productions.Sort(ExtensionMethods.SortProductions);
            foreach (var production in productions)
            {
                // Expanding boxes container
                productionBoxes.GetComponent<RectTransform>().sizeDelta += new Vector2(0, _productionBoxHeight / Utils.ScreenDif);
                print("TAMANHO: " + productionBoxesRectTransform.sizeDelta.x + "/" + productionBoxesRectTransform.sizeDelta.y);
                print("PRODUCTION BOX HEIGHT! :" + _productionBoxHeight);
                // Instantiate a new production box
                var productionsBoxTransform = productionBoxes.transform;
                var productionsBoxPosition = productionsBoxTransform.position;
                var newProductionBox = Instantiate(productionBoxPrefab,
                    productionsBoxPosition, Quaternion.identity, productionBoxes.transform);
                // newProductionBox.GetComponent<Draggable>().CanBeDragged = false;
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
            if (!acceptDrop) return;
            
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
            currentDeletability = (bool) boolean;
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDeleted = (bool) boolean;
            }
            print("DELETABILITY SETADA PARA " + (bool) boolean);
        }
        
        public void SetAllProductionsDraggability(bool? boolean)
        {
            if (boolean == null) return;
            currentDraggability = (bool) boolean;
            foreach (var productionBox in productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDragged = (bool) boolean;
            }
            print("DRAGGABILITY SETADA PARA " + (bool) boolean);
        }

        public void ResetButtonPress()
        {
            GETCurrentResetModal().SetActive(true);
        }

        private GameObject GETCurrentResetModal()
        {
            switch (levelScript.currentPhase)
            {
                case 1:
                    switch (levelScript.currentPart)
                    {
                        case 2:
                            return canvasController.resetProductionBox_f1p2;
                        
                        case 3:
                            return canvasController.resetProductionBox_f1p3;
                        
                        default:
                            print("Solicitação de reset modal incorreta! Parte errada!");
                            break;
                    }
                    break;
                
                case 2:
                    switch (levelScript.currentPart)
                    {
                        case 2:
                            return canvasController.resetProductionBox_f2p2;
                        
                        default:
                            print("Solicitação de reset modal incorreta! Parte errada!");
                            break;
                    }
                    break;
                
                case 3:
                    switch (levelScript.currentPart)
                    {
                        case 1:
                            return canvasController.resetProductionBox_f3p1;
                        
                        case 3:
                            return canvasController.resetProductionBox_f3p3;
                        
                        default:
                            print("Solicitação de reset modal incorreta! Parte errada!");
                            break;
                    }
                    break;
                
                case 4:
                    switch (levelScript.currentPart)
                    {
                        case 2:
                            return canvasController.resetProductionBox_f4p2;
                        
                        case 3:
                            return canvasController.resetProductionBox_f4p3;
                        
                        default:
                            print("Solicitação de reset modal incorreta! Parte errada!");
                            break;
                    }
                    break;
                
                default:
                    print("Solicitação de reset modal incorreta! Phase errada!");
                    break;
            }
            return null;
        }
        
        
        public void ResetToOriginalList()
        {
            FillWithProductions(originalProductionList);
            SetAllProductionsDeletability(currentDeletability);
            SetAllProductionsDraggability(currentDraggability);
        }
    }
}
