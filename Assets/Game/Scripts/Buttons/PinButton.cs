using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return GameManager.Instance.GetCanAddPins();
    }

    public int ControlValue()
    {
        return GameManager.Instance.platform.activePins.Count;
    }

    public int MaxLimit()
    {
        return GameManager.Instance.GetMaxPinCount();
    }
}
