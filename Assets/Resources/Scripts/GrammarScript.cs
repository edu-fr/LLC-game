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
        [NonSerialized] public List<char> LambdaProducers;
        [NonSerialized] public List<Production> ProductionsPhase1;
        [NonSerialized] public List<char> RemovablePhase1;

        /* Phase 2 variables */
        [NonSerialized] public List<Tuple<char, List<char>>> UnitProductions;
        [NonSerialized] public List<Production> ProductionsPhase2;
        
        
        /* Phase 3 variables */

        public void Setup()
        {
            InitializeLists();
            ExecutePhase1();
            ExecutePhase2();
        }
        
        /* Phase 1 code */
        private void ExecutePhase1()
        {
            SetLambdaProducers(); 
            DebugPrintLambdaProducers();
            SetInsertablePhase1();
            DebugPrintPhaseProductions(ProductionsPhase1, 1);
            SetRemovablePhase1();
            RemoveLambdaProductionsFromPhase1();
            DebugPrintPhaseProductions(ProductionsPhase1, 1);
        }

        private void InitializeLists()
        {
            LambdaProducers = new List<char>();
            ProductionsPhase1 = new List<Production>();
            RemovablePhase1 = new List<char>();
            UnitProductions = new List<Tuple<char, List<char>>>();
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
                    if (production._out[0] == lambdaProducer)
                    {
                        newLambdaProducers.Add(production._in);
                    }
                }
            }

            LambdaProducers.AddRange(newLambdaProducers);
        }

        private void SetInsertablePhase1()
        { 
            /* Add all productions to the new list */
            foreach (var production in Productions)
            {
                ProductionsPhase1.Add(production);
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
                        
                        for (var j = 0; j < characterChain.Length; j++)         // Count how many variations we be produced
                        {
                            if (characterChain[j] == t)
                            {
                                variableCount += 1;
                            }
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
                        ProductionsPhase1.Add(new Production(production._in, newString.ToString()));
                    }
                }
            }
        }
        
        private void SetRemovablePhase1()
        {
            foreach (var production in ProductionsPhase1.Where(production => production._out == "l"))
            {
                RemovablePhase1.Add(production._in);
            }
        }

        private void RemoveLambdaProductionsFromPhase1()
        {
            if (RemovablePhase1.Count < 1) return;
            var removeList = ProductionsPhase1.Where(production => production._out == "l").ToList();
            foreach (var removable in removeList)
            {
                ProductionsPhase1.Remove(removable);
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
            foreach (var removable in RemovablePhase1)
            {
                print(removable);
            }
        }
        
        /* Phase 2 code */
        private void ExecutePhase2()
        {
            SetUnitProductions();
            DebugPrintUnitProductions();
            SetInsertablePhase2();
            
            print("PHASE 2 FINAL TEST: ");
            print("Phase 1: ");
            DebugPrintPhaseProductions(ProductionsPhase1, 1);
            
            print("Phase 2: ");
            DebugPrintPhaseProductions(ProductionsPhase2, 2);
        }
        
        private void SetUnitProductions()
        {
            foreach (var variable in Variables)
                UnitProductions.Add(new Tuple<char, List<char>>(variable, new List<char>()));

            foreach (var tuple in UnitProductions)
            {
                foreach (var production in ProductionsPhase1)
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
        
        private void SetInsertablePhase2()
        {
            ProductionsPhase2 = ProductionsPhase1.DeepClone();
            foreach (var tuple in UnitProductions.Where(tuple => tuple.Item2.Count > 0))
            {
                foreach (var producer in tuple.Item2)
                {
                    foreach (var production in ProductionsPhase1.Where(production => production._in == producer))
                    {
                        ProductionsPhase2.Add(new Production(tuple.Item1, production._out));
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
