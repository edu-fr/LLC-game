using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class LevelScript : MonoBehaviour
    {
        public GameObject grammarObject;
        private GrammarScript _grammar;
        private List<char> _lambdaProducers = new List<char>();

        // ReSharper disable once InconsistentNaming
        [SerializeField] private GameObject CFL_Box;
        [SerializeField] private GameObject variablesBox;
        [SerializeField] private GameObject usefulVariablesBox;
        [SerializeField] private GameObject uselessVariablesBox;
        [SerializeField] private GameObject trashBin;
        private int _currentPhase;
        private int _currentPart;
        
        // Box positions
        private BoxPositionsManager _boxPositionsManager;

        private void Awake()
        {
            _boxPositionsManager = GetComponent<BoxPositionsManager>();
           
        }
        
        private void Start()
        {
            // Get Current grammar
            _grammar = grammarObject.GetComponent<GrammarScript>();
            // Analyse productions
            _grammar.Setup();
            
            // Set initial phase and part
            _currentPhase = 1;
            _currentPart = 1;
            
            SetupPhase1Part1();
            
            // DEBUG
            
            SetupPhase1Part2();
            _currentPart = 2; 
        }

        public void TryNextPhase()
        {
            switch (_currentPhase)
            {
                case 1:

                    switch (_currentPart)
                    {
                        case 1:
                            if (Phase1Part1())
                            {
                                _currentPart = 2; 
                                SetupPhase1Part2();
                            };
                            break;
                        
                        case 2:
                            if (Phase1Part2())
                            {
                                // currentPart = 3;
                                // SetupPhase1Part3();
                            }
                            break;
                    }
                    
                    break;
                
                case 2:

                    break;
                
            }
        }

        private bool Phase1Part1()
        {
            var currentVariablesOnVariablesBox = variablesBox.GetComponent<VariablesBox>().variableList;
            if (currentVariablesOnVariablesBox.Count > 0)
            {
                print("Ainda há variáveis que devem ser movidas!");
                return false;
            }
            
            var currentVariablesOnUsefulBox = usefulVariablesBox.GetComponent<VariablesBox>().variableList;
            var currentVariablesOnUselessBox = uselessVariablesBox.GetComponent<VariablesBox>().variableList;
            var correctVariables = _grammar.UsefulVariablesPhase1;

            if (correctVariables.Count != currentVariablesOnUsefulBox.Count) print("ERRADO PELO NUM DE ELEMENTOS!");
            else
            {
                foreach (var variable in currentVariablesOnUsefulBox)
                {
                    if (!correctVariables.Contains(variable))
                    {
                        print("A variavel " + variable + " não faz parte da lista de variáveis corretas!");
                        return false;
                    } 
                }
                print("Correto! Pode prosseguir para a próxima fase!");
                return true;
            }
            return false;
        }

        
        private void SetupPhase1Part1()
        {
            print("Setting up phase 1 part 1");
            // Filling boxes
            CFL_Box.GetComponent<ProductionsBox>().FillWithProductions(_grammar.Productions.ToList());
            variablesBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.Variables);
            
            // Changing productions box color
            CFL_Box.GetComponent<ProductionsBox>().SetGrayScale(true);

            // Setting positions
            CFL_Box.transform.position = _boxPositionsManager.Anchor_Phase1Part1_CLF_Box_gray.position;
            variablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_variablesBox.position;
            uselessVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_uselessVariablesBox.position;
            usefulVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_usefulVariablesBox.position;
        }
        
        private void SetupPhase1Part2()
        {
            print("Setting up phase 1 part 2");
            
            // Deactivating unused boxes
            variablesBox.SetActive(false);
            usefulVariablesBox.SetActive(false);
            
            // Activating necessary objects
            trashBin.SetActive(true);

            // Setting positions
            CFL_Box.transform.position = _boxPositionsManager.Anchor_Phase1Part2_CLF_Box.position;
            uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part2_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_TrashBin.position;
            
            // Changing box colors
            CFL_Box.GetComponent<ProductionsBox>().SetGrayScale(false);
            uselessVariablesBox.GetComponent<VariablesBox>().SetGrayScale(true);
            
            // Making CFL_Box objects draggable again
            foreach (var productionBox in CFL_Box.GetComponent<ProductionsBox>().productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDragged = true;
            }
            
        }

        private bool Phase1Part2()
        {
            // var currentProductionsOnProductionsBox = CFL_Box.GetComponent<ProductionsBox>().productionList;
            // if (currentProductionsOnProductionsBox.Count < 1)
            // {
            //     print("É necessário que alguma produção exista na linguagem");
            //     return false;
            // }
            //
            // var correctProductions = _grammar.usefulAndReachableProductionsPhase1;
            //
            // if (correctProductions.Count != currentProductionsOnProductionsBox.Count) print("Número errado de produções úteis!");
            // else
            // {
            //     foreach (var variable in currentVariablesOnUsefulBox)
            //     {
            //         if (!correctProductions.Contains(variable))
            //         {
            //             print("A variavel " + variable + " não faz parte da lista de variáveis corretas!");
            //             return false;
            //         } 
            //     }
            //     print("Correto! Pode prosseguir para a próxima fase!");
            //     return true;
            // }
            return false;
        }
        private void Update()
        {
           

        }
    }
}
