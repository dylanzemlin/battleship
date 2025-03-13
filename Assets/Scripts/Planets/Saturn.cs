using UnityEngine;

public class Saturn : MonoBehaviour
{
   public Transform target; // Orbit around target
    // Reference to the FBX prefab of fish
    public GameObject fbxPrefab;
   // Gameobjects created
   private GameObject saturn;

    public float speed = 10f;
    private float radius;

    void Start()
    {
        Vector3 start = new Vector3(0,0,0);
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null) {
            float x_size = renderer.bounds.size.x;
            float y_size = renderer.bounds.size.y;
            float z_size = renderer.bounds.size.z;
            radius = 2*z_size;
            start = new Vector3(target.position.x + x_size, target.position.y + (0.5f * y_size), target.position.z + radius);
        }
        if (fbxPrefab != null && start != Vector3.zero)
        {
            // Instantiate the FBX prefab at the origin (0, 0, 0)
            saturn = Instantiate(fbxPrefab, start, Quaternion.identity);
            saturn.transform.localScale = new Vector3(100f, 100f, 10f);
            // fish.transform.Rotate(90f, 0f, 0f);
        }
        else
        {
            Debug.LogError("Can not find 'saturn' prefab or target object.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // make saturn orbit around earth
        saturn.transform.RotateAround(target.position, new Vector3(target.position.x + speed, target.position.y + speed, transform.position.z), speed * Time.deltaTime);
    }
}
