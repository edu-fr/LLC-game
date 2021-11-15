using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

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
        
        
        /* Phase 2 variables */
        [NonSerialized] public List<char> LambdaProducers;
        [NonSerialized] public List<Production> ProductionsPhase2;
        [NonSerialized] public List<char> RemovablePhase2;

        /* Phase 3 variables */
        [NonSerialized] public List<Tuple<char, List<char>>> UnitProductions;
        [NonSerialized] public List<Production> ProductionsPhase3;
        
        

        public void Setup()
        {
            InitializeLists();
            ExecutePhase1();
            // ExecutePhase2();
            // ExecutePhase3();
        }

        private void InitializeLists()
        {
            UsefulVariables = new List<char>();
            RemovablePhase1 = new List<Production>();
            ProductionsPhase1 = new List<Production>();
            
            LambdaProducers = new List<char>();
            ProductionsPhase2 = new List<Production>();
            RemovablePhase2 = new List<char>();
            
            UnitProductions = new List<Tuple<char, List<char>>>();
        }

        
        /* Phase 1 (Useless productions) code */
        private void ExecutePhase1()
        {
            SetUsefulVariables();
            SetRemovablePhase1();
            SetProductionsPhase1();
            
            // TO DO: Remove unreachable variables
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

        private void DebugPrintUsefulVariables()
        {
            if (UsefulVariables.Count < 1) return;
            print("Useful variables: ");
            foreach (var usefulVariable in UsefulVariables)
            {
                print(usefulVariable);
            }
        }
        
        
        /* Phase 2 (Empty productions) code */
        private void ExecutePhase2()
        {
            SetLambdaProducers(); 
            DebugPrintLambdaProducers();
            SetInsertablePhase2();
            DebugPrintPhaseProductions(ProductionsPhase2, 1);
            SetRemovablePhase2();
            RemoveLambdaProductionsFromPhase2();
            DebugPrintPhaseProductions(ProductionsPhase2, 1);
        }

    
        private void SetLambdaProducers()
        {
            LambdaProducers = (from production in Productions where production._out[0] == 'l' select production._in).ToList();
            var newLambdaProducers = new List<char>();
            // Discover Variables that produces lambda productions
            foreach (var lambdaProducer in LambdaProducers)
            {
                foreach (var production in Productions)
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
            /* Add all productions to the new list */
            foreach (var production in Productions)
            {
                ProductionsPhase2.Add(production);
            }
            
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

        private void DebugPrintPhaseProductions(List<Production> phaseProductions, int phaseNumber)
        {
            print("Productions phase " + phaseNumber + ":");
            foreach (var production in phaseProductions)
            {
                print(production._in + " => " + production._out);
            }
        }

        private void DebugPrintLambdaProducers()
        {
            if (LambdaProducers.Count <= 0) return;
            print("Lambda producers: ");
            foreach (var producer in LambdaProducers)
            {
                print(producer);
            }   
        }

        private void DebugPrintPhaseRemovables()
        {
            if (LambdaProducers.Count <= 0) return;
            print("Phase removables: ");
            foreach (var removable in RemovablePhase2)
            {
                print(removable);
            }
        }
        
        /* Phase 3 (Unit productions) code */
        private void ExecutePhase3()
        {
            SetUnitProductions();
            DebugPrintUnitProductions();
            SetInsertablePhase3();
            
            print("PHASE 3 FINAL TEST: ");
            print("Phase 2: ");
            DebugPrintPhaseProductions(ProductionsPhase2, 1);
            
            print("Phase 3: ");
            DebugPrintPhaseProductions(ProductionsPhase3, 2);
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
                    if (production._out.Length > 1) continue;               // If production is not a unit
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
        
        public Production[] GetRemovablePhase3()
        {
            
            return null; 
        }

    }
}
