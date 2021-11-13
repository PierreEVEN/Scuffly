using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnPointInfos
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
    public bool isRedTeam;
}

public class GameManager : NetworkBehaviour
{
    private static GameManager _singleton;
    [HideInInspector]
    public static GameManager Singleton
    {
        get
        {
            if (!_singleton) _singleton = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            return _singleton;
        }
    }

    private void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient & !NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
        {
            if (GUILayout.Button("  Host  "))
            {
                NetworkManager.Singleton.StartHost();
                Destroy(this);
            }
            if (GUILayout.Button("  Client Only  "))
            {
                NetworkManager.Singleton.StartClient();
                Destroy(this);
            }
            if (GUILayout.Button("  Server Only  "))
            {
                NetworkManager.Singleton.StartServer();
                Destroy(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
