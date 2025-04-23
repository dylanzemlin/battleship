using UnityEngine;

// This script controls the movement of ammo along a Bezier curve,
// detects when it hits something, and optionally reloads a new ammo on deck.
public class AmmoBezierMover : MonoBehaviour
{
    // === CONFIGURATION ===

    public Transform targetPoint;             // The target location the ammo should fly to
    public float height = 5f;                 // The height of the Bezier curve's arc
    public float duration = 2f;               // Time it takes for the ammo to reach the target

    public GameObject ammoPrefabToReload;     // Prefab to reload after hitting a target
    public Transform reloadPointOnDeck;       // Location on the ship deck to place the new ammo

    // === INTERNAL BEZIER STATE ===

    private float t = 0f;                     // Time parameter along the curve (0 to 1)
    private bool isFired = false;             // Flag to control whether the ammo is currently moving
    private Vector3 p0, p1, p2;               // Bezier control points

    // === BEZIER INIT ===

    // Call this to begin ammo flight from a given start position
    public void FireFrom(Vector3 startPosition)
    {
        p0 = startPosition;                   // Start point of the curve
        p2 = targetPoint.position;            // End point (the target)
        p1 = (p0 + p2) / 2f + Vector3.up * height;  // Midpoint raised to create an arc
        t = 0f;
        isFired = true;                       // Activate movement
    }

    // === FLIGHT UPDATE ===

    void Update()
    {
        // Skip if not in flight
        if (!isFired) return;

        // Advance the timer by deltaTime, scaled to match duration
        t += Time.deltaTime / duration;

        // Clamp t at 1 and stop movement once target is reached
        if (t > 1f)
        {
            t = 1f;
            isFired = false;
            Debug.Log("Ammo hit the target!");
        }

        // Compute position along the quadratic Bezier curve: (1−t)^2*p0 + 2(1−t)t*p1 + t^2*p2
        Vector3 pos = Mathf.Pow(1 - t, 2) * p0 +
                      2 * (1 - t) * t * p1 +
                      Mathf.Pow(t, 2) * p2;

        transform.position = pos;             // Update the ammo’s position
    }

    // === COLLISION HANDLING ===

    private void OnCollisionEnter(Collision collision)
    {
        string targetName = collision.gameObject.name;

        // Choose action based on what was hit
        switch (targetName)
        {
            case "targetMast":
                // Try to get the MastFallOnHit script from the target object
                var mast = collision.gameObject.GetComponent<MastFallOnHit>();
                if (mast != null)
                    mast.TriggerFall();      // Tell the mast to start falling
                break;

            case "targetStarboardStern":
                // Placeholder for future ship damage logic
                break;

            case "targetLand":
                // Placeholder for future land impact effects
                break;

            case "targetWater":
                // Placeholder for future water splash effects
                break;

            default:
                break;
        }

        // === RELOAD NEW AMMO ON DECK ===

        if (ammoPrefabToReload != null && reloadPointOnDeck != null)
        {
            // Instantiate a new clickable ammo object back on the deck
            Instantiate(ammoPrefabToReload, reloadPointOnDeck.position, reloadPointOnDeck.rotation);
            Debug.Log("New ammo reloaded on deck!");
        }

        // Remove the fired ammo after it hits
        Destroy(gameObject);
    }
}
