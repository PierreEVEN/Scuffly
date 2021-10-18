using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "Host"))
        {
            NetworkManager.Singleton.StartHost();
            Destroy(this);
        }
        if (GUI.Button(new Rect(0, 40, 200, 20), "ServerOnly"))
        {
            NetworkManager.Singleton.StartServer();
            Destroy(this);
        }
        if (GUI.Button(new Rect(0, 80, 200, 20), "ClientOnly"))
        {
            NetworkManager.Singleton.StartClient();
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
