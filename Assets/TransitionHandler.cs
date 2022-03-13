using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources.Scripts;
using TMPro;
using UnityEngine;

public class TransitionHandler : MonoBehaviour
{
    public LevelScript levelScript;
    public CanvasRenderer transitionPanel;
    public TextMeshProUGUI transitionPanelContentText;
    public TextMeshProUGUI transitionPanelFooterText;
    private bool _waitingForKey = false;
    
    public string[] phaseTexts;

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
}
