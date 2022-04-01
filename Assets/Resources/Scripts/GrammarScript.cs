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
        [NonSerialized] public List<char> UsefulVariablesPhase1;
        [NonSerialized] public List<Production> UselessProductionsPhase1;
        [NonSerialized] public List<Production> UsefulProductionsPhase1;
        [NonSerialized] public List<Tuple<char, List<char>>> ReachableFromPhase1;
        [NonSerialized] public List<Production> usefulAndReachableProductionsPhase1;
        
        
        /* Phase 2 variables */
        [NonSerialized] public List<char> LambdaProducers;
        [NonSerialized] public List<Production> ProductionsPhase2;
        [NonSerialized] public List<char> RemovablePhase2;
        [NonSerialized] public bool StartVariableCanProduceLambda;
        [NonSerialized] public List<Production> ProductionsPhase2WithoutLambdaProductionsAndWithLambdaFromStart;

        /* Phase 3 variables */
        [NonSerialized] public List<Production> NonUselessUnitProductions;
        [NonSerialized] public List<Tuple<char, List<char>>> UnitProductions;
        [NonSerialized] public List<Production> NonUnitProductions;
        [NonSerialized] public List<Production> ProductionsPhase3;
        [NonSerialized] public List<Production> ResultingProductions;
        
        /* Phase 4 variables (bonus) */
        [NonSerialized] public List<char> VariablesPhase4;
        [NonSerialized] public List<char> UsefulVariablesPhase4;
        [NonSerialized] public List<Production> UselessProductionsPhase4;
        [NonSerialized] public List<Production> UsefulProductionsPhase4;
        [NonSerialized] public List<Tuple<char, List<char>>> ReachableFromPhase4;
        [NonSerialized] public List<Production> usefulAndReachableProductionsPhase4;
        
        

        public void Setup()
        {
            InitializeLists();
            ExecutePhase1(Productions, ref UsefulVariablesPhase1, ref UselessProductionsPhase1, ref UsefulProductionsPhase1,
               ref usefulAndReachableProductionsPhase1);
            // DebugPrintListProduction(UsefulProductionsPhase1.OrderBy(x => x._in).ToList(), "Useful productions phase 1: ");
            // DebugPrintListProduction(usefulAndReachableProductionsPhase1.OrderBy(x => x._in).ToList(), "Productions phase 1 after removing unreachable: ");
            
            // Debug.Log(" ============================= PHASE 2 ==================================== ");
            ExecutePhase2();
            // DebugPrintListProduction(ProductionsPhase2.OrderBy(x => x._in).ToList(), "Productions phase 2: ");
            
            // Debug.Log(" ================================ PHASE 3 ================================= ");
            ExecutePhase3();
            // DebugPrintListProduction(ResultingProductions.OrderBy(x => x._in).ToList(), "Productions phase 3 after removing unit: ");
            
            // Debug.Log(" ================================= PHASE 4 ================================ ");
            SetPhase4Variables();
            ExecutePhase1(ResultingProductions.ToArray(), ref UsefulVariablesPhase4, ref UselessProductionsPhase4, ref  UsefulProductionsPhase4, ref usefulAndReachableProductionsPhase4); // Yes, again
            // DebugPrintListProduction(usefulAndReachableProductionsPhase4.OrderBy(x => x._in).ToList(), "Productions phase 4 after removing unreachable: ");

        }

        private void InitializeLists()
        {
            InitializePhase1Lists(UsefulVariablesPhase1, UselessProductionsPhase1, UsefulProductionsPhase1, ReachableFromPhase1, usefulAndReachableProductionsPhase1);
        
            LambdaProducers = new List<char>();
            ProductionsPhase2 = new List<Production>();
            RemovablePhase2 = new List<char>();
            
            UnitProductions = new List<Tuple<char, List<char>>>();
            NonUnitProductions = new List<Production>();
            ProductionsPhase3 = new List<Production>();
            ResultingProductions = new List<Production>();
            
            InitializePhase1Lists(UsefulVariablesPhase4, UselessProductionsPhase4, UsefulProductionsPhase4, ReachableFromPhase4, usefulAndReachableProductionsPhase4);
        }
        
        private void InitializePhase1Lists(List<char> usefulVariables, List<Production> uselessProductions,
            List<Production> usefulProductions, List<Tuple<char, List<char>>> reachableFrom, List<Production> afterRemoveUselessAndUnreachable) {
            usefulVariables = new List<char>();
            uselessProductions = new List<Production>();
            usefulProductions = new List<Production>();
            reachableFrom = new List<Tuple<char, List<char>>>();
            afterRemoveUselessAndUnreachable = new List<Production>();
        }
        
        /* Phase 1 (Useless productions) code */
        private void ExecutePhase1(Production[] givenProductions, ref List<char> usefulVariables, ref List<Production> uselessProductions, ref List<Production> usefulProductions, ref List<Production> usefulAndReachableProductions)
        {
            usefulVariables = GetUsefulVariables(givenProductions);
            // DebugPrintListChar(usefulVariables, "Variáveis ÚTEIS: ");
            uselessProductions = GetRemovableProductions(givenProductions, usefulVariables);
            usefulProductions = GetUsefulProductions(givenProductions, uselessProductions);
            usefulAndReachableProductions = GetUsefulAndReachableProductions(usefulVariables, usefulProductions);
            // DEBUG
            // print("Quantidades!\n Given Productions : " + givenProductions.Length + "\nUseful Variables " +
            //       usefulVariables.Count + "\nUseless Productions: " + uselessProductions.Count +
            //       "\nUsefulProductions: " + usefulProductions.Count + "\nUseful and Reachable variables: " +
            //       usefulAndReachableProductions.Count);
        }

        private List<Char> GetUsefulVariables(Production[] givenProductions)
        {
            var usefulVariables = new List<char>(); 
            foreach (var production in givenProductions)                         // Checking if produce a terminal directly
            {
                if (usefulVariables.Contains(production._in)) continue;
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
                usefulVariables.Add(production._in);
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
                            if (usefulVariables.Contains(productionOutArray[i]))
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

                    if (isUsefulProducer && !usefulVariables.Contains(production._in))
                    {
                        // print("A produção " + production._in + " => " + production._out + " produz variavel util." +
                        //       " Adicionando a lista de variaveis uteis!");
                        usefulVariables.Add(production._in);
                        newUsefulVariableFound = true;
                        break;
                    }
                }
                
                if (newUsefulVariableFound)
                    newUsefulVariableFound = false;
                else
                    reachedEnd = true;
            }

            return usefulVariables;
        }

        private List<Production> GetRemovableProductions(IEnumerable<Production> givenProductions, List<char> usefulVariables)
        {
            return givenProductions.Where(production => !usefulVariables.Contains(production._in)).ToList();
        }

        private List<Production> GetUsefulProductions(IEnumerable<Production> givenProductions, List<Production> removableProductions)
        {
            var usefulProductions = givenProductions.ToList().DeepClone();
            foreach (var removable in removableProductions)
            {
                var foundProduction = usefulProductions.Find(production => production._in == removable._in);
                usefulProductions.Remove(foundProduction);
            }

            return usefulProductions;
        }

        private List<Production> GetUsefulAndReachableProductions(List<char> usefulVariables, List<Production> usefulProductions)
        {
            var usefulAndReachableProductions = new List<Production>();
            var reachableFrom = new List<Tuple<char, List<char>>>();
            foreach (var usefulVariable in usefulVariables)     // Add direct reachable list
            {
                var reachableVariables = new List<char>();
                foreach (var production in usefulProductions.Where(production => production._in == usefulVariable))
                {
                    var productionOutCharArray = production._out.ToCharArray();
                    for (var i = 0; i < productionOutCharArray.Length; i++)
                        if (char.IsUpper(productionOutCharArray[i]))
                            if(!reachableVariables.Contains(productionOutCharArray[i])) 
                                reachableVariables.Add(productionOutCharArray[i]);
                }
                reachableFrom.Add(new Tuple<char, List<char>>(usefulVariable, reachableVariables));
            }
            
            // foreach (var reachable in ReachableFrom)
            // {
            //     DebugPrintListChar(reachable.Item2, "1st iteration: Reachable from " + reachable.Item1 + ": ");
            // }

            var reachedEnd = false;
            while (!reachedEnd)
            {
                var newReachableAdded = false;
                foreach (var usefulVariable in usefulVariables)     // For each variable on UsefulVariables
                {
                    // print("TRABALHANDO COM A VARIAVEL " + usefulVariable + "!!");
                    var currentTuple = reachableFrom.Find(reachable => reachable.Item1 == usefulVariable); // Been working useful variable
                    foreach (var reachable in currentTuple.Item2)         // For each variable reachable from the current useful variable
                    {
                        // print("Olhando a variavel alcançável por " + usefulVariable + ": " + reachable);
                        var reachableListPreviousSize = currentTuple.Item2.Count;
                        // print("Numero de alcancáveis por " + usefulVariable + " antes de adicionar algo: " + reachableListPreviousSize);

                        var currentInsideTuple = reachableFrom.Find(x => x.Item1 == reachable);
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
            
            
            usefulAndReachableProductions = usefulProductions.ToList().DeepClone();

            foreach (var production in usefulProductions)
            {
                if (!reachableFrom.Find(x => x.Item1 == StartVariable).Item2.Contains(production._in))
                {
                    var foundVariable =
                        usefulAndReachableProductions.Find(x =>
                            x._in == production._in && x._out == production._out);
                    if (foundVariable._in == StartVariable) continue; // Make sure that the starting variable won't be excluded
                    usefulAndReachableProductions.Remove(foundVariable);
                }
            } 
            
            // DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable items: ");

            var auxProductionsPhase1 = usefulAndReachableProductions.DeepClone();
            foreach (var production in auxProductionsPhase1)
            {
                for (var i = 0; i < production._out.Length; i++)
                {
                    if (char.IsUpper((production._out[i])))
                    {
                        if (!usefulVariables.Contains(production._out[i]))
                        {
                            usefulAndReachableProductions.Remove(usefulAndReachableProductions.Find(productionX =>
                                productionX._in == production._in && productionX._out == production._out));
                            break;
                        }
                    }
                }
            }
            // DebugPrintListProduction(ProductionsPhase1AfterRemoveUnreachable, "After remove unreachable variables: ");
            return usefulAndReachableProductions;
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
            // DebugPrintListChar(LambdaProducers, "Lambda producers: ");
            SetInsertablePhase2();
            // DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after inserting new productions: ");
            SetRemovablePhase2();
            ProductionsPhase2WithoutLambdaProductionsAndWithLambdaFromStart = RemoveLambdaProductionsFromPhase2AndAddLambdaFromStart();
            // DebugPrintListProduction(ProductionsPhase2, "Phase 2 productions after removing");
        }

    
        private void SetLambdaProducers()
        {
            LambdaProducers = (from production in usefulAndReachableProductionsPhase1
                where production._out[0] == 'λ' && !LambdaProducers.Contains(production._in)
                select production._in).ToList();
            var newLambdaProducers = new List<char>();
            // Discover Variables that produces lambda productions
            foreach (var lambdaProducer in LambdaProducers)
            {
                foreach (var production in usefulAndReachableProductionsPhase1)
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
            // print("PRODUZ LAMBDA?? " + StartVariableCanProduceLambda);
        }

        private void SetStartVariableProduceLambda()
        {
            var startVariableProductions = (from production in usefulAndReachableProductionsPhase1
                where production._in == StartVariable
                select production._out).ToList();
            if (startVariableProductions.Contains("λ"))
            {
                StartVariableCanProduceLambda = true;
                return;
            }
            // print("================= OLA ===============");
            
            foreach (var production in startVariableProductions)
            {
                // print("Production: " + production);
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
            ProductionsPhase2 = usefulAndReachableProductionsPhase1.DeepClone();
            
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
            foreach (var production in ProductionsPhase2.Where(production => production._out == "λ"))
            {
                RemovablePhase2.Add(production._in);
            }
        }

        private List<Production> RemoveLambdaProductionsFromPhase2AndAddLambdaFromStart()
        {
            if (RemovablePhase2.Count < 1) return ProductionsPhase2;
            var removeList = ProductionsPhase2.Where(production => production._out == "λ").ToList();
            foreach (var removable in removeList)
            {
                ProductionsPhase2.Remove(removable);
            }
            
            if (StartVariableCanProduceLambda)
                ProductionsPhase2.Add(new Production(StartVariable, "λ"));
            return ProductionsPhase2.DeepClone();
        }
        
        
        /* Phase 3 (Unit productions) code */
        private void ExecutePhase3()
        {
            NonUselessUnitProductions = RemoveUselessUnitProductions();
            
            // deve retornar uma lista de producoes!
            SetUnitProductions();
            // Debug.Log("UNIT PRODUCTIONS: ");
            // DebugPrintUnitProductions();
            SetNonUnitProductions();
            // DebugPrintListProduction(NonUnitProductions, "Non unit productions: ");
            SetResultingProductions();
        }

        private List<Production> RemoveUselessUnitProductions()
        {
            ProductionsPhase3 = ProductionsPhase2.DeepClone();
            ProductionsPhase3.RemoveAll(production => production._out.Length == 1 &&
                        production._out.ToCharArray()[0] == production._in);
            return ProductionsPhase3.DeepClone();
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

        private void SetPhase4Variables()
        {
            VariablesPhase4 = new List<char>();
            foreach (var production in ResultingProductions.Where(production => !VariablesPhase4.Contains(production._in)))
            {
                VariablesPhase4.Add(production._in);
            }
        }
    }
}
