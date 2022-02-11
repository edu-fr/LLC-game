using System.Linq;
using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class ProductionMaker : MonoBehaviour
    {
        public GameObject levelController;
        private GrammarScript _currentGrammar;
        public GameObject productionTextObject; 
        public GameObject variableTextObject;
        private TextMeshProUGUI _productionText; 
        private TextMeshProUGUI _variableText;
        public GrammarScript.Production production;

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

            foreach (var character in _productionText.text)
            {
                print("Tentando char: " + char.IsWhiteSpace(character));
                if (character == ' ')
                {
                    print("VAZIO!");
                    continue;
                }
                if (!_currentGrammar.Variables.Contains(character) && !_currentGrammar.Terminals.Contains(character))
                {
                    print("Produção inválida! Não contem o char: " + character);
                    return;
                }
            }

            production = new GrammarScript.Production(_variableText.text.ToCharArray()[0], _productionText.text);
            levelController.GetComponent<LevelScript>().p2_productionsBox.GetComponent<ProductionsBox>().AppendProduction(production);
            _productionText.text = "";
            _variableText.text = "";
        }
    }
}
