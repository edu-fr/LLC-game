using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Resources.Scripts
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class BoxPositionsManager : MonoBehaviour
    {
        // ANCHORS
        public Transform Anchor_OutOfBounds;

        // Phase 1 Part 1
        public Transform Anchor_Phase1Part1_ProductionsBox_gray;
        public Transform Anchor_Phase1Part1_variablesBox;
        public Transform Anchor_Phase1Part1_usefulVariablesBox;
        public Transform Anchor_Phase1Part1_uselessVariablesBox;
    
        // Phase 1 Part 2
        public Transform Anchor_Phase1Part2_ProductionsBox;
        public Transform Anchor_Phase1Part2_uselessVariablesBox_gray;
        public Transform Anchor_Phase1Part2_trashBin;
    
        // Phase 1 Part 3
        public Transform Anchor_Phase1Part3_Productions_Box;
        public Transform Anchor_Phase1Part3_uselessVariablesBox_gray;
        public Transform Anchor_Phase1Part3_trashBin;
    
        // Phase 2 Part 1
        public Transform Anchor_Phase2Part1_productionsBox; 
        public Transform Anchor_Phase2Part1_variablesBox;
        public Transform Anchor_Phase2Part1_lambdaProducersBox; 
    }
}
