
using UnityEngine;

[ExecuteInEditMode]
public class GPULandscapeNode : MonoBehaviour
{

    public Material landscapeMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawProcedural(landscapeMaterial, new Bounds(new Vector3(), new Vector3(5000, 5000, 5000)), MeshTopology.Triangles, 10 * 10 * 6);
    }
}
