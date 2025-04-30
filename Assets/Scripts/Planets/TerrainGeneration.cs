using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration
{
    public static (Vector3[] vertices, int[] tris) Generate(Vector3[] verts, int[] tris, TerrainOptions options)
    {
        List<Vector3> vertices = new(verts);
        List<int> triangles = new(tris);

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i].normalized;

            // Apply fractal noise (multiple layers of Perlin noise)
            float elevation = GenerateFractalNoise(vertex, options);

            // Calculate final vertex position
            float height = elevation * options.heightScale;
            vertex *= (1 + height);

            // Clamp to ocean floor if needed
            if (vertex.magnitude < options.oceanFloorHeight)
            {
                vertex = vertex.normalized * options.oceanFloorHeight;
            }

            vertices[i] = vertex;
        }

        return (vertices.ToArray(), triangles.ToArray());
    }

    private static float GenerateFractalNoise(Vector3 point, TerrainOptions options)
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

        // Optional tweak to emphasize continents (threshold)
        elevation = Mathf.Clamp01(elevation - options.continentThreshold);

        return elevation;
    }

    [System.Serializable]
    public struct TerrainOptions
    {
        public float heightScale;

        [Header("Fractal Noise")]
        public int octaves; // the number of noise layers
        public float baseFrequency; // how "zoomed in" the noise is, or when visualized how big the features are
        public float lacunarity; // detail increase per octave
        public float persistence; // how strong each layer is
        public float noiseOffset; // just shifts the noise pattern

        [Header("Continent Tweaking")]
        public float continentThreshold; // Elevation below this is ocean

        [Range(0, 1)]
        public float oceanFloorHeight;

        public Gradient terrainGradient;
    }
}
