using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject DebugPanel;
    bool mute = false;
    
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
        mute = !mute;
        AudioListener.pause = !mute;
    }

    public void Taptic()
    {
        
    }
}
