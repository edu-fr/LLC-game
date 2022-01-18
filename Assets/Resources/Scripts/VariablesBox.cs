using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
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
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _variableBoxHeight = variableBoxPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        protected override void SetGrayScale(bool option)
        {
            base.SetGrayScale(option);
            

        }
        
        public void Fill(IEnumerable<char> variables)
        {
            var variableBoxesRectTransform = variableBoxes.GetComponent<RectTransform>();
            var variableCounter = 0;
            foreach (var variable in variables)
            {
                // Add production to the box list
                variableList.Add(variable);
                // Increase the scroll rect size
                variableBoxesRectTransform.sizeDelta += new Vector2(0, _variableBoxHeight);
                // Instantiate a new production box
                var variablesBoxTransform = variableBoxes.transform;
                var variablesBoxPosition = variablesBoxTransform.position;
                var newVariableBox = Instantiate(variableBoxPrefab, new Vector3(variablesBoxPosition.x, variablesBoxPosition.y, variablesBoxPosition.z), Quaternion.identity, variableBoxes.transform);
                variableBoxList.Add(newVariableBox);
                var newVariableTransform = newVariableBox.transform;
                var newVariablePosition = newVariableTransform.position;
                newVariablePosition -= new Vector3(0, _variableBoxHeight * variableCounter, 0);
                newVariableTransform.position = newVariablePosition;
                // Set new original position
                newVariableBox.GetComponent<Draggable>().OriginalPosition = newVariableTransform.localPosition;
                // Change it's text
                newVariableBox.GetComponentInChildren<TextMeshProUGUI>().SetText(variable.ToString());
                variableCounter++;
            }
            SetGrayScale(false);
        }

    }
}
