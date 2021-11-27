using MLAPI;
using UnityEngine;

// Point de spawn d'un avion
[ExecuteInEditMode]
public class PlaneSpawnpoint : NetworkBehaviour
{
    public GameObject assignedPlane;

    // Si true, ne peut �tre utilis� que par des IA, a l'inverse, ne peut etre utilis� que par des joueurs si false
    public bool useForAI = false;

    public GameObject SpawnPlane(bool isAi, ulong clientId)
    {
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
