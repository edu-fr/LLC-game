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
        [SerializeField] private GameObject p2_productionMaker;
        [SerializeField] private GameObject p2_acceptLambdaQuestionBox;
        public GameObject p2_productionsBox;
        [SerializeField] private GameObject p3_unitProductionsBox;
            
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
            SetupPhase1Part3();
            SetupPhase2Part1();
            SetupPhase2Part2();
            _currentPart = 2;
            _currentPhase = 2;
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
                    switch (_currentPart)
                    {
                        case 1:
                            if (Phase2Part1())
                            {
                                _currentPart = 2;
                                SetupPhase2Part2();
                            };
                            break;
                        
                        case 2:
                            if (Phase2Part2())
                            {
                                _currentPart = 3;
                                SetupPhase2Part3();
                            }
                            break;
                        case 3:
                            if (Phase2Part3())
                            {
                                _currentPhase = 3;
                                _currentPart = 1;
                                SetupPhase3Part1();
                            }
                            break;
                    }
                    break;
                
                case 3:
                    switch (_currentPart)
                    {
                        case 1:
                            if (Phase3Part1())
                            {
                                _currentPart = 2;
                                SetupPhase3Part2();
                            }
                            break;
                        
                        case 2:
                            if (Phase3Part2())
                            {
                                _currentPart = 3;
                                SetupPhase3Part3();
                            }
                            break;
                        
                        case 3:
                            if (Phase3Part3())
                            {
                                print("Pode prosseguir para a parte bonus!");
                                _currentPhase = 4;
                                _currentPart = 1; 
                            }
                            break;
                    }
                    break;
            }
            
        }

        private void SetupPhase1Part1()
        {
            print("Setting up phase 1 part 1");

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

            // Removing from camera unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p1_variablesBox.transform.position = outOfBoundsPosition;
            p1_usefulVariablesBox.transform.position = outOfBoundsPosition;
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
            
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p1_uselessVariablesBox.transform.position = outOfBoundsPosition; 
            p1_productionsBox.transform.position = outOfBoundsPosition;
            trashBin.transform.position = outOfBoundsPosition;
            
            // Setting positions
            p2_variablesBox.transform.position = _boxPositionsManager.Anchor_Phase2Part1_variablesBox.position;
            p2_lambdaProducersBox.transform.position =
                _boxPositionsManager.Anchor_Phase2Part1_lambdaProducersBox.position;
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part1_productionsBox.position;
            
            // Filling boxes
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.usefulAndReachableProductionsPhase1);
            var variableList = new List<char>();
            foreach (var production in _grammar.usefulAndReachableProductionsPhase1.Where(production => !variableList.Contains(production._in)))
            {
                variableList.Add(production._in);
            }
            p2_variablesBox.GetComponent<VariablesBox>().FillWithVariables(variableList);
            
            // Setting box gray
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
        }

        private bool Phase2Part1()
        {
            var correctLambdaProducers = _grammar.LambdaProducers;
            var currentVariablesOnLambdaProducersBox = p2_lambdaProducersBox.GetComponent<VariablesBox>().variableList;
            
            if (correctLambdaProducers.Count != currentVariablesOnLambdaProducersBox.Count) print("Número errado de variaveis que produzem vazio!");
            else
            {
                foreach (var variable in currentVariablesOnLambdaProducersBox)
                {
                    if (!correctLambdaProducers.Contains(variable))
                    {
                        print("A variavel " + variable + " não faz parte da lista de variáveis produtoras de lambda!");
                        return false;
                    } 
                }
                print("Correto! Pode prosseguir para a próxima fase!");
                return true;
            }
            return false;
        }

        private void SetupPhase2Part2()
        {
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_lambdaProducersBox.transform.position = outOfBoundsPosition;
            p2_variablesBox.transform.position = outOfBoundsPosition;
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part2_productionsBox.position;
            p2_productionMaker.transform.position = _boxPositionsManager.Anchor_Phase2Part2_productionMaker.position;
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
        }

        private bool Phase2Part2()
        {
            var productionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (productionList.Count != _grammar.ProductionsPhase2.Count)
            {
                print("Numero incorreto de produções!");
                return false;
            }
            foreach (var production in productionList)
            {
                var productionExists = false;
                foreach (var correctProduction in _grammar.ProductionsPhase2)
                {
                    if (production._in == correctProduction._in)
                    {
                        if (production._out == correctProduction._out)
                        {
                            productionExists = true;
                            break;
                        }
                    }
                }

                if (!productionExists)
                {
                    print("A produção " + production._in + "->" + production._out + " não está na lista de produções corretas");
                    return false;
                }
            }

            return true;
        }
        
        private void SetupPhase2Part3()
        {
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_productionMaker.transform.position = outOfBoundsPosition;
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part3_productionsBox_gray.position;
            p2_acceptLambdaQuestionBox.transform.position = _boxPositionsManager.Anchor_Phase2Part3_acceptLambdaQuestionBox.position;
            // Turning off deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            // Turning productions box gray
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
            // Setting accept lambda question box starting variable
            p2_acceptLambdaQuestionBox.GetComponent<AcceptLambdaQuestionBox>().SetStartVariable(_grammar.StartVariable);
        }
        
        private bool Phase2Part3()
        {
            var lambdaProduction = new GrammarScript.Production(_grammar.StartVariable, "λ");
            var currentProductionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            return (currentProductionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) !=
                null && _grammar.StartVariableCanProduceLambda) || (currentProductionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) ==
                null && !_grammar.StartVariableCanProduceLambda) ;
        }

        private void SetupPhase3Part1()
        {
            // Removing from camera vision unused boxes
            p2_acceptLambdaQuestionBox.transform.position = _boxPositionsManager.Anchor_OutOfBounds.position;
            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part1_productionsBox.position;
            // Turning on deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            // Setting gray scale off
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
        }

        private bool Phase3Part1()
        {
            var productionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            print("NON USELESS UNIT PRODUCTIONS!");
            foreach(var production in _grammar.NonUselessUnitProductions)
            {
                print(production._in + "->" + production._out);
            }
            if (productionList.Count != _grammar.NonUselessUnitProductions.Count) // VER SE É ESSA LISTA MESMO
            {
                print("Numero incorreto de produções!");
                return false;
            }
            foreach (var production in productionList)
            {
                var productionExists = false;
                foreach (var correctProduction in _grammar.NonUselessUnitProductions)
                {
                    if (production._in == correctProduction._in)
                    {
                        if (production._out == correctProduction._out)
                        {
                            productionExists = true;
                            break;
                        }
                    }
                }

                if (productionExists) continue;
                print("A produção " + production._in + "->" + production._out + " não está na lista de produções corretas");
                return false;
            }

            return true;
        }

        private void SetupPhase3Part2()
        {
            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part2_productionsBox.position;
            p3_unitProductionsBox.transform.position =
                _boxPositionsManager.Anchor_Phase3Part2_unitProductionsBox.position;
            // Turning off deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            // Turning on draggability 
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
        }

        private bool Phase3Part2()
        {
            var productionsBoxList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (_grammar.NonUnitProductions.Count != productionsBoxList.Count)
            {
                print("Número de produções unidade inesperado! Recebeu: " + productionsBoxList.Count + "; Esperava: " +
                      _grammar.NonUnitProductions.Count);
                return false;
            }
            foreach (var production in productionsBoxList.Where(production =>
                !_grammar.NonUnitProductions.Exists(x => x._in == production._in && x._out == production._out)))
            {
                print("A produção " + production._in + "=>" + production._out +
                      " não faz parte das produções unidade!");
                return false;
            }
            return true;
        }

        private void SetupPhase3Part3()
        {
            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part3_productionsBox.position;
            p3_unitProductionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part3_unitProductionsBox.position;
            p2_productionMaker.transform.position = _boxPositionsManager.Anchor_Phase3Part3_productionMaker.position;
            // Turning off deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            // Turning off draggability 
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
            // Setting the boxes with the correct content (debug only)
            p2_productionsBox.GetComponent<ProductionsBox>().ClearList();
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.NonUnitProductions);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
            
        }

        private bool Phase3Part3()
        {
            var productionsBoxList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (_grammar.ResultingProductions.Count != productionsBoxList.Count)
            {
                print("Número de produções finais inesperado! Recebeu: " + productionsBoxList.Count + "; Esperava: " +
                      _grammar.ResultingProductions.Count);
                return false;
            }
            
            foreach (var production in productionsBoxList.Where(production =>
                !_grammar.ResultingProductions.Exists(x => x._in == production._in && x._out == production._out)))
            {
                print("A produção " + production._in + "=>" + production._out +
                      " não faz parte das produções resultantes da fase 3!");
                return false;
            }
            
            return true; 
        }
    }
}
