using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreen : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            float cameraDistance = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (transform.position.y > 10 || transform.position.y < -20 ||cameraDistance <= 12f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
