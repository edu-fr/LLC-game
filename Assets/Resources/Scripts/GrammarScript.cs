using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [NonSerialized] public List<char> LambdaProducers;
        [NonSerialized] public List<Production> ProductionsPhase1;

        public void Setup()
        {
            SetLambdaProducers();
            SetInsertablePhase1();
            
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
            
            /* Debug print */
            if (LambdaProducers.Count <= 0) return;
            print("Lambda producers: ");
            foreach (var producer in LambdaProducers)
            {
                print(producer);
            }
        }

        private void SetInsertablePhase1()
        {
            ProductionsPhase1 = new List<Production>();
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
                        for (var j = 0; j < characterChain.Length; j++)
                        {
                            if (characterChain[j] == t) continue;
                            newString.Append(characterChain[j]);
                        }
                        ProductionsPhase1.Add(new Production(production._in, newString.ToString()));
                    }
                }
            }

            print("Productions phase 1:");
            foreach (var production in ProductionsPhase1)
            {
                print(production._in + " => " + production._out);
            }
            
        }
        
        public Production[] GetRemovablePhase1()
        {
            
            return null; 
        }
        
        public Production[] GetInsertablePhase1()
        {
            
            return null; 
        }

        public Production[] GetRemovablePhase2()
        {
            
            return null; 
        }
        
        public Production[] GetInsertablePhase2()
        {
            
            return null; 
        }
        
        
        public Production[] GetRemovablePhase3()
        {
            
            return null; 
        }


      
    }
}
