using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.HighDefinition;

public class PlayerManager : NetworkBehaviour
{
    public GameObject DefaultPlane;

    [HideInInspector]
    public PlaneManager controlledPlane;
    [HideInInspector]
    public PlaneManager viewPlane;

    NetworkVariable<string> playerName = new NetworkVariable<string>("toto ");

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<NetworkObject>().
        if (IsLocalPlayer)
        {
            RequestPlaneServerRpc();
        }
        else
        {
            Destroy(GetComponent<PlayerInput>());
            Destroy(GetComponent<AudioListener>());
            GetComponent<Camera>().enabled = false;
            Destroy(GetComponent<InputSystemUIInputModule>());
            Destroy(GetComponent<EventSystem>());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ServerRpc]
    void RequestPlaneServerRpc()
    {
        foreach (var spawner in GameObject.FindGameObjectsWithTag("PlaneSpawnPoint"))
        {
            if (!spawner.GetComponent<PlaneSpawnpoint>().hasSpawned.Value)
            {
                GameObject plane = spawner.GetComponent<PlaneSpawnpoint>().SpawnPlane(OwnerClientId);
                if (!plane) return;
                NetworkObject planeNet = plane.GetComponent<NetworkObject>();
                OnPlaneSpawnedClientRpc(planeNet.NetworkObjectId);
                break;
            }
        }
    }

    [ClientRpc]
    public void OnPlaneSpawnedClientRpc(ulong planeId)
    {
        NetworkObject viewPlaneNet = GetNetworkObject(planeId);
        if (!viewPlaneNet)
        {
            Debug.LogError("plane spawned but cannot be found on client side : " + planeId);
            return;
        }
        controlledPlane = viewPlaneNet.GetComponent<PlaneManager>();
        viewPlane = controlledPlane;
        Debug.Log("client received plane spawn info");
    }
}
