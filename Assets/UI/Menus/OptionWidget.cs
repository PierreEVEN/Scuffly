using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class OptionWidget : MonoBehaviour
{
    static Volume _globalVolume;
    public static Volume GlobalVolume
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
        LandscapeQuality = 100;
        LandscapeViewDistance = 4;
        FolliageQuality = 1.5f;
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
        LandscapeQuality = 80;
        LandscapeViewDistance = 3;
        FolliageQuality = 1f;
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
        LandscapeQuality = 50;
        LandscapeViewDistance = 2;
        FolliageQuality = 0.6f;
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
        LandscapeQuality = 25;
        LandscapeViewDistance = 1;
        FolliageQuality = 0.2f;
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
        LandscapeQuality = 10;
        LandscapeViewDistance = 0;
        FolliageQuality = 0f;
        Shadows = false;
        ContactShadows = false;
    }



    public static int LandscapeViewDistance
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


    public static int LandscapeQuality
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


    public static float FolliageQuality
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

    public static int GlobalShaderQuality
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


    public static bool Shadows
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

    public static bool VolumetricClouds
    {
        get { return GetVolumeComponent<VolumetricClouds>().active; }
        set
        {
            GetVolumeComponent<VolumetricClouds>().active = value;
        }
    }


    public static bool ContactShadows
    {
        get { return GetVolumeComponent<ContactShadows>().enable.value; }
        set
        {
            GetVolumeComponent<ContactShadows>().enable.value = value;
        }
    }

    public static bool Fog
    {
        get { return GetVolumeComponent<Fog>().enabled.value; }
        set
        {
            GetVolumeComponent<Fog>().enabled.value = value;
        }
    }

    public static bool Bloom
    {
        get { return GetVolumeComponent<Bloom>().active; }
        set
        {
            GetVolumeComponent<Fog>().active = value;
        }
    }

    public static bool AmbientOcclusion
    {
        get { return GetVolumeComponent<AmbientOcclusion>().active; }
        set
        {
            GetVolumeComponent<AmbientOcclusion>().active = value;
        }
    }

    public static T GetVolumeComponent<T>() where T : VolumeComponent
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


    public static void Quit()
    {
        Application.Quit();
    }
}
