using UnityEngine; // Unity’s core engine library, providing GameObject, Rigidbody, Transform, etc.

// Ensure that any GameObject using this script automatically has a Rigidbody attached
[RequireComponent(typeof(Rigidbody))]
public class BaseAmmo : MonoBehaviour
{
    // === CONFIGURABLE PROPERTIES ===

    public Transform planetCenter;     // The Transform representing the planet’s center (for gravity pull)
    public float gravityStrength = 9.8f; // The force applied as custom "gravity" towards the planet
    public Transform visualModel;      // (Optional) Separate visual model for more flexible rotation control (vs. Rigidbody rotation)
    
    public Rigidbody rb;               // The Rigidbody component that handles physical movement

    // === FIRING LOGIC ===

    // Private field to remember which ship fired the ammo (important for effects like treasure searches)
    private GameObject firingShip;

    // === AMMO FIRING FUNCTION ===
    public void Fire(Vector3 direction, float speed, GameObject ship)
    {
        // Set the Rigidbody's linear velocity to shoot it in the given direction at the given speed
        rb.linearVelocity = direction.normalized * speed;
        
        // Record which ship fired this ammo
        firingShip = ship;
    }

    // === PHYSICS & GRAVITY BEHAVIOR ===
    private void FixedUpdate()
    {
        // Every physics update (fixed time steps) apply custom gravity toward the planet's center
        if (planetCenter != null)
        {
            // Calculate the direction from ammo to the planet center (normalized)
            Vector3 gravityDirection = (planetCenter.position - transform.position).normalized;
            
            // Apply a constant force toward the planet center (like gravity)
            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
        }

        // Rotate ammo to face the way it’s flying (makes flight look natural)
        if (rb.linearVelocity != Vector3.zero)
        {
            // Determine which way it should face based on velocity
            Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);

            // Smoothly rotate the ammo to that direction over time
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    // === COLLISION AND TRIGGER LOGIC ===
    private void OnTriggerEnter(Collider other)
    {
        // If the ammo collides with a cannon, ignore it (don't want to react to firing cannons)
        if (other.gameObject.name.Contains("Cannon"))
        {
            return; // Skip rest of the code
        }

        // === COLLISION WITH PIRATE SHIPS ===

        // If the ammo collides with a ship called "PirateShipClick"
        if (other.gameObject.name.Contains("PirateShipClick"))
        {
            // Check all child objects of the ship for masts
            foreach (Transform child in other.transform)
            {
                if (child.name.Contains("Mast")) // Look for parts named "Mast" (centerMast, sternMast, etc.)
                {
                    // Try to get the MastFallOnHit component to trigger its fall
                    MastFallOnHit mastFall = child.GetComponent<MastFallOnHit>();
                    if (mastFall != null && !mastFall.shouldFall)
                    {
                        mastFall.TriggerFall(); // Start falling animation
                        break; // Stop after triggering one mast
                    }
                }
            }

            Destroy(gameObject); // Destroy ammo after hitting ship
            return; // Exit
        }

        // Similarly, if the ammo hits a ship called "Pirate Ship"
        if (other.gameObject.name.Contains("Pirate Ship"))
        {
            // Same mast falling logic
            foreach (Transform child in other.transform)
            {
                if (child.name.Contains("Mast"))
                {
                    MastFallOnHit mastFall = child.GetComponent<MastFallOnHit>();
                    if (mastFall != null && !mastFall.shouldFall)
                    {
                        mastFall.TriggerFall();
                        break;
                    }
                }
            }

            Destroy(gameObject); // Destroy ammo after impact
            return;
        }

        // === IF HITTING SOMETHING ELSE (LIKE WATER) ===

        // Retrieve water splash particle system from a central GameController instance
        ParticleSystem system = GameController.Instance.waterSplash;

        // Move splash effect to where the collision happened
        system.transform.position = transform.position;

        // Find the surface normal at the point of impact (perpendicular to the planet surface)
        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;

        // Align the particle effect to face outward from planet
        system.transform.forward = planetNormal;
        system.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);

        // Play the splash particle effect
        system.Play();

        // === ADDITIONAL FISH SCATTER PARTICLES ===

        // Instantiate a new fish scatter particle effect at impact point
        GameObject fishScatterInstance = Instantiate(GameController.Instance.fishScatterParticlePrefab, transform.position, Quaternion.identity);
        
        if (fishScatterInstance == null) 
        {
            Debug.LogWarning("Could not instantiate the fishScatterParticlePrefab from GameController.Instance");
        } 
        else 
        {
            // Align fish scatter to surface normal
            fishScatterInstance.transform.forward = planetNormal;
            fishScatterInstance.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);

            // Play the fish scatter particles
            ParticleSystem fishScatterParticles = fishScatterInstance.GetComponent<ParticleSystem>();
            if (fishScatterParticles == null) 
            {
                Debug.LogWarning("Could not access fish Scatter particle system from fishScatterInstance");
            } 
            else 
            {
                fishScatterParticles.Play();
            }
        }

        // === TREASURE SEARCH MINI-GAME ON WATER IMPACT ===

        // Tell the GameController to potentially find treasure at this impact location
        GameController.Instance.GetComponent<TreasureAnimation>().searchForTreasure(
            firingShip,          // The ship that fired the ammo
            transform.position,  // Impact position
            planetNormal,        // Which way is "up" at this point (relative to planet)
            planetCenter.position // Center of planet
        );

        // === FINAL CLEANUP ===

        Destroy(gameObject); // Destroy the fired ammo after handling collision and effects
    }
}
