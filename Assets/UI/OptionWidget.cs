using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;

public class OptionWidget : MonoBehaviour
{
    public HDRenderPipelineAsset hdrpAsset;

    Text fpsText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var text in GetComponentsInChildren<Text>())
        {
            if (text.text == "fps")
                fpsText = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsText)
        {
            fpsText.text = 1 / Time.deltaTime + " fps";
        }
    }

    public void SetShadows(Toggle enable)
    {
        if (enable.isOn)
            QualitySettings.SetQualityLevel(0);
        else
            QualitySettings.SetQualityLevel(2);
    }

    public void SetVolumetricClouds(bool enable)
    {
    }

    public void SetGraphicsMax()
    {
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
