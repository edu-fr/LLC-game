using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public class GrammarScript : MonoBehaviour
    {
        [Serializable()]
        public class Production
        {
            public char _in;
            public string _out;
            public Production(char productionIn, string productionOut)
            {
                _in = productionIn;
                _out = productionOut;
            }
        }
        
        public char[] Terminals;
        public char[] Variables;
        public Production[] Productions;
        public char StartVariable;
        
        /* Phase 1 variables */
        [NonSerialized] public List<char> UsefulVariables;
        [NonSerialized] public List<Production> RemovablePhase1;
        [NonSerialized] public List<Production> ProductionsPhase1;
        
        [NonSerialized] public List<Tuple<char, List<char>>> ReachableFrom;
        [NonSerialized] public List<Production> ProductionsPhase1AfterRemoveUnreachable;
        
        
        /* Phase 2 variables */
        [NonSerialized] public List<char> LambdaProducers;
        [NonSerialized] public List<Production> ProductionsPhase2;
        [NonSerialized] public List<char> RemovablePhase2;

        /* Phase 3 variables */
        [NonSerialized] public List<Tuple<char, List<char>>> UnitProductions;
        [NonSerialized] public List<Production> ProductionsPhase3;
        [NonSerialized] public List<Production> ProductionsPhase3AfterRemovingUnitProductions;
        
        

        public void Setup()
        {
            InitializeLists();
            ExecutePhase1();
            ExecutePhase2();
            ExecutePhase3();
        }

        private void InitializeLists()
        {
            UsefulVariables = new List<char>();
            RemovablePhase1 = new List<Production>();
            ProductionsPhase1 = new List<Production>();
            ReachableFrom = new List<Tuple<char, List<char>>>();
            ProductionsPhase1AfterRemoveUnreachable = new List<Production>();
        
            LambdaProducers = new List<char>();
            ProductionsPhase2 = new List<Production>();
            RemovablePhase2 = new List<char>();
            
            UnitProductions = new List<Tuple<char, List<char>>>();
            ProductionsPhase3 = new List<Production>();
            ProductionsPhase3AfterRemovingUnitProductions = new List<Production>();
        }

        
        /* Phase 1 (Useless productions) code */
        private void ExecutePhase1()
        {
            SetUsefulVariables();
            SetRemovablePhase1();
            SetProductionsPhase1();
            DebugPrintListProduction(ProductionsPhase1, "Productions phase 1: ");
            SetUnreachableStates();
        }

        private void SetUsefulVariables()
        {
            foreach (var production in Productions)                         // Checking if produce a terminal directly
            {
                if (UsefulVariables.Contains(production._in)) continue;
                var produceVariable = false;
                var productionOutArray = production._out.ToCharArray();
                for (var i = 0; i < productionOutArray.Length; i++)
                {
                    if (char.IsUpper(productionOutArray[i]))
                    {
                        // print("A producao " + production._in + " => " + production._out + 
                        //       " foi pulada pois tem a variavel " + productionOutArray[i] + "  no meio da producao!");
                        produceVariable = true;
                        break;
                    }
                }
                if (produceVariable) continue;
                UsefulVariables.Add(production._in);
            }

            var reachedEnd = false;
            var newUsefulVariableFound = false; 
            while (!reachedEnd)
            {
                foreach (var production in Productions)
                {
                    var isUsefulProducer = false; 
                    var productionOutArray = production._out.ToCharArray();
                    for (var i = 0; i < productionOutArray.Length; i++)
                    {
                        if (char.IsUpper(productionOutArray[i]))
                        {
                            if (UsefulVariables.Contains(productionOutArray[i]))
                            {
                                isUsefulProducer = true; 
                            }
                            else
                            {
                                isUsefulProducer = false;
                                break; // If the variable isn't useful, quit
                            }
                        }
                    }

                    if (isUsefulProducer && !UsefulVariables.Contains(production._in))
                    {
                        // print("A produção " + production._in + " => " + production._out + " produz variavel util." +
                        //       " Adicionando a lista de variaveis uteis!");
                        UsefulVariables.Add(production._in);
                        newUsefulVariableFound = true;
                        break;
                    }
                }
                
                if (newUsefulVariableFound)
                    newUsefulVariableFound = false;
                else
                    reachedEnd = true;
            }
        }

        private void SetRemovablePhase1()
        {
            foreach (var production in Productions)
            {
                if (!UsefulVariables.Contains(production._in)) RemovablePhase1.Add(production);
            }
        }

        private void SetProductionsPhase1()
        {
            ProductionsPhase1 = Productions.ToList().DeepClone();
            foreach (var removable in RemovablePhase1)
            {
                var foundProduction = ProductionsPhase1.Find(production => production._in == removable._in);
                ProductionsPhase1.Remove(foundProduction);
            }
        }

        private void SetUnreachableStates()
        {
            foreach (var usefulVariable in UsefulVariables)     // Add direct reachable list
            {
                var reachableVariables = new List<char>();
                foreach (var production in ProductionsPhase1.Where(production => production._in == usefulVariable))
                {
                    var productionOutCharArray = production._out.ToCharArray();
                    for (var i = 0; i < productionOutCharArray.Length; i++)
                        if (char.IsUpper(productionOutCharArray[i]))
                            if(!reachableVariables.Contains(productionOutCharArray[i])) 
                                reachableVariables.Add(productionOutCharArray[i]);
                }
                ReachableFrom.Add(new Tuple<char, List<char>>(usefulVariable, reachableVariables));
            }
            
            foreach (var reachable in ReachableFrom)
            {
                DebugPrintListChar(reachable.Item2, "1st iteration: Reachable from " + reachable.Item1 + ": ");
            }

            var reachedEnd = false;
            while (!reachedEnd)
            {
                var newReachableAdded = false;
                foreach (var usefulVariable in UsefulVariables)     // For each variable on UsefulVariables
                {
                    // print("TRABALHANDO COM A VARIAVEL " + usefulVariable + "!!");
                    var currentTuple = ReachableFrom.Find(reachable => reachable.Item1 == usefulVariable); // Been working useful variable
                    foreach (var reachable in currentTuple.Item2)         // For each variable reachable from the current useful variable
                    {
                        // print("Olhando a variavel alcançável por " + usefulVariable + ": " + reachable);
                        var reachableListPreviousSize = currentTuple.Item2.Count;
                        // print("Numero de alcancáveis por " + usefulVariable + " antes de adicionar algo: " + reachableListPreviousSize);

                        var currentInsideTuple = ReachableFrom.Find(x => x.Item1 == reachable);
                        if (currentInsideTuple == null)
                        {
                            print("A variável " + reachable + " nao sai de lugar nenhum!! Ignorando... ");
                            continue;
                        }
                        var currentInsideReachableList = currentInsideTuple.Item2;   // current reachable item from the useful variable 
                        if (currentInsideReachableList.Count < 1)
                        {
                            // print("Não foram encontradas variaveis alcancáveis a partir de " + reachable + "!");
                            continue;
                        }
                        // print("Foram encontradas " + currentInsideReachableList.Count + " variaveis alcancáveis a partir de " + reachable + "! Seguem: ");
                        foreach (var item in currentInsideReachableList)
                        {
                            // print(item);
                        }
                        
                        foreach (var insideReachableListItem in currentInsideReachableList)    // Add all not already present reachable items 
                        {
                            // print("Tentando adicionar a variavel " + insideReachableListItem + " a lista de variaveis chegaveis a partir de " + usefulVariable + "!");
                            if (!currentTuple.Item2.Contains(insideReachableListItem))
                            {
                                // print("Foi possível adicionar o item " + insideReachableListItem + " na lista de chegaveis por " + usefulVariable + "!");
                                currentTuple.Item2.Add(insideReachableListItem);
                            }
                            else
                            {
                                // print("Não foi possível adicionar pois já continha o item " + insideReachableListItem + " na lista de chegaveis por " + usefulVariable + "!");
                            }
                        }

                        var reachableListSizeAfterInsertion = currentTuple.Item2.Count;
                        
                        // print("Numero de alcancáveis por " + usefulVariable + " depois de tentar adicionar algo: " + reachableListSizeAfterInsertion);
                        
                        if (reachableListSizeAfterInsertion - reachableListPreviousSize > 0)
                        {
                            // print("Houveram adições " + usefulVariable + " na lista de chegados a partir de " + usefulVariable +"!");
                            newReachableAdded = true;
                            break;
                        }
                    }

                    if (newReachableAdded)
                    {
                        reachedEnd = false;
                        break;
                    }
                }

                if (!newReachableAdded)
                {
                    reachedEnd = true; 
                }
            }

            foreach (var reachable in ReachableFrom)
            {
                DebugPrintListChar(reachable.Item2, "2nd iteration: Reachable from " + reachable.Item1 + ": ");
            }
            
            
            ProductionsPhase1AfterRemoveUnreachable = ProductionsPhase1.ToList().DeepClone();

            foreach (var production in ProductionsPhase1)
            {
                if (!ReachableFrom.Find(x => x.Item1 == StartVariable).Item2.Contains(production._in))
                {
                    var foundVariable =
                        ProductionsPhase1AfterRemoveUnreachable.Find(x =>
                            x._in == production._in && x._out == production._out);
                    ProductionsPhase1AfterRemoveUnreachable.Remove(foundVariable);
                }
            } 
            
            DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable items: ");

            var auxProductionsPhase1 = ProductionsPhase1AfterRemoveUnreachable.DeepClone();
            foreach (var production in auxProductionsPhase1)
            {
                for (var i = 0; i < production._out.Length; i++)
                {
                    if (char.IsUpper((production._out[i])))
                    {
                        if (!UsefulVariables.Contains(production._out[i]))
                        {
                            ProductionsPhase1AfterRemoveUnreachable.Remove(ProductionsPhase1AfterRemoveUnreachable.Find(productionX =>
                                productionX._in == production._in && productionX._out == production._out));
                            break;
                        }
                    }
                }
            }
            DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable variables: ");
        }

        private void DebugPrintListChar(List<char> list, string text)
        {
            if (list.Count < 1) return;
            print(text);
            foreach (var item in list)
            {
                print(item);
            }
        }
        
        private void DebugPrintListProduction(List<Production> list, string text)
        {
            if (list.Count < 1) return;
            print(text);
            foreach (var item in list)
            {
                print(item._in + " => " + item._out);
            }
        }
        
        /* Phase 2 (Empty productions) code */
        private void ExecutePhase2()
        {
            SetLambdaProducers(); 
            DebugPrintListChar(LambdaProducers, "Lambda producers: ");
            SetInsertablePhase2();
            DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after inserting new productions: ");
            SetRemovablePhase2();
            RemoveLambdaProductionsFromPhase2();
            DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after removing");
        }

    
        private void SetLambdaProducers()
        {
            LambdaProducers = (from production in ProductionsPhase1AfterRemoveUnreachable where production._out[0] == 'l' select production._in).ToList();
            var newLambdaProducers = new List<char>();
            // Discover Variables that produces lambda productions
            foreach (var lambdaProducer in LambdaProducers)
            {
                foreach (var production in ProductionsPhase1AfterRemoveUnreachable)
                {
                    if (production._out.Length > 1) continue;
                    if (production._out[0] == lambdaProducer)
                    {
                        newLambdaProducers.Add(production._in);
                    }
                }
            }

            LambdaProducers.AddRange(newLambdaProducers);
        }

        private void SetInsertablePhase2()
        {
            ProductionsPhase2 = ProductionsPhase1AfterRemoveUnreachable.DeepClone();
            
            foreach (var lambdaProducer in LambdaProducers)
            {
                foreach (var production in Productions)
                {
                    if (!production._out.Contains(lambdaProducer)) continue;
                    var characterChain = production._out.ToCharArray();
                    if(characterChain.Length == 1) continue;                    // We can't produce a empty production
                    foreach (var t in characterChain)
                    {
                        if (t != lambdaProducer) continue;
                        var newString = new StringBuilder();
                        var variableCount = 0;
                        
                        for (var j = 0; j < characterChain.Length; j++)         // Count how many variations will be produced
                        {
                            if (characterChain[j] == t)
                                variableCount += 1;
                            
                            
                        }

                        for (var k = 0; k < variableCount; k++)                 // Produce each variation
                        {
                            var variationCount = variableCount;
                            for (var j = 0; j < characterChain.Length; j++)
                            {
                                if (characterChain[j] == t)
                                {
                                    variableCount -= 1;
                                    continue;
                                }
                                newString.Append(characterChain[j]);
                            }    
                        }
                        ProductionsPhase2.Add(new Production(production._in, newString.ToString()));
                    }
                }
            }
        }
        
        private void SetRemovablePhase2()
        {
            foreach (var production in ProductionsPhase2.Where(production => production._out == "l"))
            {
                RemovablePhase2.Add(production._in);
            }
        }

        private void RemoveLambdaProductionsFromPhase2()
        {
            if (RemovablePhase2.Count < 1) return;
            var removeList = ProductionsPhase2.Where(production => production._out == "l").ToList();
            foreach (var removable in removeList)
            {
                ProductionsPhase2.Remove(removable);
            }
        }
        
        
        /* Phase 3 (Unit productions) code */
        private void ExecutePhase3()
        {
            SetUnitProductions();
            DebugPrintUnitProductions();
            SetInsertablePhase3();
            
            print("PHASE 3 FINAL TEST: ");
            
            DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions: ");
            
            DebugPrintListProduction(ProductionsPhase3, "Phase 3 productions: ");

            RemoveUnitProductions();
            
            DebugPrintListProduction(ProductionsPhase3AfterRemovingUnitProductions.OrderBy(x => x._in).ToList(), "Final results: ");
        }

        private void SetUnitProductions()
        {
            foreach (var variable in Variables)
                UnitProductions.Add(new Tuple<char, List<char>>(variable, new List<char>()));

            foreach (var tuple in UnitProductions)
            {
                foreach (var production in ProductionsPhase2)
                {
                    if (tuple.Item1 != production._in) continue;            // If not the same producer
                    if (production._out.Length > 1) continue;               // If production is not a unit, jump
                    if (char.IsUpper(production._out.ToCharArray()[0]))     // If it's a variable
                        tuple.Item2.Add(production._out.ToCharArray()[0]);  // Add to the tuple list
                }
            }
        }

        private void DebugPrintUnitProductions()
        {
            foreach (var tuple in UnitProductions)
            {
                print("Fecho de " + tuple.Item1 + ": ");
                foreach (var variable in tuple.Item2)
                {
                    print(variable);
                }
            }
        }
        
        private void SetInsertablePhase3()
        {
            ProductionsPhase3 = ProductionsPhase2.DeepClone();
            foreach (var tuple in UnitProductions.Where(tuple => tuple.Item2.Count > 0))
            {
                foreach (var producer in tuple.Item2)
                {
                    foreach (var production in ProductionsPhase2.Where(production => production._in == producer))
                    {
                        ProductionsPhase3.Add(new Production(tuple.Item1, production._out));
                    }
                }
            }

        }
        
        private void RemoveUnitProductions()
        {
            ProductionsPhase3AfterRemovingUnitProductions = ProductionsPhase3.DeepClone();
            foreach (var unitProduction in UnitProductions)
            {
                foreach (var innerProduction in unitProduction.Item2)
                {
                    ProductionsPhase3AfterRemovingUnitProductions.RemoveAll(production => production._out.Length == 1 &&
                        production._in == unitProduction.Item1 && production._out.ToCharArray()[0] == innerProduction);
                }
            }
        }

    }
}
