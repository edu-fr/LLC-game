using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Unity.Mathematics;
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
        [NonSerialized] public bool StartVariableCanProduceLambda;

        /* Phase 3 variables */
        [NonSerialized] public List<Tuple<char, List<char>>> UnitProductions;
        [NonSerialized] public List<Production> NonUnitProductions;
        [NonSerialized] public List<Production> ProductionsPhase3;
        [NonSerialized] public List<Production> ResultingProductions;
        
        

        public void Setup()
        {
            InitializeLists();
            ExecutePhase1(Productions);
            DebugPrintListProduction(ProductionsPhase1.OrderBy(x => x._in).ToList(), "Productions phase 1: ");
            DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable.OrderBy(x => x._in).ToList(), "Productions phase 1 after removing unreachable: ");
            
            Debug.Log(" ================================================================= ");
            ExecutePhase2();
            DebugPrintListProduction(ProductionsPhase2.OrderBy(x => x._in).ToList(), "Productions phase 2: ");
            
            Debug.Log(" ================================================================= ");
            ExecutePhase3();
            DebugPrintListProduction(ResultingProductions.OrderBy(x => x._in).ToList(), "Productions phase 3 after removing unit: ");

            ExecutePhase1(ResultingProductions.ToArray()); // Yes, again
            DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable.OrderBy(x => x._in).ToList(), "Productions phase 1 AGAIN after removing unreachable: ");

        }

        private void InitializeLists()
        {
            InitializePhase1Lists();
        
            LambdaProducers = new List<char>();
            ProductionsPhase2 = new List<Production>();
            RemovablePhase2 = new List<char>();
            
            UnitProductions = new List<Tuple<char, List<char>>>();
            NonUnitProductions = new List<Production>();
            ProductionsPhase3 = new List<Production>();
            ResultingProductions = new List<Production>();
        }

        private void InitializePhase1Lists()
        {
            UsefulVariables = new List<char>();
            RemovablePhase1 = new List<Production>();
            ProductionsPhase1 = new List<Production>();
            ReachableFrom = new List<Tuple<char, List<char>>>();
            ProductionsPhase1AfterRemoveUnreachable = new List<Production>();
        }
        
        /* Phase 1 (Useless productions) code */
        private List<Production> ExecutePhase1(Production[] givenProductions)
        {
            InitializePhase1Lists();
            SetUsefulVariables(givenProductions);
            DebugPrintListChar(UsefulVariables, "Variáveis ÚTEIS: ");
            SetRemovablePhase1(givenProductions);
            SetProductionsPhase1(givenProductions);
            var onlyUsefulProductions = SetUnreachableAndRemoveThem();
            return onlyUsefulProductions;
        }

        private void SetUsefulVariables(Production[] givenProductions)
        {
            foreach (var production in givenProductions)                         // Checking if produce a terminal directly
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
                foreach (var production in givenProductions)
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

        private void SetRemovablePhase1(IEnumerable<Production> givenProductions)
        {
            foreach (var production in givenProductions)
            {
                if (!UsefulVariables.Contains(production._in)) RemovablePhase1.Add(production);
            }
        }

        private void SetProductionsPhase1(IEnumerable<Production> givenProductions)
        {
            ProductionsPhase1 = givenProductions.ToList().DeepClone();
            foreach (var removable in RemovablePhase1)
            {
                var foundProduction = ProductionsPhase1.Find(production => production._in == removable._in);
                ProductionsPhase1.Remove(foundProduction);
            }
        }

        private List<Production> SetUnreachableAndRemoveThem()
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
            
            // foreach (var reachable in ReachableFrom)
            // {
            //     DebugPrintListChar(reachable.Item2, "1st iteration: Reachable from " + reachable.Item1 + ": ");
            // }

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
                            // print("A variável " + reachable + " nao sai de lugar nenhum!! Ignorando... ");
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

            // foreach (var reachable in ReachableFrom)
            // {
            //     DebugPrintListChar(reachable.Item2, "2nd iteration: Reachable from " + reachable.Item1 + ": ");
            // }
            
            
            ProductionsPhase1AfterRemoveUnreachable = ProductionsPhase1.ToList().DeepClone();

            foreach (var production in ProductionsPhase1)
            {
                if (!ReachableFrom.Find(x => x.Item1 == StartVariable).Item2.Contains(production._in))
                {
                    var foundVariable =
                        ProductionsPhase1AfterRemoveUnreachable.Find(x =>
                            x._in == production._in && x._out == production._out);
                    if (foundVariable._in == StartVariable) continue; // Make sure that the starting variable won't be excluded
                    ProductionsPhase1AfterRemoveUnreachable.Remove(foundVariable);
                }
            } 
            
            // DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable items: ");

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
            // DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable variables: ");
            return ProductionsPhase1AfterRemoveUnreachable;
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
            // DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after inserting new productions: ");
            SetRemovablePhase2();
            RemoveLambdaProductionsFromPhase2();
            // DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after removing");
        }

    
        private void SetLambdaProducers()
        {
            LambdaProducers = (from production in ProductionsPhase1AfterRemoveUnreachable
                where production._out[0] == 'l' && !LambdaProducers.Contains(production._in)
                select production._in).ToList();
            var newLambdaProducers = new List<char>();
            // Discover Variables that produces lambda productions
            foreach (var lambdaProducer in LambdaProducers)
            {
                foreach (var production in ProductionsPhase1AfterRemoveUnreachable)
                {
                    if (production._out.Length > 1) continue;
                    if (production._out[0] == lambdaProducer && !newLambdaProducers.Contains(production._in))
                    {
                        newLambdaProducers.Add(production._in);
                    }
                }
            }

            LambdaProducers.AddRange(newLambdaProducers);

            SetStartVariableProduceLambda();
            print("PRODUZ LAMBDA?? " + StartVariableCanProduceLambda);
        }

        private void SetStartVariableProduceLambda()
        {
            var startVariableProductions = (from production in ProductionsPhase1AfterRemoveUnreachable
                where production._in == StartVariable
                select production._out).ToList();
            if (startVariableProductions.Contains("l"))
            {
                StartVariableCanProduceLambda = true;
                return;
            }
            print("================= OLA ===============");
            
            foreach (var production in startVariableProductions)
            {
                print("Production: " + production);
                var possible = true; 
                foreach (var character in production.ToCharArray())
                {
                    // if (!char.IsUpper(character))
                    //     print("Não é upper, pois => " + character);
                    // else 
                    //     print("É upper, pois => " + character);
                    //
                    // if (!LambdaProducers.Contains(character)) 
                    //     print("Não está na lista, pois => " + character);
                    // else
                    //     print("Está na lista, pois => " + character);

                    if ((!char.IsUpper(character) || !LambdaProducers.Contains(character)) && character != StartVariable)
                    {
                        possible = false;  
                        break;
                    }
                }

                if (!possible) continue;
                StartVariableCanProduceLambda = true;
                return;
            }

            StartVariableCanProduceLambda = false; 
            
            
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
                        var variableCount = 0;
                        
                        for (var j = 0; j < characterChain.Length; j++)         // Count how many variations will be produced
                        {
                            if (characterChain[j] == t)
                            {
                                variableCount += 1;
                            }
                        }
                        
                        var variationsTable = BinTableGenerator(variableCount);
                        for (var i = 0; i < math.pow(2, variableCount); i++)
                        {
                            var variationRow = new List<int>();
                            for (var j = 0; j < variableCount; j++)
                            {
                                variationRow.Add(variationsTable[i, j]);
                            }
                            var variation = GenerateVariation(characterChain.ToList(), variationRow.ToArray(), t);
                            var newStringBuilder = new StringBuilder();
                            foreach (var character in variation)
                            {
                                newStringBuilder.Append(character);
                            }

                            if (ProductionsPhase2.Find(x =>
                                    x._in == production._in && x._out == newStringBuilder.ToString()) == null &&
                                newStringBuilder.ToString().Length > 0)
                            { 
                                ProductionsPhase2.Add(new Production(production._in, newStringBuilder.ToString()));
                            }
                        }

                    }
                }
            }
        }
        
        private int[,] BinTableGenerator(int columns)
        {
            var rows = (int) math.pow(2, columns);
            var table = new int[rows, columns];

            var changeNum = rows;
            for (var j = 0; j < columns; j++)
            {
                changeNum /= 2;
                var changeNumCounter = 0;
                var current = true;
                for (var i = 0; i < rows; i++)
                {
                    table[i, j] = current ? 1 : 0;
                    changeNumCounter++;
                    if (changeNumCounter >= changeNum)
                    {
                        current = !current;
                        changeNumCounter = 0;
                    } 
                }
            }
            return table;
        }

        private List<char> GenerateVariation(List<char> charList, int[] positions, char variable)
        {
            var resultingList = charList.DeepClone();
            var onStringPositions = new List<int>();

            for (var i = 0; i < charList.Count; i++)
            {
                if (charList[i] == variable)
                {
                    onStringPositions.Add(i);
                }
            }
            
            for (var i = positions.Length - 1; i >= 0; i--)
            { 
                if (positions[i] == 0)
                {
                    resultingList.RemoveAt(onStringPositions[i]);
                }
            }
            return resultingList;
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
            
            if (StartVariableCanProduceLambda)
                ProductionsPhase2.Add(new Production(StartVariable, "l"));
        }
        
        
        /* Phase 3 (Unit productions) code */
        private void ExecutePhase3()
        {
            RemoveUselessUnitProductions();
            SetUnitProductions();
            Debug.Log("UNIT PRODUCTIONS: ");
            DebugPrintUnitProductions();
            SetNonUnitProductions();
            DebugPrintListProduction(NonUnitProductions, "Non unit productions: ");
            SetResultingProductions();
        }

        private void RemoveUselessUnitProductions()
        {
            ProductionsPhase3 = ProductionsPhase2.DeepClone();
            ProductionsPhase3.RemoveAll(production => production._out.Length == 1 &&
                        production._out.ToCharArray()[0] == production._in);
        }
        
        private void SetUnitProductions()
        {
            foreach (var variable in Variables)
                UnitProductions.Add(new Tuple<char, List<char>>(variable, new List<char>()));

            var unitProductionWasAdd = true;
            while (unitProductionWasAdd)
            {
                unitProductionWasAdd = false;
                foreach (var tuple in UnitProductions)
                {
                    foreach (var production in ProductionsPhase3)
                    {
                        if (tuple.Item1 != production._in) continue;            // If not the same producer
                        if (production._out.Length > 1) continue;               // If production is not a unit, jump
                        if (char.IsUpper(production._out.ToCharArray()[0]))     // If it's a variable
                        {
                            foreach (var variable in tuple.Item2.DeepClone())
                            {
                                foreach (var innerProduction in UnitProductions.Where(innerProduction => innerProduction.Item1 == variable))
                                {
                                    foreach (var innerInnerProduction in innerProduction.Item2)
                                    {
                                        if (!tuple.Item2.Contains(innerInnerProduction) && tuple.Item1 != innerInnerProduction)
                                        {
                                            tuple.Item2.Add(innerInnerProduction);  // Add to the tuple list
                                            unitProductionWasAdd = true;
                                        }
                                        
                                    }
                                }
                                
                            }
                            if (!tuple.Item2.Contains(production._out.ToCharArray()[0]))
                            {
                                tuple.Item2.Add(production._out.ToCharArray()[0]);  // Add to the tuple list
                                unitProductionWasAdd = true;
                            }
                        }
                    }
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

        private void SetNonUnitProductions()
        {
            foreach (var production in ProductionsPhase3)
            {
                if(production._out.Length > 1 || !char.IsUpper(production._out.ToCharArray()[0]))
                    NonUnitProductions.Add(production);
            }
        }
        
        private void SetResultingProductions()
        {
            ResultingProductions = NonUnitProductions.DeepClone();
            
            foreach (var unitProductionTuple in UnitProductions.Where(unitProductionTuple => unitProductionTuple.Item2.Count > 0))
            {
                foreach (var unitProducer in unitProductionTuple.Item2)
                {
                    foreach (var production in NonUnitProductions.Where(production => production._in == unitProducer))              // Para cada elemento do fecho, eu vejo as produções dele
                    {
                        /* Avoid equal productions to be inserted */
                        var isEqual = false; 
                        foreach (var resultingProduction in ResultingProductions)
                        {
                            if (resultingProduction._in == unitProductionTuple.Item1 &&
                                resultingProduction._out == production._out)
                                isEqual = true; 
                        }
                        if(!isEqual) 
                            ResultingProductions.Add(new Production(unitProductionTuple.Item1, production._out));
                    }
                }
            }

        }
        

    }
}
