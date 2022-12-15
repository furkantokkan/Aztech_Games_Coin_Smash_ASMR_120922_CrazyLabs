using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }
}
