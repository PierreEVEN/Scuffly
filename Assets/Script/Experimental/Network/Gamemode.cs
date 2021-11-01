using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using UnityEngine;

public class Gamemode : NetworkBehaviour
{
    private static Gamemode _singleton;
    [HideInInspector]
    public static Gamemode Singleton
    {
        get
        {
            if (!_singleton) _singleton = NetworkManager.Singleton.gameObject.AddComponent<Gamemode>();
            return _singleton;
        }
    }

    public GameObject SpawnPlane(GameObject spawner, GameObject planeClass)
    {
        if (!NetworkManager.Singleton.IsHost)
            return null;

        GameObject plane = Instantiate(planeClass);
        plane.transform.position = spawner.transform.position;
        plane.transform.rotation = spawner.transform.rotation;

        NetworkObject planeNetwork = plane.GetComponent<NetworkObject>();

        planeNetwork.Spawn();
        Debug.Log("spawn " + planeClass + " on server : " + planeNetwork.NetworkObjectId);
        return plane;
    }

    [ClientRpc]
    private void SpawnPlaneClientRpc(ulong planeNetID)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
