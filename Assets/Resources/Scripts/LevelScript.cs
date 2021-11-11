using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.Scripts
{
    public class LevelScript : MonoBehaviour
    {
        public GameObject grammarObject;
        private GrammarScript _grammar;
        private List<char> _lambdaProducers = new List<char>();

        // Start is called before the first frame update
        void Start()
        {
            // Get Current grammar
            _grammar = grammarObject.GetComponent<GrammarScript>();
            // Analyse productions
            _grammar.Setup();


            // Discover productions that could be removed on phase 1

            // Discover productions that need to me inserted on phase 1

            // Discover productions that could be removed on phase 2
            // Discover productions that need to me inserted on phase 2
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * switch(phase) {
             *
             *  phase1.1: // Definicao do conjunto vazio
             *      permitir que o player mova as variáveis que produzem vazio para os espaços corretos
             *      espera o player 
             *  
             *  phase1.2: // Produções vazias
             *      permitir que o player crie novas produções (produções adicionais para remover as vazias)
             *      permitir que o player exclua produções (produçÕes que produzem lambda)
             *      espera que o player clique em concluir etapa (ou o tempo acabe)
             * 
             *  phase2.1: // Definicao dos fechos
             *      permitir que o player mova as variaveis solitarias que sao produzidas por cada variavel
             *      espera que o player clique em concluir etapa
             *
             *  phase2.2: // Substituir produções unitárias
             *      permitir que o player exclua producoes unitarias
             *      permitir que o player introduza outras producoes no lugar dessas unitarias
             *      espera que o player clique em concluir etapa
             * 
             *  phase3: // Excluir produções inuteis
             *      permite que o player jogue fora produções inúteis
             *      espera o player clicar em concluir etapa
             * 
             * }
             */
        }
    }
}
