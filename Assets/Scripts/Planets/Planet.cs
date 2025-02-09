using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Planet : MonoBehaviour
{
    public float radius = 1;
    public int resolution = 3;

    // Components
    private MeshFilter _filter;
    private MeshRenderer _renderer;

    void Start()
    {
        _filter = gameObject.GetComponent<MeshFilter>();
        _renderer = gameObject.GetComponent<MeshRenderer>();

        Regenerate();
    }

    public void Regenerate()
    {
        SphereMesh sphereMesh = new(resolution);
        Mesh mesh = new()
        {
            vertices = sphereMesh.Vertices,
            triangles = sphereMesh.Triangles
        };
        mesh.RecalculateNormals();
        _filter.mesh = mesh;

        transform.localScale = Vector3.one * radius;
    }
}
