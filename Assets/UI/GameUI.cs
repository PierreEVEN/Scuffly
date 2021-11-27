using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject HintText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var selectedSwitch = PlayerManager.LocalPlayer.GetComponent<CameraManager>().selectedSwitch;
        HintText.GetComponent<Text>().text = selectedSwitch ? selectedSwitch.Desc : "";
    }
}
