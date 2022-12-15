using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    
    public GameObject SettingsPanel;
    public GameObject DebugPanel;
    public Sprite[] MutesSprites;
    public Sprite[] TapticSprites;
    public Image muteImage;
    public Image tapticımage;
    public Material[] FirstMaterials;
    public Material[] SecondMaterials;
    bool mute = false;
    bool taptic = false;
    [HideInInspector]
    public bool material = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void Settings()
    {
        if(SettingsPanel.activeInHierarchy)
            SettingsPanel.SetActive(false);
        else SettingsPanel.SetActive(true);
    }

    public void Debug()
    {
        if(DebugPanel.activeInHierarchy)
            DebugPanel.SetActive(false);
        else DebugPanel.SetActive(true);
    }
    
    public void Mute()
    {
        muteImage.sprite = IconChanger(MutesSprites[0], MutesSprites[1], mute);
        mute = !mute;
        AudioListener.pause = !mute;
    }

    public void Taptic()
    {
        tapticımage.sprite = IconChanger(TapticSprites[0], TapticSprites[1], taptic);
        taptic = !taptic;
    }
    Sprite IconChanger(Sprite first, Sprite second,bool state)
    {
        return state ? first : second;
    }
    
    public void FirstMaterial()
    {
        material = false;
    }
    public void SecondMaterial()
    {
        material = true;
    }
}
