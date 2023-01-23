using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallButton : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return GameManager.Instance.GetCanAddBall();
    }

    public int ControlValue()
    {
        return GameManager.Instance.ActiveBalls.Count;
    }

    public int MaxLimit()
    {
        return GameManager.Instance.GetMaxBallCount();
    }
}
