using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInputs : MonoBehaviour
{
    public GameObject PauseUIObject;
    public GameObject IngameUIObject;
    GameObject spawnedPauseMenu;
    GameObject spawnedIngameUI;
    bool isPaused = false;

    void OnEnable()
    {
        if (IngameUIObject)
            spawnedIngameUI = Instantiate(IngameUIObject);
    }

    private void OnDisable()
    {
        if (spawnedIngameUI)
            Destroy(spawnedIngameUI);
    }

    void OnPause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (PauseUIObject)
                spawnedPauseMenu = GameObject.Instantiate(PauseUIObject);
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
