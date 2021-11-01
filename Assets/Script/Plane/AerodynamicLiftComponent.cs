using System.Collections.Generic;
using UnityEngine;

class AeroSection
{
    public float xStep = 0;
    public Vector2 bounds = new Vector2();
    AerodynamicLiftComponent parentComponent;

    List<Vector3> computePoints = new List<Vector3>();

    public AeroSection(AerodynamicLiftComponent parent)
    {
        parentComponent = parent;
    }

    public void Init()
    {
        FindComputePoints();



    }

    public void DrawDebugs()
    {
        Gizmos.color = new Color(.5f, 0, 0, .3f);
        Gizmos.DrawCube(parentComponent.transform.TransformPoint(new Vector3(xStep, parentComponent.wingBoundsLocal.center.y, (bounds.x + bounds.y) / 2.0f)), new Vector3(parentComponent.SectionStep / 4, parentComponent.wingBoundsLocal.size.y, Mathf.Abs(bounds.x - bounds.y)));
        Gizmos.color = Color.green;
        foreach (var point in computePoints)
        {
            Gizmos.DrawSphere(parentComponent.transform.TransformPoint(point), parentComponent.ComputePointStep / 4);
        }
    }

    void FindComputePoints()
    {
        string outStr = "test_str\n";
        computePoints.Clear();
        RaycastHit hitInfos = new RaycastHit();

        bool forward = true;
        do
        {
            for (float z = forward ? bounds.y : bounds.x; forward ? z <= bounds.x : z >= bounds.y; z = forward ? z + parentComponent.ComputePointStep : z - parentComponent.ComputePointStep)
            {
                // Compute dispersion to have more precision on wing borders
                float width = Mathf.Abs(bounds.y - bounds.x);
                float center = (bounds.y + bounds.x) / 2;
                float pointBeforeDispersion = (z - center) / (width / 2);
                float sign = Mathf.Sign(pointBeforeDispersion);
                float pointAfterDispersion = Mathf.Pow(Mathf.Abs(pointBeforeDispersion), 1.0f / parentComponent.ComputePointDispersion) * sign;
                float zOffset = pointAfterDispersion * (width / 2) + center;

                float distance = parentComponent.wingBoundsLocal.size.y;
                Vector3 point = new Vector3();
                bool hit = false;
                Ray ray = new Ray(
                    new Vector3(xStep, parentComponent.wingBoundsLocal.center.y + (forward ? parentComponent.wingBoundsLocal.size.y : -parentComponent.wingBoundsLocal.size.y) / 2.0f, zOffset),
                    new Vector3(0, forward ? -1 : 1, 0)
                );

                foreach (var collider in parentComponent.meshColliders)
                {
                    if (collider.Raycast(ray, out hitInfos, parentComponent.wingBoundsLocal.size.y))
                    {
                        hit = true;
                        if (hitInfos.distance < distance)
                        {
                            hitInfos.distance = distance;
                            point = hitInfos.point;
                        }
                    }
                }
                if (!hit)
                    continue;
                computePoints.Add(point);
                outStr += string.Format("{0,8:F8}", 1 - (point.z + parentComponent.wingBoundsLocal.size.z) / parentComponent.wingBoundsLocal.size.z) + " " + string.Format("{0,8:F8}", point.y / parentComponent.wingBoundsLocal.size.z) + "\n";
            }
            forward = !forward;
        } while (forward == false);
        Debug.Log(outStr.Replace(',', '.'));
    }



}




[ExecuteInEditMode]
public class AerodynamicLiftComponent : MonoBehaviour
{
    [Range(0.001f, 0.1f)]
    public float BoundAnalysisStep = 0.01f;

    [Range(0.05f, 1.0f)]
    public float SectionStep = 0.20f;

    [Range(0.01f, 0.5f)]
    public float ComputePointStep = 0.04f;

    [Range(1, 8)]
    public int ComputePointDispersion = 4;

