using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInputs : MonoBehaviour
{
    public GameObject UIObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPause()
    {
        if (UIObject)
            GameObject.Instantiate(UIObject);
    }
}
