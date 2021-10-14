
using UnityEngine;

public class PlaneInputInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
