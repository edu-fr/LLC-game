using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroductionManager : MonoBehaviour
{
    public GameObject Footer;
    public TextMeshProUGUI content1, content2;
    private bool WaitingForKey;
    private int currentContent;
    
    private void Update()
    {
        if (WaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                WaitingForKey = false;
                NextContent();
            }
        }
    }

    private void OnEnable()
    {
        currentContent = 1;
        content1.gameObject.SetActive(true);
        content2.gameObject.SetActive(false);
        Footer.SetActive(false);
        WaitingForKey = false;
        StartCoroutine(ShowFooter());
    }

    private IEnumerator ShowFooter()
    {
        yield return new WaitForSeconds(2f);
        Footer.SetActive(true);
        WaitingForKey = true;
    }

    private void NextContent()
    {
        if (currentContent == 1)
        {
            content1.gameObject.SetActive(false);
            content2.gameObject.SetActive(true);
            currentContent++;
        }
        else
        {
            gameObject.SetActive(false);
            return; 
        }
        
        Footer.SetActive(false);
        StartCoroutine(ShowFooter());
    }

}
