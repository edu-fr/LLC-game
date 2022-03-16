using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources.Scripts;
using TMPro;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class CanvasController : MonoBehaviour
{
    public LevelScript levelScript;
    public CanvasRenderer transitionPanel;
    public TextMeshProUGUI transitionPanelContentText;
    public TextMeshProUGUI transitionPanelFooterText;
    private bool _waitingForKey = false;
    
    public string[] phaseTexts;
    public GameObject tutorial_1_1;
    public GameObject tutorial_1_2;
    public GameObject tutorial_1_3;
    public GameObject tutorial_2_1;
    public GameObject tutorial_2_2;
    public GameObject tutorial_2_3;
    public GameObject tutorial_3_1;
    public GameObject tutorial_3_2;
    public GameObject tutorial_3_3;
    public GameObject tutorial_4_1;
    public GameObject tutorial_4_2;
    public GameObject tutorial_4_3;

    private void Update()
    {
        if (!_waitingForKey) return;
        if (Input.anyKeyDown)
        {
            transitionPanel.gameObject.SetActive(false);
            _waitingForKey = false;
        }
    }
    
    public void Transition(int phase)
    {
        transitionPanelFooterText.gameObject.SetActive(false);
        transitionPanelContentText.SetText(phaseTexts[phase - 1]);
        transitionPanel.gameObject.SetActive(true);
        StartCoroutine(WaitToShowFooter());
    }

    private IEnumerator WaitToShowFooter()
    {
        yield return new WaitForSeconds(1.5f);
        transitionPanelFooterText.gameObject.SetActive(true);
        _waitingForKey = true; 
    }

    public void ActivateTutorial(int phase, int part)
    {
        switch (phase)
        {
            case 1:
                switch (part)
                {
                    case 1:
                        tutorial_1_1.SetActive(true);
                        break;
                    
                    case 2:
                        tutorial_1_2.SetActive(true);
                        break;
                    
                    case 3:
                        tutorial_1_3.SetActive(true);
                        break;
                }

                break;
            
            case 2:
                switch (part)
                {
                    case 1:
                        tutorial_2_1.SetActive(true);
                        break;
                    
                    case 2:
                        tutorial_2_2.SetActive(true);
                        break;
                    
                    case 3:
                        tutorial_2_3.SetActive(true);
                        break;
                }

                break;

            case 3:
                switch (part)
                {
                    case 1:
                        tutorial_3_1.SetActive(true);
                        break;
                    
                    case 2:
                        tutorial_3_2.SetActive(true);
                        break;
                    
                    case 3:
                        tutorial_3_3.SetActive(true);
                        break;
                }

                break;

            case 4:
                switch (part)
                {
                    case 1:
                        tutorial_4_1.SetActive(true);
                        break;
                    
                    case 2:
                        tutorial_4_2.SetActive(true);
                        break;
                    
                    case 3:
                        tutorial_4_3.SetActive(true);
                        break;
                }

                break;
        }
    }
}
