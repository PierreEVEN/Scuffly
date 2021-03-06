using MLAPI;
using UnityEngine;

/// <summary>
/// Wip multiplayer manager
/// //@TODO MULTIPLAYER !!!
/// </summary>
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

    private void Start()
    {
        NetworkManager.Singleton.StartHost();
        Destroy(this);
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

    public void GoToMenu()
    {

    }
}
