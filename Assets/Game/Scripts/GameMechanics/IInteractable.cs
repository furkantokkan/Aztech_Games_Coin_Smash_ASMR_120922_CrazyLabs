using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool CanInteract();

    int MaxLimit();

    int ControlValue();
}
