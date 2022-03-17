using UnityEngine;

namespace Resources.Scripts
{
    public class BoxContent : MonoBehaviour
    {
        public char Variable { get; private set; } = '´';
        public GrammarScript.Production Production { get; private set; } = null;

        public void SetVariable(char newVariable)
        {
            Variable = newVariable;
            Production = null;
        }
        
        public void SetProduction(GrammarScript.Production production)
        {
            Production = production;
            Variable = '´';
        }
    }
}
