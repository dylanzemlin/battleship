using UnityEngine;

public class MastFallOnHit : MonoBehaviour
{
    public float fallSpeed = 30f; // Degrees per second
    public float fallAngle = 90f; // How far to fall

   // Keeps track of whether the mast should start falling
private bool isFalling = false;

// Tracks how much the mast has rotated so far
private float currentRotation = 0f;

void Update()
{
    // Only run the fall logic if falling has been triggered
    // AND the mast hasn't completed the full fall angle yet
    if (isFalling && currentRotation < fallAngle)
    {
        // Calculate how much we should rotate this frame based on time and speed
        float rotationStep = fallSpeed * Time.deltaTime;

        // Calculate how much rotation remains to reach the full fall angle
        float remaining = fallAngle - currentRotation;

        // Use the smaller of the two: rotate only what's left, or one step
        float actualStep = Mathf.Min(rotationStep, remaining);

        // Rotate the mast around the Z-axis (forward), like tipping sideways or forward
        // This assumes Z-axis is the correct axis based on your mast orientation
        transform.Rotate(Vector3.forward, actualStep);

        // Keep track of the total amount rotated so far
        currentRotation += actualStep;
    }
}

// Public method that starts the mast falling when called
public void TriggerFall()
{
    isFalling = true;  // activates the fall logic in Update()
    Debug.Log("Mast is falling!");  // helpful debug message in Console
}

}
