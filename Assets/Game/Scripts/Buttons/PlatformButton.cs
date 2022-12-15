using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }
}
