using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration
{
    public static (Vector3[] vertices, int[] tris) Generate(Vector3[] verts, int[] tris, TerrainOptions options)
    {
        List<Vector3> vertices = new(verts);
        List<int> triangles = new(tris);

        // Adjust vertex heights using Perlin noise
        for (int i = 0; i < vertices.Count; i++)
        {
			Vector3 vertex = vertices[i];

            float noise = Mathf.PerlinNoise(vertex.x * options.noiseScale + options.noiseOffset, vertex.z * options.noiseScale + options.noiseOffset);
            float height = noise * options.noiseStrength * options.heightScale;
            vertices[i] = vertex.normalized * (1 + height);
        }

        return (vertices.ToArray(), triangles.ToArray());
    }

    [System.Serializable]
    public struct TerrainOptions
    {
        public float heightScale;
        public float noiseScale;
        public float noiseStrength;
        public float noiseOffset;
    }
}
