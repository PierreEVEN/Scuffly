using MLAPI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkBehaviour))]
public class PlaneManager : MonoBehaviour
{
    public Vector3 massCenter = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = massCenter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetAttitude()
    {
        return Mathf.Asin(transform.forward.y) / Mathf.PI * 90;
    }

    public float GetRoll()
    {
        return Mathf.Asin(transform.right.y) / Mathf.PI * 90;
    }
    
    public float GetHeading()
    {
        return transform.rotation.eulerAngles.y;
    }
}