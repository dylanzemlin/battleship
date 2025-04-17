using UnityEngine;

public class MakeFish : MonoBehaviour
{
    public Transform target; // Orbit around target
    // Reference to the FBX prefab of fish
    public GameObject fbxPrefab;
   // Gameobjects created
   private GameObject fish;

    public float speed = 1f;
    public float radius = 5f;
    private float angle = 0f;
    void Start()
    {
        Vector3 start = new Vector3(0,0,0);
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null) {
            float x_size = renderer.bounds.size.x;
            float y_size = renderer.bounds.size.y;
            float z_size = renderer.bounds.size.z;
            start = new Vector3(target.position.x, target.position.y , target.position.z+ (0.49f*z_size));
        }
        if (fbxPrefab != null && start != Vector3.zero)
        {
            // Instantiate the FBX prefab at the origin (0, 0, 0)
            fish = Instantiate(fbxPrefab, start, Quaternion.identity);
            fish.transform.Rotate(90f, 0f, 0f);
        }
        else
        {
            Debug.LogError("Can not find 'fish' or earth is not found.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the angle based on time and speed
        angle += speed * Time.deltaTime;

        // Calculate the X and Y positions using the polar to Cartesian conversion
        float x = radius * Mathf.Cos(angle);  // X = r * cos(θ)
        float y = radius * Mathf.Sin(angle);  // Y = r * sin(θ)

        // Rotate fish around earth
        fish.transform.RotateAround(target.position, new Vector3(x, y, transform.position.z), speed * Time.deltaTime);
    }
}
