using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class AcceptLambdaQuestionBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI startVariableText;
        [SerializeField] private GameObject productionsBoxObject;
        public void SetStartVariable(char startVariable)
        {
            startVariableText.SetText(char.ToUpper(startVariable).ToString());
        }

        public void AddEmptyWordToProductions()
        {
            var lambdaProduction = new GrammarScript.Production(startVariableText.text.ToCharArray()[0], "λ");
            if (productionsBoxObject.GetComponent<ProductionsBox>().productionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) == null)
            {
                var productionsBoxComponent = productionsBoxObject.GetComponent<ProductionsBox>();
                productionsBoxComponent.InsertAndReconstructList(new GrammarScript.Production(startVariableText.text.ToCharArray()[0], "λ"), draggable: false, deletable: false, grayscale: false);
            }
        }

        public void RemoveEmptyWordFromProductions()
        {
            var lambdaProduction = new GrammarScript.Production(startVariableText.text.ToCharArray()[0], "λ");
            if (productionsBoxObject.GetComponent<ProductionsBox>().productionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) != null)
            {
                var lambdaProductionBox = productionsBoxObject.GetComponent<ProductionsBox>().productionBoxList
                    .Find(x => x.GetComponent<BoxContent>().Production._in == lambdaProduction._in &&
                               x.GetComponent<BoxContent>().Production._out == lambdaProduction._out);
                var productionsBoxComponent = productionsBoxObject.GetComponent<ProductionsBox>();
                ((FillableBox) productionsBoxComponent).RemoveAndReconstructList(lambdaProductionBox.gameObject, draggable: false, deletable: false, grayscale: false, destroy: true);
            }
        }
    }
}
