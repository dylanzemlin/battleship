using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Planet : MonoBehaviour
{
    public float radius = 1;
    public int resolution = 3;
    public TerrainGeneration.TerrainOptions terrainOptions;

    // Components
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private bool _dirty;

    void Start()
    {
        _filter = gameObject.GetComponent<MeshFilter>();
        _renderer = gameObject.GetComponent<MeshRenderer>();

        Regenerate();
    }

    public void Regenerate()
    {
        SphereMesh sphereMesh = new(resolution, terrainOptions);

        // Calculate the minimum/maximum height of the planet from the vertices
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        foreach (Vector3 vertex in sphereMesh.Vertices)
        {
            float height = vertex.magnitude;
            if (height < minHeight)
            {
                minHeight = height;
            }
            if (height > maxHeight)
            {
                maxHeight = height;
            }
        }

        Color[] colors = new Color[sphereMesh.Vertices.Length];
        for (int i = 0; i < sphereMesh.Vertices.Length; i++)
        {
            float height = sphereMesh.Vertices[i].magnitude;
            float t = Mathf.InverseLerp(minHeight, maxHeight, height);
            colors[i] = terrainOptions.terrainGradient.Evaluate(t);
        }

        Mesh mesh = new()
        {
            vertices = sphereMesh.Vertices,
            triangles = sphereMesh.Triangles,
            colors = colors
        };
        mesh.RecalculateNormals();
        _filter.mesh = mesh;
        transform.localScale = Vector3.one * radius;
    }

    private void Update() {
        if (_dirty)
        {
            _dirty = false;
            Regenerate();
        }
    }

    private void OnValidate() {
        _dirty = true;
    }
}
