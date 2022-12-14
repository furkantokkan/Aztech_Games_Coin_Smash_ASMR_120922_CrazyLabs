using Cinemachine;
using UnityEngine;
public class Cameras : MonoBehaviour
{
    public GameObject firstCamera;
    public GameObject secondCamera;
    public float speed;
    public Animator anim;
    private bool _state = true;
   
    // Update is called once per frame
    void Update()
    {
        if(_state) FCam();
        else SCam();
    }

    void FCam()
    {
        firstCamera.transform.Rotate(0, InputManager.Instance.GetDirection().x * -speed, 0);
    }

    void SCam()
    {
        secondCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView += InputManager.Instance.GetDirection().y * speed;
    }
    
    public void ClickFirstCam()
    {
        anim.Play("FirstCam");
        secondCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
        _state = true;
    }
    public void ClickSecondCam()
    {
        anim.Play("SecondCam");
        _state = false;
    }
}
