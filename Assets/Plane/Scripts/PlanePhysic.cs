using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysic : MonoBehaviour
{
    private struct PhysicArea
    {
        public Vector3 normal;
        public float area;
        public Vector3 center;
    }

    public float drag_coeff = 0.0001f;

    Rigidbody targetBody;

    List<PhysicArea> Areas = new List<PhysicArea>();

    // Start is called before the first frame update
    void Start()
    {
        targetBody = gameObject.GetComponent<Rigidbody>();
        targetBody.AddForce(new Vector3(-1000, 0, 0));

        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())
        {
            var triangles = collider.sharedMesh.GetTriangles(0);

            for (int i = 0; i < triangles.Length / 3; ++i)
            {
                Areas.Add(ComputePhysicArea(
                    gameObject.transform.InverseTransformPoint(collider.gameObject.transform.TransformPoint(collider.sharedMesh.vertices[triangles[i * 3]])),
                    gameObject.transform.InverseTransformPoint(collider.gameObject.transform.TransformPoint(collider.sharedMesh.vertices[triangles[i * 3 + 1]])),
                    gameObject.transform.InverseTransformPoint(collider.gameObject.transform.TransformPoint(collider.sharedMesh.vertices[triangles[i * 3 + 2]]))
                    )); ;
            }
        }
    }

    private PhysicArea ComputePhysicArea(Vector3 vertice1, Vector3 vertice2, Vector3 vertice3)
    {
        PhysicArea area = new PhysicArea();
        Vector3 AB = vertice2 - vertice1;
        Vector3 AC = vertice3 - vertice2;

        float nAB = AB.magnitude;
        float nAC = AC.magnitude;

        area.normal = Vector3.Cross(AB, AC).normalized;
        float dot = Vector3.Dot(AB, AC);
        float dot2 = dot * dot;

        area.area = Mathf.Sqrt(nAB * nAB * nAC * nAC - dot2) / 2.0f;

        area.center = (vertice1 + vertice2 + vertice3) / 3;

        return area;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 total_drag = new Vector3(0, 0, 0);
        foreach (var area in Areas)
        {

            Vector3 relativeVelocity = gameObject.transform.InverseTransformDirection(targetBody.GetPointVelocity(gameObject.transform.TransformPoint(area.center)));

            float areaDrag = Mathf.Max(0.0f, Vector3.Dot(area.normal, relativeVelocity)) * area.area;
            Vector3 local_drag = area.normal * areaDrag * areaDrag * -1;
            total_drag += local_drag * drag_coeff;

            Vector3 dragApplyPos = gameObject.transform.TransformPoint(area.center);
            Vector3 dragApplyVector = gameObject.transform.TransformDirection(local_drag);

            targetBody.AddForceAtPosition(dragApplyVector * Time.deltaTime *drag_coeff, dragApplyPos);

            Debug.DrawLine(dragApplyPos, dragApplyPos + dragApplyVector * 0.0002f, Color.red);
        }

        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + total_drag, Color.blue);

        //targetBody.AddForce(total_drag * Time.deltaTime);


        /*
        foreach (var area in Areas) // Draw drag areas
            Debug.DrawLine(area.center, area.center + area.normal * area.area * 0.5f, Color.green);
        */
    }
}
