// Movement that takes in user input and moves camera in an orbit manner around given object

/**
    Orbit around given target:
        W/S: vertical orbit
        D/A: horizontal orbit

    Planar/Rectangular movement:
        LeftShift + 
            W/S: Move y coordinate (Up/Down)
            D/A: Move x coordinate (Right/Left)
            Q/Z: Move Z coordinate (Zoom in/out)
*/

using UnityEngine;

public class CameraMovement : MonoBehaviour{
    public Transform target; // Orbit around target
    public float speed = 50f; // speed of movement
    public float zoomSpeed = 500f; // Speed of zoom
    public float minZoom = 500f; // Minimum zoom distance
    public float maxZoom = 5000f; // Maximum zoom distance
    public float rotationSpeed = 200f; // Speed of right-click rotation

    private Vector3 initialiOffset = new Vector3(0, 3200, -1000); // Hardcoded default placement.
    private Vector3 offset = new Vector3(0, 0, 0);

    void Start() {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null) {
            float x_size = renderer.bounds.size.x;
            float y_size = renderer.bounds.size.y;
            float z_size = renderer.bounds.size.z;
            Debug.Log(x_size + " " + y_size + " " + z_size);
            initialiOffset = new Vector3(target.position.x, target.position.y + (0.4f*y_size), -(target.position.z + x_size));
        }
        transform.position = initialiOffset;        
    }

    void Update() {
        // get distance from target
        Vector3 direction = (target.position - transform.position).normalized;
        bool isZRoll = false;
        
        if (Input.GetMouseButton(1)) {
            isZRoll = RightClickRotate();
        }

        if (Input.GetKey (KeyCode.LeftShift)){ // Move in strict coordinates
            Vector3 v = planeMovement();
            if (v != Vector3.zero) {
                transform.Translate(v * Time.deltaTime * speed);
            }
        } else { // Move in orbital coordinates
            Vector3 v = Orbit();
            if (v != Vector3.zero) {
                transform.RotateAround(target.position, v, speed * Time.deltaTime);
            }
        }
        
        if (!isZRoll){
            Zoom();
        }
    }

    private Vector3 planeMovement() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)){
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S)){
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Q)){
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.Z)){
            p_Velocity += new Vector3(0, 0, -1);
        }
        return p_Velocity;
    }

    private Vector3 Orbit() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.A)){ // W
            p_Velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.D)){ // S
            p_Velocity += -transform.up;
        }
        if (Input.GetKey(KeyCode.S)){ // A
            p_Velocity += -transform.right;
        }
        if (Input.GetKey(KeyCode.W)){ // D
            p_Velocity += transform.right;
        }
        return p_Velocity.normalized;
    }

    private void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            Vector3 direction = (transform.position - target.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);
            float newDistance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoom, maxZoom);
            transform.position = target.position + direction * newDistance;
        }
    }

    // return true if scroll wheel is used, false otherwise
    private bool RightClickRotate() {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // SHIFT + SCROLL + RIGHT CLICK TO CHANGE Z ROLL
        // RIGHT CLICK MOVE MOUSE TO CHANGE MOUSE POSITON
        float scrollZ = 0f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            scrollZ = Input.GetAxis("Mouse ScrollWheel") * rotationSpeed; // Roll based on mouse scroll while holding Shift
        }
        Vector3 currentRotation = transform.eulerAngles;
        Quaternion rotation = Quaternion.Euler(-mouseY, mouseX, scrollZ);
        transform.rotation *= rotation;

        // OLD MOUSE MOVEMENT - RIGHT CLICK ROTATES CAMERA AROUND EARTH
        // Vector3 axis = (target.position - transform.position).normalized;
        // transform.RotateAround(target.position, axis, mouseX);
        // Vector3 right = transform.right;
        // transform.RotateAround(transform.position, -right, -mouseY);
        return scrollZ == 0 ? false : true;
    }
}