
using UnityEngine;

public class PlaneInputInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
    }

    void OnGUI()
    {
        GUILayout.Space(50);
        GUILayout.TextArea("Velocity : " + gameObject.GetComponent<Rigidbody>().velocity.magnitude + " m/s  |  " + gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6 + " km/h  |  " + gameObject.GetComponent<Rigidbody>().velocity.magnitude * 1.94384519992989f + " noeuds");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetThrustInput(float value)
    {
        foreach (var thruster in gameObject.GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(value);
        }
    }

    public void setPitchInput(float value)
    {
        foreach (var part in gameObject.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Pitch")
                part.setInput(value);
    }

    public void setYawInput(float value)
    {
        foreach (var part in gameObject.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Yaw")
                part.setInput(value);
    }
    public void setRollInput(float value)
    {
        foreach (var part in gameObject.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Roll")
                part.setInput(value);
    }
}
