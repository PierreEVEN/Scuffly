using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class OptionWidget : MonoBehaviour
{
    public HDRenderPipelineAsset hdrpAsset;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetGraphicsMax()
    {
        GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows = LightShadows.Hard;
        GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
        GPULandscape terrain = landscape.GetComponent<GPULandscape>();
        ProceduralFolliageSpawner folliage = landscape.GetComponent<ProceduralFolliageSpawner>();
        terrain.meshDensity = 80;
        terrain.sectionLoadDistance = 4;
        folliage.densityMultiplier = 1.5f;
        folliage.sectionLoadDistance = 2;
        terrain.Reset = true;
        folliage.Reset = true;
    }
    public void SetGraphicsHigh()
    {
        GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows = LightShadows.Hard;
        GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
        GPULandscape terrain = landscape.GetComponent<GPULandscape>();
        ProceduralFolliageSpawner folliage = landscape.GetComponent<ProceduralFolliageSpawner>();
        terrain.meshDensity = 50;
        terrain.sectionLoadDistance = 2;
        folliage.densityMultiplier = 1.25f;
        folliage.sectionLoadDistance = 2;
        terrain.Reset = true;
        folliage.Reset = true;
    }

    public void SetGraphicsMedium()
    {
        GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows = LightShadows.Hard;
        GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
        GPULandscape terrain = landscape.GetComponent<GPULandscape>();
        ProceduralFolliageSpawner folliage = landscape.GetComponent<ProceduralFolliageSpawner>();
        terrain.meshDensity = 25;
        terrain.sectionLoadDistance = 1;
        folliage.densityMultiplier = 0.7f;
        folliage.sectionLoadDistance = 1;
        terrain.Reset = true;
        folliage.Reset = true;
    }

    public void SetGraphicsMinimum()
    {
        GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows = LightShadows.None;
        GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
        GPULandscape terrain = landscape.GetComponent<GPULandscape>();
        ProceduralFolliageSpawner folliage = landscape.GetComponent<ProceduralFolliageSpawner>();
        terrain.meshDensity = 10;
        terrain.sectionLoadDistance = 0;
        folliage.densityMultiplier = 0.4f;
        folliage.sectionLoadDistance = 0;
        terrain.Reset = true;
        folliage.Reset = true;
    }

    public void Quit()
    {

        Application.Quit();
    }
}
