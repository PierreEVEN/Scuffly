using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundParallax : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        transform.localPosition = new Vector3(mousePos.x * -.01f, mousePos.y * -.01f, 0);
    }
}
