using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class FillableBox : MonoBehaviour
    {
        private RectTransform _fillableBoxTransform;
        [SerializeField] private GameObject scrollRectObject;
        [SerializeField] private GameObject productionBoxes;
        public List<GrammarScript.Production> productionList;
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
                newProductionBox.transform.position -= new Vector3(0, _productionBoxHeight * productionCounter, 0);
                // Change it's text
                newProductionBox.GetComponentInChildren<TextMeshProUGUI>().SetText(production._in + "→" + production._out);
                productionCounter++;
                
            }
        }
    }
}