    public bool DebugDrawTraces = false;
    public bool ForceRecompute = false;

    // Physic object
    private Rigidbody rigidBody;

    // Collider containing mesh data
    [HideInInspector]
    public List<MeshCollider> meshColliders = new List<MeshCollider>();
    private List<AeroSection> sections = new List<AeroSection>();

    [HideInInspector]
    public Bounds wingBoundsLocal;



    void Start()
    {
        rigidBody = gameObject.GetComponentInParent<Rigidbody>();
        if (!rigidBody)
            Debug.LogError("Aerodynamic Lift Component requires a rigidbody component in parents game objects" + gameObject.name);

        RecomputeData();
    }

    private void RecomputeData()
    {
        meshColliders.Clear();

        foreach (var comp in gameObject.GetComponentsInChildren<MeshCollider>())
            meshColliders.Add(comp);

        if (meshColliders.Count == 0)
            Debug.LogError("Aerodynamic Lift Component requires a mesh collider component within the children of " + gameObject.name);

        UpdateGlobalBounds();

        sections.Clear();
        for (float xStep = wingBoundsLocal.center.x - wingBoundsLocal.size.x; xStep < wingBoundsLocal.center.x + wingBoundsLocal.size.x; xStep += SectionStep)
        {
            AeroSection section = new AeroSection(this);
            if (!ComputeSectionBounds(xStep, out section.bounds))
                continue;
            section.xStep = xStep;
            section.Init();

            sections.Add(section);
        }
    }

    void UpdateGlobalBounds()
    {
        wingBoundsLocal = new Bounds();
        for (int i = 0; i < meshColliders.Count; ++i)
        {
            Bounds addedBounds = new Bounds(transform.InverseTransformPoint(meshColliders[i].bounds.center), meshColliders[i].bounds.size);
            if (i == 0)
                wingBoundsLocal = addedBounds;
            else
                wingBoundsLocal.Encapsulate(addedBounds);
        }
    }

    bool ComputeSectionBounds(float sectionOffset, out Vector2 outBounds)
    {
        bool foundBeginning = false;
        bool foundEnd = false;
        outBounds = new Vector2();
        RaycastHit hitInfos = new RaycastHit();
        for (float zStep = wingBoundsLocal.center.z + wingBoundsLocal.size.z / 2; zStep > wingBoundsLocal.center.z - wingBoundsLocal.size.z / 2; zStep -= BoundAnalysisStep)
        {
            bool hasHit = false;
            foreach (var comp in meshColliders)
            {
                hasHit = comp.Raycast(new Ray(new Vector3(sectionOffset, wingBoundsLocal.center.y + wingBoundsLocal.size.y / 2.0f, zStep), new Vector3(0, -1, 0)), out hitInfos, wingBoundsLocal.size.y);
                if (hasHit) break;
            }
            if (hasHit)
            {
                if (!foundBeginning)
                {
                    foundBeginning = true;
                    outBounds.x = zStep - BoundAnalysisStep / 2.0f;
                }
            }
            else
            {
                if (foundBeginning)
                {
                    foundEnd = true;
                    outBounds.y = zStep - BoundAnalysisStep / 2.0f;
                    return true;
                }
            }
        }

        if (foundBeginning && !foundEnd)
        {
            outBounds.y = wingBoundsLocal.center.z - wingBoundsLocal.size.z / 2;
            return true;
        }
        return false;
    }


    private void OnDrawGizmos()
    {
        if (!DebugDrawTraces) return;
        Gizmos.color = new Color(.2f, .2f, 0, .1f);
        Gizmos.DrawCube(transform.TransformPoint(wingBoundsLocal.center), wingBoundsLocal.size);
        foreach (var section in sections)
        {
            section.DrawDebugs();
        }
    }

    void Update()
    {
        if (sections.Count == 0 || ForceRecompute)
        {
            ForceRecompute = false;
            RecomputeData();
        }
    }
}
