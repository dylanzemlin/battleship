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

public class OrbitMovement : MonoBehaviour{
    public Transform target; // Orbit around target
    public float speed = 50f; // speed of movement

    void Update()
    {
        // get distance from target
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 offset = new Vector3(0,0,0);
        
        if (Input.GetKey (KeyCode.LeftShift)){ // Move in strict coordinates
            Vector3 v = planeMovement();
            transform.Translate(v * Time.deltaTime * speed);
        } else { // Move in orbital coordinates
            Vector3 v = Orbit();
            transform.RotateAround(target.position, v, speed * Time.deltaTime);
        }
    }

    private Vector3 planeMovement() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey (KeyCode.Q)){
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey (KeyCode.Z)){
            p_Velocity += new Vector3(0, 0, -1);
        }
        return p_Velocity;
    }

    private Vector3 Orbit() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey (KeyCode.A)){ // W
            p_Velocity += transform.up;
        }
        if (Input.GetKey (KeyCode.D)){ // S
            p_Velocity += -transform.up;
        }
        if (Input.GetKey (KeyCode.S)){ // A
            p_Velocity += -transform.right;
        }
        if (Input.GetKey (KeyCode.W)){ // D
            p_Velocity += transform.right;
        }
        return p_Velocity.normalized;
    }

}
