using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public SoundManager soundManager;
    public Slider sliderSFX;
    public Slider sliderBGM;

    public void UpdateBGMVolume()
    {
        soundManager.SetBGMVolume(sliderBGM.value);        
    }
    
    public void UpdateSFXVolume()
    {
        soundManager.SetSFXVolume(sliderSFX.value);        
    }
}
