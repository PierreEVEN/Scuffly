using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 */


/**
 * Aerodynamic component class
 * Simulate aerodynamic forces on owning gameObject.
 * REQUIREMENTS : this component requires a rigidbody in parent hierarchy, and a MeshCollider on the current object.
 * The mesh assigned to the MeshCollider component should be marked as "read/write".
 * 
 * The physic is fully dynamic : updating this part transform will impact the behavior of the physic object.
 * 
 * The maths are quite simple
 * 1) precompute each surface of the mesh (area / position / normal)
 * 2) each frame : compute the velocity squared applied on each surface dot the surface normal, times the area of the surface
 * 3) apply resulting force on the parent obejct rigidBody
 * (not accurate at all, but the result is not so bad for a dumb approximation)
 */
[RequireComponent(typeof(MeshCollider))]
public class AerodynamicComponent : MonoBehaviour
{
    private struct PhysicSurface
    {
        public Vector3 localNormal; // object space surface normal
        public Vector3 localCenter; // object space surface center
        public float worldArea; // World space surface area
    }

    [Header("Developper settings")]
    public bool drawSurfaceInfluence = false;
    public bool drawTotalForce = false;
    public bool drawPerSurfaceForce = false;

    // Physic object
    private Rigidbody rigidBody;

    // Collider containing mesh data
    private MeshCollider meshCollider;

    // All the surface are precomputed and stored into this surface list.
    private List<PhysicSurface> Surfaces = new List<PhysicSurface>();

    void Start()
    {
        rigidBody = gameObject.GetComponentInParent<Rigidbody>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        if (!meshCollider)
            Debug.LogError("Aerodynamic Component requires a mesh collider component in the current gameObject : " + gameObject.name);

        if (!rigidBody)
            Debug.LogError("Aerodynamic Component requires a rigidbody component in parents game objects" + gameObject.name);

        RecomputeData();
    }

    private void RecomputeData()
    {
        Surfaces.Clear();

        for (int sectionID = 0; sectionID < meshCollider.sharedMesh.subMeshCount; sectionID++) {
            int[] triangles = meshCollider.sharedMesh.GetTriangles(sectionID);
            for (int i = 0; i < triangles.Length / 3; ++i)
            {
                Vector3 v1 = meshCollider.sharedMesh.vertices[triangles[i * 3]];
                Vector3 v2 = meshCollider.sharedMesh.vertices[triangles[i * 3 + 1]];
                Vector3 v3 = meshCollider.sharedMesh.vertices[triangles[i * 3 + 2]];
                Surfaces.Add(ComputePhysicSurface(v1, v2, v3));
            }
        }
    }

    // A surface is caracterized by 3 triangles. This function will compute the area, the center, and the normal of the shape of the given triangle.
    private PhysicSurface ComputePhysicSurface(Vector3 vertice1, Vector3 vertice2, Vector3 vertice3)
    {
        PhysicSurface surface = new PhysicSurface();

        // Compute local surface center
        surface.localCenter = (vertice1 + vertice2 + vertice3) / 3;

        Vector3 AB = vertice2 - vertice1;
        Vector3 AC = vertice3 - vertice2;

        // Compute local surface normal
        surface.localNormal = Vector3.Cross(AB, AC).normalized;

        float nAB = AB.magnitude;
        float nAC = AC.magnitude;

        //@TODO area will not be correct if object scale is not (1, 1, 1)
        float dot = Vector3.Dot(AB, AC);
        float dot2 = dot * dot;
        surface.worldArea = Mathf.Sqrt(nAB * nAB * nAC * nAC - dot2) / 2.0f;

        return surface;
    }

    void Update()
    {
        Vector3 totalForce = new Vector3();

        foreach (var surface in Surfaces)
        {
            Vector3 worldCenter = gameObject.transform.TransformPoint(surface.localCenter);
            Vector3 worldPointVelocity = rigidBody.GetPointVelocity(worldCenter);
            
            // @TODO : don't set 0 in the first argument of max(), but an approximation of lift instead (allow negative force)
            float areaDrag = Mathf.Max(0.0f, Vector3.Dot(gameObject.transform.TransformDirection(surface.localNormal), worldPointVelocity * worldPointVelocity.magnitude)) * surface.worldArea;
            Vector3 local_drag = surface.localNormal * areaDrag * -1;

            Vector3 dragApplyVector = gameObject.transform.TransformDirection(local_drag) * 200; //@TODO replace hardcoded friction with custom value

            if (drawTotalForce)
                totalForce += dragApplyVector;

            rigidBody.AddForceAtPosition(dragApplyVector * Time.deltaTime, worldCenter);

            if (drawPerSurfaceForce) Debug.DrawLine(worldCenter, worldCenter + dragApplyVector * 0.0001f, Color.red);
        }

        if (drawTotalForce)
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + totalForce * 0.000001f, Color.green);

        if (drawSurfaceInfluence)
        {
            foreach (var surface in Surfaces) // Draw drag areas
            {
                var worldCenter = gameObject.transform.TransformPoint(surface.localCenter);
                var direction = gameObject.transform.TransformDirection(surface.localNormal);
                Debug.DrawLine(worldCenter, worldCenter + direction * surface.worldArea * .1f, Color.green);
            }
        }
    }
}
