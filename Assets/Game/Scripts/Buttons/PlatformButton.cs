using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public int ControlValue()
    {
        return -1;
    }

    public int MaxLimit()
    {
        return -1;
    }
}
