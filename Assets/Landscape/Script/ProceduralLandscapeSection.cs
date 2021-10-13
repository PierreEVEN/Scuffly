using UnityEngine;

public class ProceduralLandscapeSection
{
    public Vector3 Pos;
    ProceduralLandscape Landscape;
    float Scale;
    ProceduralLandscapeNode RootNode;
    public ProceduralLandscapeSection(ProceduralLandscape inLandscape, Vector3 inPos, float inScale)
    {
        Landscape = inLandscape;
        Pos = inPos;
        Scale = inScale;
        RootNode = new ProceduralLandscapeNode(Landscape, 1, Pos, Scale);
    }

    public void update()
    {
        RootNode.update();
    }

    /**
     * Destructor
     */
    public void destroy()
    {
        RootNode.destroy();
        RootNode = null;
    }
}

