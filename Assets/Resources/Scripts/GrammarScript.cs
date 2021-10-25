using System;
using UnityEngine;

namespace Resources.Scripts
{
    public class GrammarScript : MonoBehaviour
    {
        [Serializable()]
        public class Production
        {
            public string _in;
            public string _out;
        }
        
        public char[] Terminals;
        public char[] Variables;
        public Production[] Productions;
        public char StartVariable;

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
