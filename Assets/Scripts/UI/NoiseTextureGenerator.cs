using UnityEngine;

public static class NoiseTextureGenerator
{
    public static Texture2D GenerateNoiseTexture(int width, int height, TerrainGeneration.TerrainOptions options)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 point = new Vector3((float)x / width, 0f, (float)y / height);
                float elevation = GenerateFractalNoise(point, options);
                Color color = new Color(elevation, elevation, elevation); // grayscale
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static float GenerateFractalNoise(Vector3 point, TerrainGeneration.TerrainOptions options)
    {
        float total = 0f;
        float frequency = options.baseFrequency;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < options.octaves; i++)
        {
            float noise = Mathf.PerlinNoise(
                point.x * frequency + options.noiseOffset,
                point.z * frequency + options.noiseOffset
            );

            total += noise * amplitude;
            maxValue += amplitude;

            amplitude *= options.persistence;
            frequency *= options.lacunarity;
        }

        float elevation = total / maxValue;
        elevation = Mathf.Clamp01(elevation - options.continentThreshold);
        return elevation;
    }
}
