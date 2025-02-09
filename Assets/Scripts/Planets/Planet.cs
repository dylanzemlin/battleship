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
        Mesh mesh = new()
        {
            vertices = sphereMesh.Vertices,
            triangles = sphereMesh.Triangles
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
