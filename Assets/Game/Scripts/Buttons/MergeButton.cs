using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }
}
