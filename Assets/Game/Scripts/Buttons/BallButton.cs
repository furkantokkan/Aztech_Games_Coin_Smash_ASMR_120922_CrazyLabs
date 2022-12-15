using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return GameManager.Instance.GetCanAddBall();
    }
}
