using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    
    public GameObject SettingsPanel;
    public Sprite[] MutesSprites;
    public Sprite[] TapticSprites;
    public Image muteImage;
    public Image tapticımage;
    bool mute = false;
    bool taptic = false;

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
}
