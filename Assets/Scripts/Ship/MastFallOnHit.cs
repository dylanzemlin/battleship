using UnityEngine;

public class MastFallOnHit : MonoBehaviour
{
    public bool shouldFall = false;

    public float fallSpeedY = 90f;    // Spin around Y (degrees per second)
    public float twistSpeedZ = 30f;   // Twist around Z (degrees per second)
    public float twistDirection = 1f; // +1 = right twist, -1 = left twist (MANUAL CONTROL)

    private float fallProgress = 0f;
    private bool hasStartedSinking = false;

    private void Update()
    {
        if (shouldFall)
        {
            FallSpinWithTwist();
        }
    }

    public void TriggerFall()
    {
        shouldFall = true;
    }

    private void FallSpinWithTwist()
    {
        if (fallProgress < 90f)
        {
            float spinStepY = fallSpeedY * Time.deltaTime;
            float twistStepZ = twistSpeedZ * Time.deltaTime * twistDirection;

            // Rotate around local Y axis (spin)
            transform.Rotate(Vector3.up * spinStepY, Space.Self);

            // Twist around local Z axis
            transform.Rotate(Vector3.forward * twistStepZ, Space.Self);

            fallProgress += spinStepY;
        }
        else
        {
            if (!hasStartedSinking)
            {
                hasStartedSinking = true;
            }

            if (hasStartedSinking)
            {
                // Slowly sink
                transform.position += Vector3.down * 2f * Time.deltaTime;
            }

            if (transform.position.y < -10f)
            {
                Destroy(gameObject);
            }
        }
    }
}
