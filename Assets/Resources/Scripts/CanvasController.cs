using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Resources.Scripts;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class CanvasController : MonoBehaviour
{
    public LevelScript levelScript;
    private LevelSelectController levelSelectControllerReference;
    
    public CanvasRenderer tryAgainPanel;
    public CanvasRenderer youWinPanel;
    public CanvasRenderer transitionPanel;
    public CanvasRenderer pausePanel;
    public CanvasRenderer windowPanel;
    
    public TextMeshProUGUI transitionPanelContentText;
    public TextMeshProUGUI transitionPanelFooterText;
    private bool _waitingForKey = false;
    public float timeToShowFooter;

    public TextMeshProUGUI HUDTime; 
    public TextMeshProUGUI HUDLife; 
    
    [SerializeField] internal float initialTime;
    [SerializeField] internal float remainingTime;
    [SerializeField] internal int initialLife;
    [SerializeField] internal int remainingLife;
    
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

    public GameObject mistakeHelpModal;
    
    public GameObject resetProductionBox_f1p2;
    public GameObject resetProductionBox_f1p3;
    public GameObject resetProductionBox_f2p2;
    public GameObject resetProductionBox_f3p1;
    public GameObject resetProductionBox_f3p3;
    public GameObject resetProductionBox_f4p2;
    public GameObject resetProductionBox_f4p3;

    
    
    private void Start()
    {
        levelSelectControllerReference = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelSelectController>();
        remainingLife = initialLife;
        remainingTime = initialTime;
    }
    private void Update()
    {
        UpdateHUD();
        
        if (!_waitingForKey) return;
        if (Input.anyKeyDown)
        {
            transitionPanel.gameObject.SetActive(false);
            _waitingForKey = false;
            ActivateTutorial(levelScript.currentPhase, levelScript.currentPart);
        }
    }

    internal void UpdateTime()
    {
        remainingTime = remainingTime <= 0 ? 0 : remainingTime -= Time.deltaTime;
    }

    private void UpdateHUD()
    {
        HUDTime.SetText(((int) remainingTime).ToString(CultureInfo.CurrentCulture));
        var currentLife = new List<char>();
        for (var i = 0; i < remainingLife; i++)
        {
            currentLife.Add('*');
        }
        var currentLifeString = currentLife.ToArray().ArrayToString();
        HUDLife.SetText(currentLifeString);
    }

    public void PlayerMistake()
    {
        SoundManager.instance.Play("Player mistake");
        remainingLife -= 1;
    }
    
    public void Transition(int phase)
    {
        levelScript.ChangeGameState(LevelScript.GameState.PopUp);
        transitionPanelFooterText.gameObject.SetActive(false);
        transitionPanelContentText.SetText(phaseTexts[phase - 1]);
        transitionPanel.gameObject.SetActive(true);
        StartCoroutine(WaitToShowFooter());
    }

    private IEnumerator WaitToShowFooter()
    {
        yield return new WaitForSeconds(timeToShowFooter);
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

    public void ShowPausePanel(bool boolean)
    {
        pausePanel.gameObject.SetActive(boolean);
    }

    public void ConfigureAndCallHelpModal(string title, string message)
    {
        mistakeHelpModal.GetComponent<WindowTrigger>().title = title;
        mistakeHelpModal.GetComponent<WindowTrigger>().message = message;
        mistakeHelpModal.SetActive(true);
        
    }

    public void OpenTryAgainScreen()
    {
        transitionPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
        windowPanel.gameObject.SetActive(false);
        youWinPanel.gameObject.SetActive(false);
        tryAgainPanel.gameObject.SetActive(true);
    }

    public void OpenYouWinScreen()
    {
        transitionPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
        windowPanel.gameObject.SetActive(false);
        tryAgainPanel.gameObject.SetActive(false);
        youWinPanel.gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        levelSelectControllerReference.ResetLevel();
    }

    public void GoToMainMenu()
    {
        levelSelectControllerReference.LoadMenu();
    }

    public void GoToNextLevel()
    {
        levelSelectControllerReference.NextLevel();
    }

    public void ActivateCurrentTutorial()
    {
        ActivateTutorial(levelScript.currentPhase, levelScript.currentPart);
    }

}
