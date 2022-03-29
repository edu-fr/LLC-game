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
        public enum GameState {
            Running, Paused, PopUp, Lost
        }

        public GameState currentGameState;
        public GameState previouslyGameState; 
        
        public GameObject grammarObject;
        private GrammarScript _grammar;
        [SerializeField] private CanvasController canvasController;

        [SerializeField] private GameObject p1_productionsBox;
        [SerializeField] private GameObject p1_productionsBox_resetButton;
        [SerializeField] private GameObject p1_variablesBox;
        [SerializeField] private GameObject p1_usefulVariablesBox;
        [SerializeField] private GameObject p1_uselessVariablesBox;
        [SerializeField] private GameObject p2_variablesBox;
        [SerializeField] private GameObject p2_lambdaProducersBox;
        [SerializeField] private GameObject p2_productionMaker;
        [SerializeField] private GameObject p2_acceptLambdaQuestionBox;
        public GameObject p2_productionsBox;
        public GameObject p2_productionsBox_resetButton;
        [SerializeField] private GameObject p3_unitProductionsBox;
        [SerializeField] private GameObject p3_unitProductionsBox_resetButton;
            
        [SerializeField] private GameObject trashBin;
        
        public int currentPhase { private set; get; }
        public int currentPart { private set; get; }
        
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
            // Accept lambda question box config
            p2_acceptLambdaQuestionBox.GetComponent<AcceptLambdaQuestionBox>().SetStartVariable(_grammar.StartVariable);
            
            // Set initial phase and part
            currentPhase = 1;
            currentPart = 1;

            canvasController.Transition(currentPhase);
            SetupPhase1Part1();
        }

        private void Update()
        {
            switch (currentGameState)
            {
                case GameState.Running:
                    canvasController.UpdateTime();
                    // Open pause menu
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ChangeGameState(GameState.Paused);
                    }
                    break;
                
                case GameState.Paused:
                    // Unpause
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ChangeGameState(previouslyGameState);
                    }
                    
                    break;
                
                case GameState.PopUp:
                   
                    break;
                
                case GameState.Lost:
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (canvasController.remainingTime <= 0 || canvasController.remainingLife <= 0)
            {
                SoundManager.instance.Play("Player lose");
                canvasController.OpenTryAgainScreen();
                currentGameState = GameState.Lost;
            }
        }

        public void Unpause()
        {
            ChangeGameState(GameState.Running);    
        }

        public void UnpopUp()
        {   
            ChangeGameState(GameState.Running);
        }

        public void ChangeGameState(GameState gameState)
        {
            switch(gameState)
            {
                case GameState.Running:
                    previouslyGameState = currentGameState;
                    currentGameState = GameState.Running;
                    Time.timeScale = 1f;
                    canvasController.ShowPausePanel(false);
                    break;
                
                case GameState.Paused:
                    previouslyGameState = currentGameState;
                    currentGameState = GameState.Paused;
                    Time.timeScale = 0f;
                    canvasController.ShowPausePanel(true);
                    break;
                
                case GameState.PopUp:
                    previouslyGameState = currentGameState;
                    currentGameState = GameState.PopUp;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }

       

        public void TryNextPhase()
        {
            switch (currentPhase)
            {
                case 1:
                    switch (currentPart)
                    {
                        case 1:
                            if (Phase1Part1())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 2;
                                SetupPhase1Part2();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            
                            break;
                        
                        case 2:
                            if (Phase1Part2())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 3;
                                SetupPhase1Part3();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 3:
                            if (Phase1Part3())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 1;
                                currentPhase = 2;
                                canvasController.Transition(currentPhase);
                                SetupPhase2Part1();
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                    }
                    
                    break;
                
                case 2:
                    switch (currentPart)
                    {
                        case 1:
                            if (Phase2Part1())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 2;
                                SetupPhase2Part2();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 2:
                            if (Phase2Part2())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 3;
                                SetupPhase2Part3();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        case 3:
                            if (Phase2Part3())
                            {
                                SoundManager.instance.Play("Right");
                                currentPhase = 3;
                                currentPart = 1;
                                canvasController.Transition(currentPhase);
                                SetupPhase3Part1();
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                    }
                    break;
                
                case 3:
                    switch (currentPart)
                    {
                        case 1:
                            if (Phase3Part1())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 2;
                                SetupPhase3Part2();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 2:
                            if (Phase3Part2())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 3;
                                SetupPhase3Part3();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 3:
                            if (Phase3Part3())
                            {
                                SoundManager.instance.Play("Right");
                                currentPhase = 4;
                                currentPart = 1; 
                                canvasController.Transition(currentPhase);
                                SetupPhase4Part1();
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                    }
                    break;
                
                case 4:
                    switch (currentPart)
                    {
                        case 1:
                            if (Phase4Part1())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 2; 
                                SetupPhase4Part2();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 2:
                            if (Phase4Part2())
                            {
                                SoundManager.instance.Play("Right");
                                currentPart = 3;
                                SetupPhase4Part3();
                                canvasController.ActivateTutorial(currentPhase, currentPart);
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                        
                        case 3:
                            if (Phase4Part3())
                            {
                                SoundManager.instance.Play("Right");
                                print("VENCEU!!");
                            }
                            else
                                canvasController.PlayerMistake();   
                            break;
                    }
                    break; 
            }
            
        }

        private void SetupPhase1Part1()
        {
            print("Setting up phase 1 part 1");
            
            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_ProductionsBox_gray.position;
            p1_variablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_variablesBox.position;
            p1_uselessVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_uselessVariablesBox.position;
            p1_usefulVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_usefulVariablesBox.position;
            
            // Filling boxes
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.Productions.ToList());
            p1_variablesBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.Variables);
            p1_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDraggability(true);
            p1_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDeletability(false);

            // Changing productions box color
            p1_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
    
            // Setting off reset button
            p1_productionsBox_resetButton.SetActive(false);
        }

        private bool Phase1Part1() 
        {
            var currentVariablesOnVariablesBox = p1_variablesBox.GetComponent<VariablesBox>().variableList;
            if (currentVariablesOnVariablesBox.Count > 0)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "Ainda há variáveis que devem ser movidas!");
                return false;
            }
            
            var currentVariablesOnUsefulBox = p1_usefulVariablesBox.GetComponent<VariablesBox>().variableList;
            var correctVariables = _grammar.UsefulVariablesPhase1;

            if (correctVariables.Count > currentVariablesOnUsefulBox.Count) 
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos variáveis úteis do que o esperado!");
            else if (correctVariables.Count < currentVariablesOnUsefulBox.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais variáveis úteis do que o esperado!");
            }
            else
            {
                foreach (var variable in currentVariablesOnUsefulBox)
                {
                    if (!correctVariables.Contains(variable))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A variável " + variable + " não faz parte da lista de variáveis corretas!");
                        return false;
                    } 
                }
                // Victory screen and fanfare
                return true;
            }
            return false;
            
        }

        private void SetupPhase1Part2()
        {
            print("Setting up phase 1 part 2");
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.Productions.ToList());
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.Productions.ToList();
            
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
            
            // Making productions box objects draggable and deletable again
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            
            // Setting on reset button
            p1_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase1Part2()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "É necessário que alguma produção exista na linguagem!");
                return false;
            }
            
            var correctProductions = _grammar.UsefulProductionsPhase1;
            
            if (correctProductions.Count > currentProductionsOnProductionsBox.Count) 
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções na janela de produções corretas do que o esperado!");
            else if (correctProductions.Count < currentProductionsOnProductionsBox.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções na janela de produções corretas do que o esperado!");
            } 
            else 
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A producao " + production._in + " -> " + production._out + " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }

                // Victory screen and fanfare
                return true;
            }
            return false;
        }

        private void SetupPhase1Part3()
        {
            print("Setting up phase 1 part 3");
            // Filling box independently of other phases
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.UsefulProductionsPhase1);
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.UsefulProductionsPhase1;

            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part3_Productions_Box.position;
            p1_uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part3_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase1Part3_trashBin.position;
            
            // Setting draggability and deletability
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
        }

        private bool Phase1Part3()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "É necessário que alguma produção exista na linguagem!");
                return false;
            }
            
            var correctProductions = _grammar.usefulAndReachableProductionsPhase1;
            
            if (correctProductions.Count > currentProductionsOnProductionsBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções na janela de produções úteis e alcançáveis do que o esperado!");
            else if (correctProductions.Count < currentProductionsOnProductionsBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções na janela de produções úteis e alcançáveis do que o esperado!");
            else
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A producao " + production._in + " -> " + production._out + " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }

                // Victory screen and fanfare
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
            p2_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDraggability(true);
            p2_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDeletability(false);

            // Setting box gray
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
            
            // Setting off reset button
            p2_productionsBox_resetButton.SetActive(false);
        }

        private bool Phase2Part1()
        {
            var correctLambdaProducers = _grammar.LambdaProducers;
            var currentVariablesOnLambdaProducersBox = p2_lambdaProducersBox.GetComponent<VariablesBox>().variableList;
            
            
            if (correctLambdaProducers.Count > currentVariablesOnLambdaProducersBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos variáveis que produzem vazio do que o esperado!");
            else if (correctLambdaProducers.Count < currentVariablesOnLambdaProducersBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais variáveis que produzem vazio do que o esperado!");
            else
            {
                foreach (var variable in currentVariablesOnLambdaProducersBox)
                {
                    if (!correctLambdaProducers.Contains(variable))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A variável " + variable + " não é produtora de vazio!");
                        return false;
                    } 
                }
                
                // Victory screen and fanfare
                return true;
            }
            return false;
        }

        private void SetupPhase2Part2()
        {
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_variablesBox.transform.position = outOfBoundsPosition;
            
            // Filling productions box independently of other phases
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.usefulAndReachableProductionsPhase1);
            // Saving the original production box content
            p2_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.usefulAndReachableProductionsPhase1;

            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part2_productionsBox.position;
            p2_productionMaker.transform.position = _boxPositionsManager.Anchor_Phase2Part2_productionMaker.position;
            p2_lambdaProducersBox.transform.position = _boxPositionsManager.Anchor_Phase2Part2_lambdaProducersBox.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase2Part2_trashBin.position;
            // Setting on deletability and draggability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetAllVariablesDeletability(false);
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetAllVariablesDraggability(false);
            // Setting off production box gray scale
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
            // Setting on lambda producers box gray scale
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetGrayScale(true);
            
            
            // Setting on reset button
            p2_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase2Part2()
        {
            var productionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (productionList.Count > _grammar.ProductionsPhase2.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções do que o esperado!");
                return false;
            }
            if (productionList.Count < _grammar.ProductionsPhase2.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções do que o esperado!");
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
                    canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A produção " + production._in + "->" + production._out + " não é esperada");
                    return false;
                }
            }

            // Victory screen and fanfare
            return true;
        }
        
        private void SetupPhase2Part3()
        {
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_productionMaker.transform.position = outOfBoundsPosition;
            trashBin.transform.position = outOfBoundsPosition;
            // Filling boxes independently of other phases
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.ProductionsPhase2);
            p2_lambdaProducersBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.LambdaProducers);
            // Moving some useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase2Part3_productionsBox_gray.position;
            p2_acceptLambdaQuestionBox.transform.position = _boxPositionsManager.Anchor_Phase2Part3_acceptLambdaQuestionBox.position;
            p2_lambdaProducersBox.transform.position = _boxPositionsManager.Anchor_Phase2Part3_lambdaProducersBox.position;
            // Turning off deletability and draggability for both boxes
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetAllVariablesDeletability(false);
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetAllVariablesDraggability(false);
            // Turning productions box gray
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
            p2_lambdaProducersBox.GetComponent<VariablesBox>().SetGrayScale(true);
            // Setting accept lambda question box starting variable
            
            // Setting off reset button
            p2_productionsBox_resetButton.SetActive(false);
        }
        
        private bool Phase2Part3()
        {
            var lambdaProduction = new GrammarScript.Production(_grammar.StartVariable, "λ");
            var currentProductionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if ((currentProductionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) !=
                null && _grammar.StartVariableCanProduceLambda) || (currentProductionList.Find(x => x._in == lambdaProduction._in && x._out == lambdaProduction._out) ==
                null && !_grammar.StartVariableCanProduceLambda))
            {
                // Victory screen and fanfare
                return true;
            }
            return false;
        }

        private void SetupPhase3Part1()
        {
            // Removing from camera vision unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_acceptLambdaQuestionBox.transform.position = outOfBoundsPosition;
            p2_lambdaProducersBox.transform.position = outOfBoundsPosition;
           
            // Filling productions box independently of other phases
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.ProductionsPhase2WithoutLambdaProductionsAndWithLambdaFromStart);
            // Saving the original production box content
            p2_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.ProductionsPhase2WithoutLambdaProductionsAndWithLambdaFromStart;

            // Moving useful boxes
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase3Part1_trashBin.position; 
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part1_productionsBox.position;
            // Turning on deletability and Draggability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            // Setting gray scale off
            p2_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
            
            // Setting on reset button
            p2_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase3Part1()
        {
            var productionList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            // print("NON USELESS UNIT PRODUCTIONS!");
            // foreach(var production in _grammar.NonUselessUnitProductions)
            // {
            //     print(production._in + "->" + production._out);
            // }
            if (productionList.Count > _grammar.NonUselessUnitProductions.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções do que o esperado!");
                return false;
            }
            
            if (productionList.Count < _grammar.NonUselessUnitProductions.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções do que o esperado!");
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
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A produção " + production._in + "->" + production._out + " não é esperada");
                return false;
            }

            // Victory screen and fanfare
            return true;
        }

        private void SetupPhase3Part2()
        {
            // Moving useless boxes to out of bounds position
            trashBin.transform.position = _boxPositionsManager.Anchor_OutOfBounds.position;
            
            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part2_productionsBox.position;
            p3_unitProductionsBox.transform.position =
                _boxPositionsManager.Anchor_Phase3Part2_unitProductionsBox.position;
            // Turning off deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            // Turning on draggability 
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            // Setting off reset button
            p3_unitProductionsBox_resetButton.SetActive(false);
            p2_productionsBox_resetButton.SetActive(false);
        }

        private bool Phase3Part2()
        {
            var productionsBoxList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (_grammar.NonUnitProductions.Count < productionsBoxList.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções unidade do que o esperado!");
                return false;
            }
            if (_grammar.NonUnitProductions.Count > productionsBoxList.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções unidade do que o esperado!");
                return false;
            }

            foreach (var production in productionsBoxList.Where(production =>
                !_grammar.NonUnitProductions.Exists(x => x._in == production._in && x._out == production._out)))
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!","A produção " + production._in + "=>" + production._out +
                                                                     " não faz parte das produções unidade!");
                return false;
            }
            
            // Victory screen and fanfare
            return true;
        }

        private void SetupPhase3Part3()
        {
            // Filling productions box independently of other phases
            p2_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.NonUnitProductions);
            // Saving the original production box content
            p2_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.NonUnitProductions;
            
            // p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true); // verificar?
            // Moving useful boxes
            p2_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part3_productionsBox.position;
            p3_unitProductionsBox.transform.position = _boxPositionsManager.Anchor_Phase3Part3_unitProductionsBox.position;
            p2_productionMaker.transform.position = _boxPositionsManager.Anchor_Phase3Part3_productionMaker.position;
            // Turning off deletability
            p2_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            // Turning off unit productions box drop acceptance
            p3_unitProductionsBox.GetComponent<ProductionsBox>().acceptDrop = false;
            // Turning off draggability 
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
            // Turning gray
            p3_unitProductionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
            // Setting off reset button
            p3_unitProductionsBox_resetButton.SetActive(false);
            p2_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase3Part3()
        {
            var productionsBoxList = p2_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (_grammar.ResultingProductions.Count < productionsBoxList.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções do que o esperado!");
                return false;
            }
            if (_grammar.ResultingProductions.Count > productionsBoxList.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções do que o esperado!");
                return false;
            }
            
            foreach (var production in productionsBoxList.Where(production =>
                !_grammar.ResultingProductions.Exists(x => x._in == production._in && x._out == production._out)))
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A produção " + production._in + "=>" + production._out + " não é esperada!");
                return false;
            }
            
            // Victory screen and fanfare
            return true; 
        }

        private void SetupPhase4Part1()
        {
            print("Setting up phase 4 part 1");
            
            // Removing useless boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p2_productionsBox.transform.position = outOfBoundsPosition;
            p2_productionMaker.transform.position = outOfBoundsPosition;
            p3_unitProductionsBox.transform.position = outOfBoundsPosition;
            
            // Filling boxes
            p1_productionsBox.GetComponent<ProductionsBox>().ClearList();
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.ResultingProductions);
            p1_variablesBox.GetComponent<VariablesBox>().ClearList();
            p1_variablesBox.GetComponent<VariablesBox>().FillWithVariables(_grammar.VariablesPhase4);
            // Cleaning some boxes
            p1_usefulVariablesBox.GetComponent<VariablesBox>().ClearList();
            p1_uselessVariablesBox.GetComponent<VariablesBox>().ClearList();
            // Changing box color
            p1_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(true);
            p1_uselessVariablesBox.GetComponent<VariablesBox>().SetGrayScale(false);
            // Changing deletability and draggability
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(false);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(false);
            p1_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDraggability(true);
            p1_variablesBox.GetComponent<VariablesBox>().SetAllVariablesDeletability(false);
            
            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_ProductionsBox_gray.position;
            p1_variablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_variablesBox.position;
            p1_uselessVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_uselessVariablesBox.position;
            p1_usefulVariablesBox.transform.position = _boxPositionsManager.Anchor_Phase1Part1_usefulVariablesBox.position;
            
            // Setting off reset button
            p1_productionsBox_resetButton.SetActive(false);
        }
        
        
        private bool Phase4Part1() 
        {
            var currentVariablesOnVariablesBox = p1_variablesBox.GetComponent<VariablesBox>().variableList;
            if (currentVariablesOnVariablesBox.Count > 0)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "Ainda há variáveis que devem ser movidas!");
                return false;
            }
            
            
            
            var currentVariablesOnUsefulBox = p1_usefulVariablesBox.GetComponent<VariablesBox>().variableList;
            var correctVariables = _grammar.UsefulVariablesPhase4;

            if (correctVariables.Count > currentVariablesOnUsefulBox.Count) 
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos variáveis úteis do que o esperado!");
            else if (correctVariables.Count < currentVariablesOnUsefulBox.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais variáveis úteis do que o esperado!");
            }
            else
            {
                foreach (var variable in currentVariablesOnUsefulBox)
                {
                    if (!correctVariables.Contains(variable))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A variável " + variable + " não faz parte da lista de variáveis corretas!");
                        return false;
                    } 
                }
                print("Correto! Pode prosseguir para a próxima fase!");
                return true;
            }
            // Victory screen and fanfare
            return false;
        }

        private void SetupPhase4Part2()
        {
            print("Setting up phase 4 part 2");
            // Removing from camera unused boxes
            var outOfBoundsPosition = _boxPositionsManager.Anchor_OutOfBounds.position;
            p1_variablesBox.transform.position = outOfBoundsPosition;
            p1_usefulVariablesBox.transform.position = outOfBoundsPosition;
            
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.ResultingProductions);
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.ResultingProductions;
            
            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part2_ProductionsBox.position;
            p1_uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part2_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase1Part2_trashBin.position;
            
            // Changing box colors
            p1_productionsBox.GetComponent<ProductionsBox>().SetGrayScale(false);
            p1_uselessVariablesBox.GetComponent<VariablesBox>().SetGrayScale(true);
            
            // Making productions_Box objects draggable and deletable again
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);
    
            // Setting on reset button
            p1_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase4Part2()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "É necessário que alguma produção exista na linguagem!");
                return false;
            }
            
            var correctProductions = _grammar.UsefulProductionsPhase4;
            
            if (correctProductions.Count > currentProductionsOnProductionsBox.Count) 
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções na janela de produções corretas do que o esperado!");
            else if (correctProductions.Count < currentProductionsOnProductionsBox.Count)
            {
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções na janela de produções corretas do que o esperado!");
            } 
            else 
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A producao " + production._in + " -> " + production._out + " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }

                // Victory screen and fanfare
                return true;
            }
            return false;
        }

        private void SetupPhase4Part3()
        {
            print("Setting up phase 4 part 3");
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().FillWithProductions(_grammar.UsefulProductionsPhase4);
            // Saving the original production box content
            p1_productionsBox.GetComponent<ProductionsBox>().originalProductionList = _grammar.UsefulProductionsPhase4;

            // Setting positions
            p1_productionsBox.transform.position = _boxPositionsManager.Anchor_Phase1Part3_Productions_Box.position;
            p1_uselessVariablesBox.transform.position =
                _boxPositionsManager.Anchor_Phase1Part3_uselessVariablesBox_gray.position;
            trashBin.transform.position = _boxPositionsManager.Anchor_Phase1Part3_trashBin.position;
            
            // Making productions_Box objects draggable and deletable again
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDraggability(true);
            p1_productionsBox.GetComponent<ProductionsBox>().SetAllProductionsDeletability(true);

            // Setting on reset button
            p1_productionsBox_resetButton.SetActive(true);
        }

        private bool Phase4Part3()
        {
            var currentProductionsOnProductionsBox = p1_productionsBox.GetComponent<ProductionsBox>().productionList;
            if (currentProductionsOnProductionsBox.Count < 1)
            {
                canvasController.ConfigureAndCallHelpModal("Dica", "É necessário que alguma produção exista na linguagem!");
                return false;
            }
            
            var correctProductions = _grammar.usefulAndReachableProductionsPhase4;
            
            if (correctProductions.Count > currentProductionsOnProductionsBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há menos produções na janela de produções úteis e alcançáveis do que o esperado!");
            else if (correctProductions.Count < currentProductionsOnProductionsBox.Count)
                canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "Há mais produções na janela de produções úteis e alcançáveis do que o esperado!");
            else
            {
                foreach (var production in currentProductionsOnProductionsBox)
                {
                    if (!correctProductions.Any(correctProduction =>
                        correctProduction._in == production._in && correctProduction._out == production._out))
                    {
                        canvasController.ConfigureAndCallHelpModal("Resposta incorreta!", "A producao " + production._in + " -> " + production._out + " não faz parte da lista de producoes corretas!");
                        return false;
                    }
                }
                // Victory screen and fanfare
                canvasController.OpenYouWinScreen();
                return true;
            }
            return false;
        }
        
    }
}
