
using UnityEngine;

public class GPULandscapeNode
{
    MaterialPropertyBlock MPB;
    float width;
    GPULandscape owner;
    int quadtreeLevel;
    Vector3 worldPosition;
    bool shouldDisplay;
    Bounds bounds;

    private GPULandscapeNode[] children;

    public GPULandscapeNode(GPULandscape owner, int quadtreeLevel, Vector3 worldPosition, float width)
    {
        this.owner = owner;
        this.quadtreeLevel = quadtreeLevel;
        this.worldPosition = worldPosition;
        this.width = width;
        MPB = new MaterialPropertyBlock();
        bounds = new Bounds(worldPosition, new Vector3(this.width, 100000, this.width));
    }

    public void destroy()
    {
        MPB = null;

        ShowCurrentNode();
    }

    public void OnDrawGizmos()
    {

    }

    void DrawSection()
    {
        if (shouldDisplay)
        {
            MPB.SetInt("_Subdivision", owner.chunkSubdivision);
            MPB.SetFloat("_Width", width / owner.chunkSubdivision);
            MPB.SetVector("_Offset", worldPosition);
            int triangleVerticeCount = owner.chunkSubdivision * owner.chunkSubdivision * 6;
            Graphics.DrawProcedural(owner.landscape_material, bounds, MeshTopology.Triangles, triangleVerticeCount, 1, null, MPB);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        /*
        Either we wants to subdivide this node if he is to close to the camera, either we wants to display its mesh section
         */
        if (ComputeDesiredLODLevel() > quadtreeLevel)
            SubdivideCurrentNode();
        else
            ShowCurrentNode();

        if (shouldDisplay)
            DrawSection();

        // Propagate update through children
        if (children != null)
            foreach (var child in children)
                child.Update();
    }


    private void ShowCurrentNode()
    {
        shouldDisplay = true;

        /*
        Destroy child if not destroyed
         */
        if (children == null) return;
        foreach (var child in children)
            child.destroy();
        children = null;
    }

    // Subdivide this node into 4 nodes 2 times smaller
    private void SubdivideCurrentNode()
    {
        // Test if already subdivided, or start subdivision
        if (children == null)
        {
            children = new GPULandscapeNode[4];
            children[0] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x - width / 4, worldPosition.y, worldPosition.z - width / 4),
                width / 2);
            children[1] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x + width / 4, worldPosition.y, worldPosition.z - width / 4),
                width / 2);
            children[2] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x + width / 4, worldPosition.y, worldPosition.z + width / 4),
                width / 2);
            children[3] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x - width / 4, worldPosition.y, worldPosition.z + width / 4),
                width / 2);
        }

        shouldDisplay = false;
    }

    private int ComputeDesiredLODLevel()
    {
        // Height correction
        Vector3 cameraGroundLocation = owner.GetCameraPosition();
        cameraGroundLocation.y -= owner.GetAltitudeAtLocation(owner.GetCameraPosition().x, owner.GetCameraPosition().z);
        float Level = owner.maxLevel - Mathf.Min(owner.maxLevel, (Vector3.Distance(cameraGroundLocation, worldPosition) - width) / owner.quadtreeExponent);
        return (int)Level;
    }
}
