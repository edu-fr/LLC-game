using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class ProductionMaker : MonoBehaviour
    {
        public GameObject levelController;
        private GrammarScript _currentGrammar;
        public GameObject productionTextFieldObject;
        public GameObject variableTextFieldObject;
        public GameObject productionTextObject; 
        public GameObject variableTextObject;
        private TextMeshProUGUI _productionText; 
        private TextMeshProUGUI _variableText;
        
        public void Start()
        {
            _currentGrammar = levelController.GetComponent<LevelScript>().grammarObject.GetComponent<GrammarScript>();
            _productionText = productionTextObject.GetComponent<TextMeshProUGUI>();
            _variableText = variableTextObject.GetComponent<TextMeshProUGUI>();
        }
        
        public void CreateProduction()
        {
            print(_variableText.text);
            print(_productionText.text);
            if (!_currentGrammar.Variables.Contains(_variableText.text[0]))
            {
                print("ERRO! Variável inválida!");
                return;
            }

            for (var i = 0; i < _productionText.text.Length - 1; i++)
            {
                print("Tentando char: " + _productionText.text[i]);
                if (char.IsNumber(_productionText.text[i]))
                {
                    print("Numeros nao sao permitidos!");
                    return;
                }
                
                if (_productionText.text[i] is '\n' or ' ' or '\0' or '/')
                {
                    print("VAZIO!");
                    continue;
                }
                if (!_currentGrammar.Variables.Contains(_productionText.text[i]) && !_currentGrammar.Terminals.Contains(_productionText.text[i]))
                {
                    print("Produção inválida! Não contem o char: " + _productionText.text[i]);
                    return;
                }
            }

            var newProduction = new GrammarScript.Production(_variableText.text.ToCharArray()[0], _productionText.text);

            var productionsBox = levelController.GetComponent<LevelScript>().p2_productionsBox
                .GetComponent<ProductionsBox>();

            var alreadyExists = false;
            foreach (var production in productionsBox.productionList)
            {
                if (production._in != newProduction._in || production._out != newProduction._out)
                    continue;
                alreadyExists = true;
                break;
            }

            if (!alreadyExists)
            {
                productionsBox.AppendProduction(newProduction);
                productionTextFieldObject.GetComponent<TMP_InputField>().text = "";
                variableTextFieldObject.GetComponent<TMP_InputField>().text = "";
            }
            else
                print("Produção repetida!");

        }
    }
}
