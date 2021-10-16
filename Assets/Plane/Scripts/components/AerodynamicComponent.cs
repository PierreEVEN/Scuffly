using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicComponent : MonoBehaviour
{
    private struct PhysicSurface
    {
        public Vector3 localNormal;
        public Vector3 localCenter;
        public float worldArea;
    }

    public float drag_coeff = 0.0001f;

    Rigidbody rigidBody;
    MeshCollider meshCollider;

    List<PhysicSurface> Surfaces = new List<PhysicSurface>();

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponentInParent<Rigidbody>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        if (!meshCollider)
            Debug.LogError("Aerodynamic Component requires a mesh collider component in the current gameObject : " + gameObject.name);

        if (!rigidBody)
            Debug.LogError("Aerodynamic Component requires a rigidbody component in parents game objects" + gameObject.name);

        UpdateData();
    }

    private void UpdateData()
    {
        Surfaces.Clear();

        if (!meshCollider)
            return;

        var triangles = meshCollider.sharedMesh.GetTriangles(0);

        for (int i = 0; i < triangles.Length / 3; ++i)
        {
            Vector3 v1 = meshCollider.sharedMesh.vertices[triangles[i * 3]];
            Vector3 v2 = meshCollider.sharedMesh.vertices[triangles[i * 3 + 1]];
            Vector3 v3 = meshCollider.sharedMesh.vertices[triangles[i * 3 + 2]];

            Surfaces.Add(ComputePhysicArea(v1, v2, v3));
        }
    }

    private PhysicSurface ComputePhysicArea(Vector3 vertice1, Vector3 vertice2, Vector3 vertice3)
    {
        PhysicSurface surface = new PhysicSurface();

        // Compute local surface center
        surface.localCenter = (vertice1 + vertice2 + vertice3) / 3;

        Vector3 AB = vertice2 - vertice1;
        Vector3 AC = vertice3 - vertice2;

        // Compute local surface normal
        surface.localNormal = Vector3.Cross(AB, AC).normalized;

        // Switch to world space to get correct surface area
        AB = gameObject.transform.TransformPoint(AB);
        AC = gameObject.transform.TransformPoint(AC);

        float nAB = AB.magnitude;
        float nAC = AC.magnitude;

        float dot = Vector3.Dot(AB, AC);
        float dot2 = dot * dot;
        surface.worldArea = Mathf.Sqrt(nAB * nAB * nAC * nAC - dot2) / 2.0f;


        if (float.IsNaN(surface.worldArea))
        {
            Debug.LogError("3");
            surface.worldArea = 0;
        }

        return surface;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 totalForce = new Vector3();

        foreach (var surface in Surfaces)
        {
            Vector3 worldCenter = gameObject.transform.TransformPoint(surface.localCenter);
            Vector3 worldPointVelocity = rigidBody.GetPointVelocity(worldCenter);
            
            // @IDEA : don't set 0 but an approximation of lift instead
            float areaDrag = Mathf.Max(0.0f, Vector3.Dot(gameObject.transform.TransformDirection(surface.localNormal), worldPointVelocity * worldPointVelocity.magnitude)) * surface.worldArea;
            Vector3 local_drag = surface.localNormal * areaDrag * -1;


            Vector3 dragApplyVector = gameObject.transform.TransformDirection(local_drag) * drag_coeff * 100f;

            totalForce += dragApplyVector;
            rigidBody.AddForceAtPosition(dragApplyVector * Time.deltaTime, worldCenter);

            Debug.DrawLine(worldCenter, worldCenter + dragApplyVector * 0.00001f, Color.red);
        }

        var center = gameObject.transform.position;
        Debug.DrawLine(center, center + totalForce * 0.000001f, Color.green);
       
        foreach (var surface in Surfaces) // Draw drag areas
        {
            var worldCenter = gameObject.transform.TransformPoint(surface.localCenter);
            var direction = gameObject.transform.TransformDirection(surface.localNormal);
            Debug.DrawLine(worldCenter, worldCenter + direction * surface.worldArea * .0001f, Color.green);
        }
    }
}
