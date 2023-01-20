using UnityEngine;
public class Cameras : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Rotate(0, InputManager.Instance.GetDirection().x * -speed, 0);
    }

}
