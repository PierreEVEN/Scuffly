using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class TransformReplicationComponent : NetworkBehaviour
{
    private NetworkVariable<Vector3> objectPosition = new NetworkVariable<Vector3>(new Vector3());
    private NetworkVariable<Quaternion> objectRotation = new NetworkVariable<Quaternion>(new Quaternion());
    private NetworkVariable<Vector3> objectVelocity = new NetworkVariable<Vector3>(new Vector3());
    private NetworkVariable<Vector3> objectAngularVelocity = new NetworkVariable<Vector3>(new Vector3());

    private Rigidbody physicComponent;

    // Start is called before the first frame update
    void Start()
    {
        physicComponent = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            objectPosition.Value = gameObject.transform.position;
            objectRotation.Value = gameObject.transform.rotation;
            if (physicComponent)
            {
                objectVelocity.Value = physicComponent.velocity;
                objectAngularVelocity.Value = physicComponent.angularVelocity;
            }
        }
        else
        {
            gameObject.transform.position = objectPosition.Value;
            gameObject.transform.rotation = objectRotation.Value;
            if (physicComponent)
            {
                physicComponent.velocity = objectVelocity.Value;
                physicComponent.angularVelocity = objectAngularVelocity.Value;
            }
        }
    }
}
