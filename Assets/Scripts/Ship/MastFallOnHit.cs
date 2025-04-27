using UnityEngine; // Import Unity's core library for GameObjects, Transforms, MonoBehaviour, etc.

public class MastFallOnHit : MonoBehaviour
{
    public bool shouldFall = false; // Whether the mast should currently start falling

    public float fallSpeedY = 90f;    // How fast the mast spins around its local Y-axis (degrees per second)
    public float twistSpeedZ = 30f;   // How fast the mast twists sideways around its local Z-axis (degrees per second)
    public float twistDirection = 1f; // +1 for twisting right, -1 for twisting left (manual control)

    private float fallProgress = 0f;  // How much the mast has already spun (in degrees); stops spinning after 90 degrees
    private bool hasStartedSinking = false; // Whether the mast has finished falling and started sinking into the water

    private void Update()
    {
        if (shouldFall)
        {
            FallSpinWithTwist(); // Continuously call the fall behavior once triggered
        }
    }

    public void TriggerFall()
    {
        shouldFall = true; // External call to start the fall sequence
    }

    private void FallSpinWithTwist()
    {
        if (fallProgress < 90f) // Step 1: Rotating and twisting phase
        {
            float spinStepY = fallSpeedY * Time.deltaTime; // Calculate how much to spin around Y-axis this frame
            float twistStepZ = twistSpeedZ * Time.deltaTime * twistDirection; // Calculate how much to twist around Z-axis this frame, using twistDirection

            // Apply rotation around the local Y-axis to simulate falling spin
            transform.Rotate(Vector3.up * spinStepY, Space.Self);

            // Apply rotation around the local Z-axis to simulate sideways twist
            transform.Rotate(Vector3.forward * twistStepZ, Space.Self);

            fallProgress += spinStepY; // Accumulate how much spinning has happened so far
        }
        else // Step 2: Sinking phase (after finishing 90° fall)
        {
            if (!hasStartedSinking)
            {
                hasStartedSinking = true; // Mark that sinking should now start
            }

            if (hasStartedSinking)
            {
                // Slowly move the mast downward (sink into water or ground)
                transform.position += Vector3.down * 2f * Time.deltaTime;
            }

            // Step 3: Final cleanup - Once sunk far enough (below -10 on the Y-axis), destroy the mast GameObject
            if (transform.position.y < -10f)
            {
                Destroy(gameObject); // Remove this mast completely from the scene
            }
        }
    }
}
