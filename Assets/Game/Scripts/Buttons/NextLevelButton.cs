using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return false;
    }
}
