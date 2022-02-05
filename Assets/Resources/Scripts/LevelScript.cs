using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LevelScript : MonoBehaviour
    {
        public GameObject grammarObject;
        private GrammarScript _grammar;

        [SerializeField] private GameObject p1_productionsBox;
        [SerializeField] private GameObject p1_variablesBox;
        [SerializeField] private GameObject p1_usefulVariablesBox;
        [SerializeField] private GameObject p1_uselessVariablesBox;
        [SerializeField] private GameObject p2_variablesBox;
        [SerializeField] private GameObject p2_lambdaProducersBox;
        [SerializeField] private GameObject p2_productionsBox;
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
            //
            // SetupPhase1Part2();
            // _currentPart = 2; 
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
                                _currentPart = 3;
                                SetupPhase1Part3();
                            }
                            break;
                        
                        case 3:
                            if (Phase1Part3())
                            {
                                _currentPart = 1;
                                _currentPhase = 2;
                                SetupPhase2Part1();
                            }
                            break;
                    }
                    
                    break;
                
                case 2:

                    break;
                
            }
        }

        private void SetupPhase1Part1()
        {
            print("Setting up phase 1 part 1");
            
            // Deactivating all objects
            DeactivateAllObjects();
            
            // Activating necessary objects
            p1_productionsBox.SetActive(true);
            p1_variablesBox.SetActive(true);
            p1_usefulVariablesBox.SetActive(true);
            p1_uselessVariablesBox.SetActive(true);

            // Filling boxes
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.Productions.ToList());
            p1_variablesBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.Variables);
            
            // Changing productions box color
            p1_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);

            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_ProductionsBox_gray.position;
            p1_variablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_variablesBox.position;
            p1_uselessVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_uselessVariablesBox.position;
            p1_usefulVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_usefulVariablesBox.position;
        }

        private bool Phase1Part1() 
        {
            var currentVariablesOnVariablesBox = p1_variablesBox.GetComponent<VariablesBox>().variableList;
            if (currentVariablesOnVariablesBox.Count > 0)
            {
                print("Ainda há variáveis que devem ser movidas!");
                return false;
            }
            
            var currentVariablesOnUsefulBox = p1_usefulVariablesBox.GetComponent<VariablesBox>().variableList;
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

        private void SetupPhase1Part2()
        {
            print("Setting up phase 1 part 2");
            
            // Deactivating all objects
            DeactivateAllObjects();
            
            // Activating necessary objects
            p1_productionsBox.SetActive(true);
            p1_uselessVariablesBox.SetActive(true);
            p1_usefulVariablesBox.SetActive(true);
            trashBin.SetActive(true);

            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part2_ProductionsBox.position;
            p1_uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part2_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase1Part2_trashBin.position;
            
            // Changing box colors
            p1_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
            p1_uselessVariablesBox.GetComponent<VariablesBox>().SetGrayScale(true);
            
            // Making CFL_Box objects draggable again
            foreach (var productionBox in p1_productionsBox.GetComponent<ProductionsBox>().productionBoxList)
            {
                productionBox.GetComponent<Draggable>().CanBeDragged = true;
            }
            
        }

        private bool Phase1Part2()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                print("É necessário que alguma produção exista na linguagem");
                return false;
            }
            
            var correctProductions = _grammar.UsefulProductionsPhase1;
            
            if (correctProductions.Count != currentProductionsOnProductionsBox.Count) print("Número errado de produções úteis!");
            else
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        print("A producao " + production._in + " -> " + production._out +
                              " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }

                print("Correto! Pode prosseguir para a próxima fase!");
                return true;
            }
            return false;
        }

        private void SetupPhase1Part3()
        {
            print("Setting up phase 1 part 3");

            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part3_Productions_Box.position;
            p1_uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part3_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase1Part3_trashBin.position;
        }

        private bool Phase1Part3()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                print("É necessário que alguma produção exista na linguagem");
                return false;
            }
            
            var correctProductions = _grammar.usefulAndReachableProductionsPhase1;
            
            if (correctProductions.Count != currentProductionsOnProductionsBox.Count) print("Número errado de produções úteis e alcançáveis!");
            else
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        print("A producao " + production._in + " -> " + production._out +
                              " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }

                print("Correto! Pode prosseguir para a próxima fase!");
                return true;
            }
            return false;
        }
        
        private void SetupPhase2Part1()
        {
            print("Setting up Phase 2 Part 1");
            
            // Deactivating all objects
            DeactivateAllObjects();
            
            // Activating useful objects
            p2_productionsBox.SetActive(true);
            p2_variablesBox.SetActive(true);
            p2_lambdaProducersBox.SetActive(true);
            
            // Setting positions
            p2_variablesBox.transform.position = _boxPositionsManager.Anchor_Phase2Part1_variablesBox.position;
            p2_lambdaProducersBox.transform.position =
                _boxPositionsManager.Anchor_Phase2Part1_lambdaProducersBox.position;
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part1_productionsBox.position;
            
            // Filling boxes
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.usefulAndReachableProductionsPhase1);
            var variableList = new List<char>();
            foreach (var production in _grammar.UsefulProductionsPhase1.Where(production => !variableList.Contains(production._in)))
            {
                variableList.Add(production._in);
            }
            p2_variablesBox.GetComponent<VariablesBox>().FillWithVariables(variableList);
        }

        private void DeactivateAllObjects()
        {
            p1_productionsBox.SetActive(false);
            p1_variablesBox.SetActive(false);
            p1_usefulVariablesBox.SetActive(false);
            p1_uselessVariablesBox.SetActive(false);
            p2_variablesBox.SetActive(false);
            p2_lambdaProducersBox.SetActive(false);
            p2_productionsBox.SetActive(false);
            trashBin.SetActive(false);
        }
        private void Update()
        {
           

        }
    }
}
