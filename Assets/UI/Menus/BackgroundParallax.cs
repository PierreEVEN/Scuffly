using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handle the parallax effect of a UI object by moving it slighty depending on the mouse position
/// </summary>
public class BackgroundParallax : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        transform.localPosition = new Vector3(mousePos.x * -.01f, mousePos.y * -.01f, 0);
    }
}
