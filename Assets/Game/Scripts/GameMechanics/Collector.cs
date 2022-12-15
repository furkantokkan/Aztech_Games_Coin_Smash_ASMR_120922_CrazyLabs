using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public int value;
    void Update()
    {
        if (UIManager.Instance.material)
            GetComponent<Renderer>().material = UIManager.Instance.FirstMaterials[value];
        else GetComponent<Renderer>().material = UIManager.Instance.SecondMaterials[value];
    }
}
