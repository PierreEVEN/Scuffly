using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
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
public class AerodynamicComponent : MonoBehaviour
{
    /// <summary>
    /// For the aerodynamic simulations, we need the area, the normal, and the position of each face
    /// All of this is stured in a structure list
    /// </summary>
    private struct PhysicSurface
    {
        public Vector3 localNormal; // object space surface normal
        public Vector3 localCenter; // object space surface center
        public float worldArea; // World space surface area
    }
    /// <summary>
    /// All the surface are precomputed and stored into this surface list.
    /// </summary>
    private List<PhysicSurface> Surfaces = new List<PhysicSurface>();

    /// <summary>
    /// The mesh used to compute the aerodynamic forces (it should be a simplified version of the original model)
    /// </summary>
    public Mesh meshOverride;

    /// <summary>
    /// RigidBody of the object owning this component
    /// </summary>
    private Rigidbody rigidBody;


#if UNITY_EDITOR
    static bool drawSurfaceInfluence = true;
    static bool drawPerSurfaceForce = true;
#endif

    void OnEnable()
    {
        rigidBody = gameObject.GetComponentInParent<Rigidbody>();

        if (!rigidBody)
            Debug.LogError("Aerodynamic Component requires a rigidbody component in parents game objects" + gameObject.name);

        RecomputeData();
    }

    /// <summary>
    /// Rebuild the precomputed data for each face of the mesh (normal / area / relative position...)
    /// </summary>
    private void RecomputeData()
    {
        Profiler.BeginSample("Recompute aerodynamic surfaces");
        Surfaces.Clear();

        if (!meshOverride)
        {
            Debug.LogError("missing aerodynamic mesh on " + name);
            return;
        }
                 
        for (int sectionID = 0; sectionID < meshOverride.subMeshCount; sectionID++)
        {
            int[] triangles = meshOverride.GetTriangles(sectionID);
            for (int i = 0; i < triangles.Length / 3; ++i)
            {
                Vector3 v1 = meshOverride.vertices[triangles[i * 3]];
                Vector3 v2 = meshOverride.vertices[triangles[i * 3 + 1]];
                Vector3 v3 = meshOverride.vertices[triangles[i * 3 + 2]];
                Surfaces.Add(ComputePhysicSurface(v1, v2, v3));
            }
        }
        Profiler.EndSample();
    }

    /// <summary>
    /// A surface is caracterized by 3 triangles.This function will compute the area, the center, and the normal of the shape of the given triangle.
    /// </summary>
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

    void FixedUpdate()
    {
        if (Surfaces.Count == 0)
            RecomputeData();

        Profiler.BeginSample("Compute aerodynamic forces for " + transform.root.name + " : " + gameObject.name);

        // On applique une force à chaque surface
        for (int i = 0; i < Surfaces.Count; ++i)
        {
            PhysicSurface surface = Surfaces[i];

            Vector3 worldCenter = transform.TransformPoint(surface.localCenter);
            Vector3 worldPointVelocity = rigidBody.GetPointVelocity(worldCenter);
            Vector3 WorldNormal = transform.TransformDirection(surface.localNormal);
            float pointMagnitude = worldPointVelocity.magnitude;

            // Densite du milieu (100 pour l'eau au lieu de 1000 pour eviter de trop rebondir)
            float ro = worldCenter.y <= 0 ? 100.0f : 1.2f;

            // @TODO : don't set 0 in the first argument of max(), but an approximation of lift instead (allow negative force)
            float areaDrag = Mathf.Max(0.0f, Vector3.Dot(WorldNormal, worldPointVelocity.normalized)) * surface.worldArea;
            Vector3 local_drag = surface.localNormal * areaDrag * pointMagnitude * pointMagnitude * ro * -1;

            Vector3 dragApplyVectors = gameObject.transform.TransformDirection(local_drag);

            rigidBody.AddForceAtPosition(dragApplyVectors, worldCenter);

#if UNITY_EDITOR
            if (drawPerSurfaceForce) Debug.DrawLine(worldCenter, worldCenter + dragApplyVectors * 0.002f, Color.red);
#endif
        }
        Profiler.EndSample();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Only within editor : draw debug aerodynamic forces at each point
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
#endif
}
