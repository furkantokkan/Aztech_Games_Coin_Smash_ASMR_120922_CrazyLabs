using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return GameManager.Instance.GetCanMergeBalls();
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
