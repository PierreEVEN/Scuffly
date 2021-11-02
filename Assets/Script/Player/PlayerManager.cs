using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public GameObject DefaultPlane;

    [HideInInspector]
    public PlaneManager controlledPlane;
    [HideInInspector]
    public NetworkVariable<GameObject> viewPlane = new NetworkVariable<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
            PossessPlane(Gamemode.Singleton.SpawnPlane(GameObject.FindGameObjectWithTag("SpawnPoint"), DefaultPlane));

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PossessPlane(GameObject plane)
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        controlledPlane = plane.GetComponent<PlaneManager>();
        viewPlane.Value = plane;
        Debug.Log("posses server");
    }
}
