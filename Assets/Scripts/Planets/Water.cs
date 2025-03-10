using LazySquirrelLabs.SphereGenerator.Generators;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    public float radius = 1;
    public ushort resolution = 3;

    // Components
    private MeshFilter _filter;
    private bool _dirty;

    void Start()
    {
        _filter = gameObject.GetComponent<MeshFilter>();
        Regenerate();
    }

    public void Regenerate()
    {
        SphereGenerator gen = new CubeSphereGenerator(radius, resolution);
        Mesh mesh = gen.Generate();

        // Generate sphere uvs
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            uvs[i] = new Vector2(v.x, v.z); // lol
        }
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        _filter.mesh = mesh;
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
