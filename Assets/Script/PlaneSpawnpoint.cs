using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

// Point de spawn d'un avion
[ExecuteInEditMode]
public class PlaneSpawnpoint : NetworkBehaviour
{
    public GameObject assignedPlane;

    public bool useForAI = false;

    [HideInInspector]
    public NetworkVariable<bool> hasSpawned = new NetworkVariable<bool>(false);

    public GameObject SpawnPlane(bool isAi, ulong clientId)
    {
        hasSpawned.Value = true;
        GameObject plane = Instantiate(assignedPlane);
        plane.transform.position = transform.position;
        plane.transform.rotation = transform.rotation;
        if (isAi)
            plane.AddComponent<PlaneAIController>();
        else
            plane.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        return plane;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gameObject.transform.position, 1);
    }
}
