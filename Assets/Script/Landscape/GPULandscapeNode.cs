
using UnityEngine;

public class GPULandscapeNode
{
    private GPULandscape owner;
    private bool shouldDisplay;

    // Section informations
    private int quadtreeLevel;
    private float width;
    private Vector3 worldPosition;

    // Delimitations de la section (@TODO : faire la gestion des bounds sur l'axe y)
    private Bounds bounds;

    //  Informations pour le material sur le node courrant a afficher
    private MaterialPropertyBlock MPB;

    // Nodes enfant du quadtree
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

    void DrawSection()
    {
        if (shouldDisplay)
        {
            // Draw mesh using landscape material
            MPB.SetInt("_Subdivision", owner.chunkSubdivision);
            MPB.SetFloat("_Width", owner.SectionWidth);
            MPB.SetVector("_Offset", owner.CameraCurrentLocation);
            MPB.SetFloat("worldExponent", owner.worldExponent);
            MPB.SetFloat("minValue", owner.minValue);
            Graphics.DrawProcedural(owner.landscape_material, bounds, MeshTopology.Triangles, owner.chunkSubdivision * owner.chunkSubdivision * 6, 1, null, MPB);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        /*
        Si le niveau de subdivision desire est superieur au niveau de subdivision courant : on subdivise, sinon on affiche le mesh courant et on detruit les nodes enfants
         */

        /*
        if (ComputeDesiredLODLevel() > quadtreeLevel)
            SubdivideCurrentNode();
        else
        */
        ShowCurrentNode();

        if (shouldDisplay)
            DrawSection();

        // Propagate update through children
        if (children != null)
            foreach (var child in children)
                child.Update();
    }


    // Detruit les nodes enfant et active le rendu du mesh courant.
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

    // Subdivise le node courant en 4 nodes 2 fois plus petits. Desactive l'affichage du node courant
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
        Vector3 cameraGroundLocation = owner.CameraCurrentLocation;
        cameraGroundLocation.y -= 0; // @TODO : get landscape altitude at camera location
        float Level = owner.maxLevel - Mathf.Min(owner.maxLevel, (Vector3.Distance(cameraGroundLocation, worldPosition) - width) / owner.quadtreeExponent);
        return (int)Level;
    }
}
