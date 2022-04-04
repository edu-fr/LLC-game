using System;
using System.Linq;
using System.Text;
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
            if (_variableText.text.Length < 2)
            {
                levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Nâo é possível criar uma produção sem uma variável na esquerda.", CanvasController.HelpWindowType.Attention);
                return;
            }

            if (_productionText.text.Length < 2)
            {
                levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Nâo é possível criar uma produção sem conteúdo no lado direito.", CanvasController.HelpWindowType.Attention);
                return;
            }
            
            if (!_currentGrammar.Variables.Contains(_variableText.text[0]))
            {
                levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Variável existe na linguagem.", CanvasController.HelpWindowType.Attention);

                return;
            }

            for (var i = 0; i < _productionText.text.Length - 1; i++)
            {
                if (char.IsNumber(_productionText.text[i]))
                {
                    levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Números não são permitidos.", CanvasController.HelpWindowType.Attention);

                    return;
                }
                
                if (_productionText.text[i] is '\n' or ' ' or '\0' or '/')
                {
                    levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Vazio não permitido.", CanvasController.HelpWindowType.Attention);

                    continue;
                }
                if (!_currentGrammar.Variables.Contains(_productionText.text[i]) && !_currentGrammar.Terminals.Contains(_productionText.text[i]))
                {
                    levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "Produção inválida! A linguagem não produz " + _productionText.text[i] + ".", CanvasController.HelpWindowType.Attention);
                    return;
                }
            }

            var correctString = new StringBuilder();
            for (int i = 0; i < _productionText.text.Length - 1; i++)
            {
                correctString.Append(_productionText.text[i]);
            }

            var newProduction = new GrammarScript.Production(_variableText.text.ToCharArray()[0], correctString.ToString());

            var productionsBox = levelController.GetComponent<LevelScript>().p2_productionsBox.GetComponent<ProductionsBox>();

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
                productionsBox.InsertAndReconstructList(newProduction, true, true, false);
                SoundManager.instance.PlayOneShot("insert-list");
                productionTextFieldObject.GetComponent<TMP_InputField>().text = "";
                variableTextFieldObject.GetComponent<TMP_InputField>().text = "";
            }
            else
                levelController.GetComponent<LevelScript>().canvasController.ConfigureAndCallHelpModal("Erro", "A produção já está na lista de produções.", CanvasController.HelpWindowType.Attention);

        }
    }
}
