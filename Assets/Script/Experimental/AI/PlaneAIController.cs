using UnityEngine;

public class PlaneAIController : MonoBehaviour
{
    PlaneManager controlledPlane;

    float averageFlightAltitude = 2000.0f;


    // Start is called before the first frame update
    void Start()
    {
        controlledPlane = GetComponent<PlaneManager>();
        controlledPlane.EnableAPU = true;
        controlledPlane.ThrottleNotch = true;
        controlledPlane.Brakes = false;
        controlledPlane.SetThrustInput(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        NavigationMode();

    }


    void NavigationMode()
    {
        float finalPitchInput = Mathf.Clamp((averageFlightAltitude - transform.position.y) / 100.0f, 0, 1) + controlledPlane.GetAttitude() * 0.01f;

        // controlledPlane.GetAttitude() * 0.1f

        controlledPlane.SetRollInput(Mathf.Pow(controlledPlane.GetRoll() * -0.001f, 3));

        controlledPlane.SetPitchInput(finalPitchInput);
    }

}
