using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private SoundManager _soundManager;
    public Slider sliderSFX;
    public Slider sliderBGM;
    public Toggle tutorialsToggle;

    private void Awake()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    private void OnEnable()
    {
        sliderBGM.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sliderSFX.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        tutorialsToggle.isOn = PlayerPrefs.GetInt("showTutorials", 1) != 0;
    }

    public void UpdateBGMVolume()
    {
        _soundManager.SetBGMVolume(sliderBGM.value);        
        PlayerPrefs.SetFloat("BGMVolume", sliderBGM.value);
        PlayerPrefs.Save();
    }
    
    public void UpdateSFXVolume()
    {
        _soundManager.SetSFXVolume(sliderSFX.value);        
        PlayerPrefs.SetFloat("SFXVolume", sliderSFX.value);
        PlayerPrefs.Save();
    }

    public void UpdateTutorialToggle()
    {
        PlayerPrefs.SetInt("showTutorials", tutorialsToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
