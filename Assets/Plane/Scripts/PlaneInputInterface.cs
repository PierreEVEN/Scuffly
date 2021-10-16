
using UnityEngine;

public class PlaneInputInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnGUI()
    {
        GUILayout.Space(50);
        GUILayout.TextArea("Velocity : " + gameObject.GetComponent<Rigidbody>().velocity.magnitude + " m/s  |  " + gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6 + " km/h  |  " + gameObject.GetComponent<Rigidbody>().velocity.magnitude * 1.94384519992989f + " noeuds");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(gameObject.GetComponent<Rigidbody>().gameObject.transform.TransformPoint(gameObject.GetComponent<Rigidbody>().centerOfMass), 0.5f);
    }

    public Vector3 MassCenter = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = MassCenter;
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
                part.setInput(value * -1);
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

    public void switchApu()
    {
        foreach (var part in gameObject.GetComponentsInChildren<APU>())
            if (part.IsReady())
                part.StopApu();
            else
                part.StartApu();
    }
    public void switchEngine()
    {
        foreach (var part in gameObject.GetComponentsInChildren<Thruster>())
            if (part.IsEnabled())
                part.StopEngine();
            else
                part.StartEngine();
    }
}
