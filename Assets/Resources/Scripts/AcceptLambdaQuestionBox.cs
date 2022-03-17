using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class AcceptLambdaQuestionBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI StartVariableText;
        [SerializeField] private GameObject ProductionsBoxObject;
        public void SetStartVariable(char startVariable)
        {
            StartVariableText.SetText(char.ToUpper(startVariable).ToString());
        }

        public void AddEmptyWordToProductions()
        {
            var lambdaProduction = new GrammarScript.Production(StartVariableText.text.ToCharArray()[0], "λ");
            if (ProductionsBoxObject.GetComponent<ProductionsBox>().productionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) == null)
            {
                var productionsBoxComponent = ProductionsBoxObject.GetComponent<ProductionsBox>();
                productionsBoxComponent.InsertAndReconstructList(new GrammarScript.Production(StartVariableText.text.ToCharArray()[0], "λ"), draggable: false, deletable: false, grayscale: false);
                productionsBoxComponent.SetAllProductionsDeletability(false);
                productionsBoxComponent.SetGrayScale(true);
            }
        }

        public void RemoveEmptyWordForProductions()
        {
            var lambdaProduction = new GrammarScript.Production(StartVariableText.text.ToCharArray()[0], "λ");
            if (ProductionsBoxObject.GetComponent<ProductionsBox>().productionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) != null)
            {
                var lambdaProductionBox = ProductionsBoxObject.GetComponent<ProductionsBox>().productionBoxList
                    .Find(x => x.GetComponent<BoxContent>().Production._in == lambdaProduction._in &&
                               x.GetComponent<BoxContent>().Production._out == lambdaProduction._out);
                var productionsBoxComponent = ProductionsBoxObject.GetComponent<ProductionsBox>();
                ((FillableBox) productionsBoxComponent).RemoveAndReconstructList(lambdaProductionBox.gameObject, draggable: false, deletable: false, grayscale: false);
                productionsBoxComponent.SetAllProductionsDeletability(false);
                productionsBoxComponent.SetGrayScale(true);
            }
        }
    }
}
