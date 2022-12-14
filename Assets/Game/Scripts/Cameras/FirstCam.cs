using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FirstCam : MonoBehaviour
{
    public GameObject FirstCamera;
    public GameObject SecondCamera;
    public float speed;
    public Animator anim;
    private bool state = true;
   
    // Update is called once per frame
    void Update()
    {
        if(state)
            FCam();
        else SCam();
    }

    void FCam()
    {
        if(Input.touchCount > 0)
        {
            float x = Input.GetTouch(0).deltaPosition.x;
            FirstCamera.transform.Rotate(0, x * Time.deltaTime * speed, 0);
        }
    }

    void SCam()
    {
        if(Input.touchCount > 0)
        {
            float y = Input.GetTouch(0).deltaPosition.y;
            SecondCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView += y * Time.deltaTime;
        }
    }
    
    public void ClickFirstCam()
    {
        anim.Play("FirstCam");
        state = true;
    }
    public void ClickSecondCam()
    {
        anim.Play("SecondCam");
        state = false;
    }
}
