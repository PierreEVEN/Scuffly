using System;
using UnityEngine;

public class HeightGenerator
{
    private HeightGenerator()
    {
    }

    private static HeightGenerator GlobalInstance;
    public static HeightGenerator Get()
    {
        if (GlobalInstance == null)
            GlobalInstance = new HeightGenerator();
        return GlobalInstance;
    }

    public float GetAltitudeAtLocation(float posX, float posY)
    {

        posX += 100000;
        posY += 10000;

        float mountainLevel = getMountainLevel(posX, posY);

        float alt = mountainLevel * 800;

        float scale = 0.01f;
        float mountainNoise = (float)Math.Pow(Mathf.PerlinNoise(posX * scale, posY * scale), 2) * 3000;

        alt += mountainLevel * mountainNoise;

        alt += getHillsLevel(posX, posY, mountainLevel) * 200;

        alt = addBeaches(posX, posY, alt);

        return alt;
    }

    float getMountainLevel(float posX, float posY)
    {
        float scale = 0.001f;
        float level = 1.5f - Mathf.PerlinNoise(posX * scale, posY * scale) * 1.5f;
        level -= 0.5f;

        return level;
    }

    float getHillsLevel(float posX, float posY, float mountainLevel)
    {
        float scale = 0.01f;
        return Mathf.PerlinNoise(posX * scale, posY * scale) * (1 - (float)Math.Pow(Math.Abs(mountainLevel), 1));
    }

    float addBeaches(float posX, float posY, float currentAltitude)
    {
        return currentAltitude;
    }
};



