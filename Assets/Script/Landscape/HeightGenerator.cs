using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/**
 *  @Author : Pierre EVEN
 */

public class HeightGenerator
{
    public static bool RectIntersect(Rect a, Rect b)
    {
        return b.xMax > a.xMin &&
                a.xMax > b.xMin &&
                b.yMax > a.yMin &&
                a.yMax > b.yMin;
    }

    public List<LandscapeModifier> modifiers = new List<LandscapeModifier>();
    public OnUpdateSectionEvent OnUpdateRegion = new OnUpdateSectionEvent();

    public class OnUpdateSectionEvent : UnityEvent<Rect> { }

    public void MoveModifier(LandscapeModifier modifier, Rect oldBounds, Rect newbounds)
    {
        if (!modifiers.Contains(modifier))
            modifiers.Add(modifier);

        float minX = Mathf.Min(oldBounds.xMin, newbounds.xMin);
        float minY = Mathf.Min(oldBounds.yMin, newbounds.yMin);
        float maxX = Mathf.Max(oldBounds.xMax, newbounds.xMax);
        float maxY = Mathf.Max(oldBounds.yMax, newbounds.yMax);
        Rect updateBound = Rect.MinMaxRect(minX, minY, maxX, maxY);
        updateBound.size *= 2;
        OnUpdateRegion.Invoke(updateBound);
    }

    public void RemoveModifier(LandscapeModifier modifier)
    {
        modifiers.Remove(modifier);
    }

    private HeightGenerator() {}

    private static HeightGenerator GlobalInstance;
    public static HeightGenerator Singleton
    {
        get
        {
            if (GlobalInstance == null)
                GlobalInstance = new HeightGenerator();
            return GlobalInstance;
        }
    }


    public float GetAltitudeAtLocation(float posX, float posZ)
    {
        // handle modifiers
        float altitudeOverride = 0;
        float incidence = 0;
        int maxPriority = 0;
        foreach (var modifier in modifiers)
        {
            if (modifier.priority >= maxPriority)
            {
                if (modifier.worldBounds.Contains(new Vector2(posX, posZ)))
                {
                    maxPriority = modifier.priority;
                    incidence = modifier.GetIncidenceAtLocation(posX, posZ);
                    altitudeOverride = modifier.GetAltitudeAtLocation(posX, posZ);
                }
            }
        }
        if (incidence == 1)
            return altitudeOverride;

        posX *= 0.04f;
        posZ *= 0.04f;

        float mountainLevel = getMountainLevel(posX, posZ);

        float alt = mountainLevel * 800;

        float scale = 0.01f;
        float mountainNoise = (float)Math.Pow(Mathf.PerlinNoise(posX * scale, posZ * scale), 2) * 3000;

        alt += mountainLevel * mountainNoise;

        alt += getHillsLevel(posX, posZ, mountainLevel) * 200;

        alt = addBeaches(posX, posZ, alt);

        return Mathf.Lerp(alt, altitudeOverride, incidence);
    }

    float getMountainLevel(float posX, float posZ)
    {
        float scale = 0.001f;
        float level = 1.5f - Mathf.PerlinNoise(posX * scale, posZ * scale) * 1.5f;
        level -= 0.5f;

        return level;
    }

    float getHillsLevel(float posX, float posZ, float mountainLevel)
    {
        float scale = 0.01f;
        return Mathf.PerlinNoise(posX * scale, posZ * scale) * (1 - (float)Math.Pow(Math.Abs(mountainLevel), 1));
    }

    float addBeaches(float posX, float posZ, float currentAltitude)
    {
        return currentAltitude;
    }
};



