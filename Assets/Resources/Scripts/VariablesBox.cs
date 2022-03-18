using System;
using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class VariablesBox : FillableBox
    {
        [SerializeField] private GameObject variableBoxes;
        public List<char> variableList;
        public List<Transform> variableBoxList;
        public Transform variableBoxPrefab;
        private float _variableBoxHeight;
        private Vector2 _variableBoxesOriginalSize;
        private char _lastVariableInserted;
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _variableBoxHeight = variableBoxPrefab.GetComponent<RectTransform>().sizeDelta.y * Utils.ScreenDif;
            _variableBoxesOriginalSize = variableBoxes.GetComponent<RectTransform>().sizeDelta;
        }

        public override void SetGrayScale(bool? option)
        {
            if (option == null) return;
            base.SetGrayScale(option);
            var currentEffectMode = (bool) option ? EffectMode.Grayscale : EffectMode.None;

            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            }

        }

        public void FillWithVariables(IEnumerable<char> variables)
        {
            if (variableBoxList.Count > 0 || variableList.Count > 0)
            {
                print("box list: " + variableBoxList.Count + "list: " + variableList.Count);
                return;
            }
            var variableBoxesRectTransform = variableBoxes.GetComponent<RectTransform>();
            var variableCounter = 0;
            foreach (var variable in variables)
            {
                // Expanding boxes container
                variableBoxesRectTransform.sizeDelta += new Vector2(0, _variableBoxHeight / Utils.ScreenDif);
                // Instantiate a new production box
                var variablesBoxTransform = variableBoxes.transform;
                var variablesBoxPosition = variablesBoxTransform.position;
                var newVariableBox = Instantiate(variableBoxPrefab,
                    variablesBoxPosition, Quaternion.identity, variableBoxes.transform);
                newVariableBox.GetComponent<Draggable>().CanBeDragged = false;
                newVariableBox.GetComponent<Draggable>().AttachedTo = variableBoxes;
                newVariableBox.GetComponent<BoxContent>().SetVariable(variable);
                var newVariableTransform = newVariableBox.transform;
                var newVariablePosition = variablesBoxPosition;
                newVariablePosition -= new Vector3(0, _variableBoxHeight * variableCounter, 0);
                newVariableTransform.position = newVariablePosition;
                // Change it's text
                newVariableBox.GetComponentInChildren<TextMeshProUGUI>().SetText(variable.ToString());
                AddToLists(newVariableBox.gameObject);
                _lastVariableInserted = variable;
                variableCounter++;
            }
            SetGrayScale(false);
        }

        public override void RemoveFromLists(GameObject box)
        {
            variableBoxList.Remove(box.transform);
            variableList.Remove(box.GetComponent<BoxContent>().Variable);
        }

        public override void AddToLists(GameObject box)
        {
            variableBoxList.Add(box.transform);
            variableList.Add(box.GetComponent<BoxContent>().Variable);
        }
        
        public override void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            if (eventData.pointerDrag.CompareTag("Variable"))
            {
                eventData.pointerDrag.GetComponent<Draggable>().IsOnValidPositionToDrop = true;
                print("É VARIÁVEL E FOI DROPADA DENTRO DA CAIXA " + name);
                InsertAndReconstructList(eventData.pointerDrag.GetComponent<BoxContent>().Variable, draggable: true, deletable: false, grayscale: false);
                Destroy(eventData.pointerDrag);
            }
        }
        
        public override void ClearList()
        {
            while (variableBoxList.Count > 0)
            {
                var currentProductionBox = variableBoxList[0];
                RemoveFromLists(currentProductionBox.gameObject);
                Destroy(currentProductionBox.gameObject);
            }
            variableBoxes.GetComponent<RectTransform>().sizeDelta = _variableBoxesOriginalSize;
        }

        public override void InsertAndReconstructList(GrammarScript.Production productionToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            throw new NotImplementedException();
        }

        public override void InsertAndReconstructList(char variableToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            var variablesListCopy = variableList.DeepClone();
            variablesListCopy.Add(variableToBeInserted);
            ClearList();
            FillWithVariables(variablesListCopy);
            SetAllVariablesDeletability(deletable);
            SetAllVariablesDraggability(draggable);
            SetGrayScale(grayscale);
        }
        
        public override void RemoveAndReconstructList(GameObject variableBoxToBeRemoved, bool? draggable, bool? deletable, bool? grayscale, bool destroy)
        {
            RemoveFromLists(variableBoxToBeRemoved);
            if(destroy)
                Destroy(variableBoxToBeRemoved.gameObject);
            var variableListCopy = variableList.DeepClone();
           
            ClearList();
            FillWithVariables(variableListCopy);
            SetAllVariablesDeletability(deletable);
            SetAllVariablesDraggability(draggable);
        }
        
        public void SetAllVariablesDeletability(bool? boolean)
        {
            if (boolean == null) return; 
            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<Draggable>().CanBeDeleted = (bool) boolean;
            }
        }
        
        public void SetAllVariablesDraggability(bool? boolean)
        {
            if (boolean == null) return; 
            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<Draggable>().CanBeDragged = (bool)  boolean;
            }
        }

    }
}
