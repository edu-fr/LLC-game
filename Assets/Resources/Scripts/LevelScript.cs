using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class LevelScript : MonoBehaviour
    {
        public GameObject grammarObject;
        private GrammarScript _grammar;
        private List<char> _lambdaProducers = new List<char>();

        // ReSharper disable once InconsistentNaming
        [SerializeField] private GameObject _CFL_Box;
        [SerializeField] private GameObject variablesBox;
        [SerializeField] private GameObject usefulVariablesBox;
        [SerializeField] private GameObject uselessVariablesBox;
        private int currentPhase;
        private int currentPart;
        private void Awake()
        {
            
        }
        
        private void Start()
        {
            // Get Current grammar
            _grammar = grammarObject.GetComponent<GrammarScript>();
            // Analyse productions
            _grammar.Setup();
            _CFL_Box.GetComponent<ProductionsBox>().FillWithProductions(_grammar.Productions.ToList());
            _CFL_Box.GetComponent<ProductionsBox>().SetGrayScale(true);
            variablesBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.Variables);

            currentPhase = 1;
            currentPart = 1;

        }

        public void TryNextPhase()
        {
            switch (currentPhase)
            {
                case 1:

                    switch (currentPart)
                    {
                        case 1:
                            Phase1Part1();
                            break;
                        
                        case 2:

                            break;
                    }
                    
                    break;
                
                case 2:

                    break;
                
            }
        }

        private void Phase1Part1()
        {
            var currentVariablesOnVariablesBox = variablesBox.GetComponent<VariablesBox>().variableList;
            if (currentVariablesOnVariablesBox.Count > 0)
            {
                print("Ainda há variáveis que devem ser movidas!");
                return;
            }
            
            var currentVariablesOnUsefulBox = usefulVariablesBox.GetComponent<VariablesBox>().variableList;
            var currentVariablesOnUselessBox = uselessVariablesBox.GetComponent<VariablesBox>().variableList;
            var correctVariables = _grammar.UsefulVariables;

            if (correctVariables.Count != currentVariablesOnUsefulBox.Count) print("ERRADO PELO NUM DE ELEMENTOS!");
            else
            {
                foreach (var variable in currentVariablesOnUsefulBox)
                {
                    if (!correctVariables.Contains(variable))
                    {
                        print("A variavel " + variable + " não faz parte da lista de variáveis corretas!");
                        return;
                    } 
                }
                print("Correto! Pode prosseguir para a próxima fase!");
            }
        }
        private void Update()
        {
           

        }
    }
}
