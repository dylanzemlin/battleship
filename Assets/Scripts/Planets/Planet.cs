using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Planet : MonoBehaviour
{
    public float radius = 1;
    public int resolution = 3;
    public float rotationSpeed = 5;
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

    public static Vector3[] CalculateNormals(Vector3[] vertices, int[] triangles)
    {
        Vector3[] normals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int index0 = triangles[i * 3 + 0];
            int index1 = triangles[i * 3 + 1];
            int index2 = triangles[i * 3 + 2];

            Vector3 v0 = vertices[index0];
            Vector3 v1 = vertices[index1];
            Vector3 v2 = vertices[index2];

            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            normals[index0] += normal;
            normals[index1] += normal;
            normals[index2] += normal;
        }

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = normals[i].normalized;
        }

        return normals;
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
            colors = colors,
            normals = CalculateNormals(sphereMesh.Vertices, sphereMesh.Triangles)
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

        // Rotate the planet given the speed on two axes
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnValidate() {
        _dirty = true;
    }
}
