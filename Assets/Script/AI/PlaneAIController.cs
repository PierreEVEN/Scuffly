using UnityEngine;

public class PlaneAIController : MonoBehaviour
{
    PlaneInputInterface controlledPlane;

    Vector3 targetLocation;

    // Start is called before the first frame update
    void Start()
    {
        controlledPlane = GetComponent<PlaneInputInterface>();
        controlledPlane.SetApuEnabled(true);
        controlledPlane.SetEngineEnabled(true);
        controlledPlane.SetThrustInput(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
