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
                print("Ainda nao contem! vou adicionar!");
                ProductionsBoxObject.GetComponent<ProductionsBox>().InsertProductionAndReconstructList(new GrammarScript.Production(StartVariableText.text.ToCharArray()[0], "λ"));
            }
            else
            {
                print("Já contem, nao vou adicionar de novo!");
            }
        }

        public void RemoveEmptyWordForProductions()
        {
            var lambdaProduction = new GrammarScript.Production(StartVariableText.text.ToCharArray()[0], "λ");
            if (ProductionsBoxObject.GetComponent<ProductionsBox>().productionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) != null)
            {
                print("CONTEM, vou retirar!!!");
                var lambdaProductionBox = ProductionsBoxObject.GetComponent<ProductionsBox>().productionBoxList
                    .Find(x => x.GetComponent<BoxContent>().Production._in == lambdaProduction._in &&
                               x.GetComponent<BoxContent>().Production._out == lambdaProduction._out);
                ProductionsBoxObject.GetComponent<ProductionsBox>().RemoveProductionAndReconstructList(lambdaProductionBox.gameObject);
            }
            else
            {
                print("NÃO CONTEM, entao nao da pra retirar!!!");
            }
            
        }
    }
}
