using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI revenueText;
    bool mute = false;
    bool taptic = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        GameManager.Instance.updateUI += UpdateRevenueText;
        BallMovement.onBallsStartToMove += UpdateRevenueText;
    }
    private void OnDisable()
    {
        GameManager.Instance.updateUI -= UpdateRevenueText;
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
    public void UpdateRevenueText()
    {
        StartCoroutine(CalculateRevenuePerSec());
    }
    private IEnumerator CalculateRevenuePerSec()
    {
        float revenue = 0f;
        int pinAmount = GameManager.Instance.platform.activePins.Count + 2;

        yield return new WaitForEndOfFrame();

        if (GameManager.Instance.ActiveBalls.Count > 0)
        {
            for (int i = 0; i < GameManager.Instance.ActiveBalls.Count; i++)
            {
                PoolItems poolItems = GameManager.Instance.ActiveBalls[i].GetComponent<PoolElement>().value;
                int index = Array.IndexOf(Enum.GetValues(poolItems.GetType()), poolItems) + 1;
                revenue += index;
                yield return null;
            }
        }

        revenue *= pinAmount;

        revenueText.text = "$" + MinimizeTheNumbers(revenue) + " /sec";

        if (revenue > 0f)
        {
            BallMovement.onBallsStartToMove -= UpdateRevenueText;   
        }
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
    public string MinimizeTheNumbers(float score)
    {
        if (score > 100000) return (score / 10000).ToString("F") + "B";
        else if (score > 10000) return (score / 10000).ToString("F") + "M";
        else if (score > 1000) return (score / 1000).ToString("F") + "K";
        else return score.ToString();
    }
}
