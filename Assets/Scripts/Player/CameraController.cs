using UnityEngine;

public class PlanetCameraController : MonoBehaviour
{
    public GameObject planet; // Assign the planet GameObject in the inspector
    public float zoomSpeed = 10f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 50f;
    
    private float planetRadius;
    private float minZoom;
    private float maxZoom;
    private float currentZoom;
    private Vector3 cameraOffset;

    void Start()
    {
        if (planet == null)
        {
            Debug.LogError("Planet is not assigned!");
            return;
        }

        planetRadius = planet.GetComponent<Planet>().radius;
        minZoom = planetRadius * 1.2f;
        maxZoom = planetRadius * 3f;
        currentZoom = minZoom + (maxZoom - minZoom) / 6f;
        cameraOffset = new Vector3(0, currentZoom, 0);
        transform.position = planet.transform.position + cameraOffset;
        transform.LookAt(planet.transform.position);
    }

    void Update()
    {
        if (planet == null) return;

        HandleZoom();
        HandleMovement();
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        transform.position = planet.transform.position + cameraOffset.normalized * currentZoom;
    }

    void HandleMovement()
    {
        float horizontal = -Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = -Input.GetAxis("Vertical"); // W/S or Up/Down

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isShiftPressed)
        {
            horizontal *= 2;
            vertical *= 2;
        }

        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(right, transform.position - planet.transform.position).normalized;
        
        Vector3 movement = (right * horizontal + forward * vertical) * moveSpeed * Time.deltaTime;
        cameraOffset = Quaternion.AngleAxis(movement.magnitude * Mathf.Rad2Deg / planetRadius, Vector3.Cross(movement, transform.position - planet.transform.position)) * cameraOffset;

        transform.position = planet.transform.position + cameraOffset.normalized * currentZoom;
        transform.LookAt(planet.transform.position);
    }
}