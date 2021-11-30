using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class OptionWidget : MonoBehaviour
{
    Volume _globalVolume;
    public Volume GlobalVolume
    {
        get
        {
            if (!_globalVolume)
            {
                GameObject lighting = GameObject.Find("Lighting");
                if (lighting)
                    _globalVolume = lighting.GetComponent<Volume>();
            }
            return _globalVolume;
        }
    }

    public void SetGraphicsMax()
    {
        AmbientOcclusion = true;
        Bloom = true;
        Fog = true;
        VolumetricClouds = true;
        GlobalShaderQuality = 2;
        LandscapeQuality = 30;
        LandscapeViewDistance = 4;
        FolliageQuality = 1.6f;
        Shadows = true;
        ContactShadows = true;
    }
    public void SetGraphicsHigh()
    {
        AmbientOcclusion = true;
        Bloom = true;
        Fog = true;
        VolumetricClouds = true;
        GlobalShaderQuality = 2;
        LandscapeQuality = 20;
        LandscapeViewDistance = 3;
        FolliageQuality = 1.1f;
        Shadows = true;
        ContactShadows = true;
    }

    public void SetGraphicsMedium()
    {
        AmbientOcclusion = true;
        Bloom = true;
        Fog = true;
        VolumetricClouds = true;
        GlobalShaderQuality = 1;
        LandscapeQuality = 15;
        LandscapeViewDistance = 2;
        FolliageQuality = 0.8f;
        Shadows = true;
        ContactShadows = true;
    }

    public void SetGraphicsLow()
    {
        AmbientOcclusion = false;
        Bloom = true;
        Fog = false;
        VolumetricClouds = false;
        GlobalShaderQuality = 0;
        LandscapeQuality = 10;
        LandscapeViewDistance = 1;
        FolliageQuality = 0.25f;
        Shadows = false;
        ContactShadows = false;
    }

    public void SetGraphicsMinimum()
    {
        AmbientOcclusion = false;
        Bloom = false;
        Fog = false;
        VolumetricClouds = false;
        GlobalShaderQuality = 0;
        LandscapeQuality = 5;
        LandscapeViewDistance = 0;
        FolliageQuality = 0f;
        Shadows = false;
        ContactShadows = false;
    }



    int LandscapeViewDistance
    {
        get
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<GPULandscape>())
            {
                return landscape.GetComponent<GPULandscape>().sectionLoadDistance;
            }
            return 0;
        }
        set
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<GPULandscape>())
            {
                landscape.GetComponent<GPULandscape>().sectionLoadDistance = value;
                landscape.GetComponent<GPULandscape>().Reset = true;
            }
        }
    }


    int LandscapeQuality
    {
        get
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<GPULandscape>())
            {
                return landscape.GetComponent<GPULandscape>().meshDensity;
            }
            return 0;
        }
        set
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<GPULandscape>())
            {
                landscape.GetComponent<GPULandscape>().meshDensity = value;
                landscape.GetComponent<GPULandscape>().Reset = true;
            }
        }
    }


    float FolliageQuality
    {
        get
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<ProceduralFolliageSpawner>())
            {
                return landscape.GetComponent<ProceduralFolliageSpawner>().densityMultiplier;
            }
            return 0;
        }
        set
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (landscape && landscape.GetComponent<ProceduralFolliageSpawner>())
            {
                landscape.GetComponent<ProceduralFolliageSpawner>().densityMultiplier = value;
                landscape.GetComponent<ProceduralFolliageSpawner>().Reset = true;
            }
        }
    }

    int GlobalShaderQuality
    {
        get
        {
            return QualitySettings.GetQualityLevel();
        }
        set
        {
            QualitySettings.SetQualityLevel(Mathf.Clamp(value, 0, 2));
        }
    }


    public bool Shadows
    {
        get
        {
            return GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows != LightShadows.None;
        }
        set
        {
            GameObject.Find("Directional Light Sun").GetComponent<Light>().shadows = value ? LightShadows.Hard : LightShadows.None;
        }
    }

    public bool VolumetricClouds
    {
        get { return GetVolumeComponent<VolumetricClouds>().active; }
        set
        {
            GetVolumeComponent<VolumetricClouds>().active = value;
        }
    }


    public bool ContactShadows
    {
        get { return GetVolumeComponent<ContactShadows>().enable.value; }
        set
        {
            GetVolumeComponent<ContactShadows>().enable.value = value;
        }
    }

    public bool Fog
    {
        get { return GetVolumeComponent<Fog>().enabled.value; }
        set
        {
            GetVolumeComponent<Fog>().enabled.value = value;
        }
    }

    public bool Bloom
    {
        get { return GetVolumeComponent<Bloom>().active; }
        set
        {
            GetVolumeComponent<Fog>().active = value;
        }
    }

    public bool AmbientOcclusion
    {
        get { return GetVolumeComponent<AmbientOcclusion>().active; }
        set
        {
            GetVolumeComponent<AmbientOcclusion>().active = value;
        }
    }

    public T GetVolumeComponent<T>() where T : VolumeComponent
    {
        foreach (var component in GlobalVolume.profile.components)
        {
            if (component is T)
            {
                return component as T;
            }
        }
        return null;
    }


    public void Quit()
    {
        Application.Quit();
    }
}
