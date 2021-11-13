using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInputs : MonoBehaviour
{
    public GameObject UIObject;
    GameObject spawnedPauseMenu;
    bool isPaused = false;

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
        isPaused = !isPaused;
        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (UIObject)
                spawnedPauseMenu = GameObject.Instantiate(UIObject);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (spawnedPauseMenu)
                GameObject.Destroy(spawnedPauseMenu);
        }
    }
}
