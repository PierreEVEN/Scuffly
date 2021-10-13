using UnityEngine;

public class PlaneController : MonoBehaviour
{
    // PMANE PHYSIC BODY
    Rigidbody planeBody;

    // Start is called before the first frame update
    void Start()
    {
        planeBody = GetComponent<Rigidbody>();
        if (!planeBody)
            Debug.LogError("current plane doesn't have any rigid body component");
    }

    // Update is called once per frame
    void Update()
    {



    }

    void set_thrust_input(float value)
    {
        foreach (var thruster in gameObject.GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(value);
        }
    }

}
