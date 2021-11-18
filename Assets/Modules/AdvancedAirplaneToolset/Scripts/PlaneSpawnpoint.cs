using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

// Point de spawn d'un avion
[ExecuteInEditMode]
public class PlaneSpawnpoint : NetworkBehaviour
{
    public bool IsRedTeam = false;
    public GameObject assignedPlane;

    [HideInInspector]
    public NetworkVariable<bool> hasSpawned = new NetworkVariable<bool>(false);

    public GameObject SpawnPlane(ulong clientId)
    {
        hasSpawned.Value = true;
        GameObject plane = Instantiate(assignedPlane);
        plane.transform.position = transform.position;
        plane.transform.rotation = transform.rotation;
        plane.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        return plane;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gameObject.transform.position, 1);
    }
}
