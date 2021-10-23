using UnityEngine;

public class PlaneAIController : MonoBehaviour
{
    PlaneManager controlledPlane;

    // Start is called before the first frame update
    void Start()
    {
        controlledPlane = GetComponent<PlaneManager>();
        controlledPlane.ApuSwitch = true;
        controlledPlane.ThrottleNotch = true;
        controlledPlane.Brakes = false;
        controlledPlane.SetThrustInput(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
