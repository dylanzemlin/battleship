using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class ammoTrajectoryAndCollision : MonoBehaviour
{
    // === Configurable Fields ===
    public float gravityStrength = 9.8f;                   // Simulated gravity strength
    public Transform visualModel;                          // Optional: the visible mesh (rotated separately from physics)
    public GameObject ammoPrefabToReload;                  // Prefab to respawn on the deck after firing
    public Transform reloadPointOnDeck;                    // Where the new ammo should respawn
    public Rigidbody rb;                                   // Physics body for ammo
    public Transform planetCenter;                         // Center of gravity for "planet-style" force

    private bool hasBeenFired = false;                     // Flag to ignore collisions before being fired

    // === Initialization ===
    private void Start()
    {
        hasBeenFired = false;
        rb.isKinematic = true; // Ammo shouldn't move until fired

        // Automatically assign gravity source
        if (planetCenter == null)
        {
            GameObject planet = GameObject.Find("Planet");
            if (planet != null)
                planetCenter = planet.transform;
            else
                Debug.LogWarning("BaseAmmo: No 'Planet' GameObject found.");
        }

        // Automatically assign deck spawn point based on name (e.g., Ammo2 → DeckSpawn2)
        if (reloadPointOnDeck == null)
        {
            string number = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
            if (!string.IsNullOrEmpty(number))
            {
                GameObject reloadPoint = GameObject.Find("DeckSpawn" + number);
                if (reloadPoint != null)
                    reloadPointOnDeck = reloadPoint.transform;
                else
                    Debug.LogWarning($"BaseAmmo: No DeckSpawn{number} found for {name}");
            }
        }
    }

    // === Fired from Cannon ===
    public void Fire(Vector3 direction, float speed, GameObject ship)
    {

        hasBeenFired = true;

        rb.isKinematic = false;
        rb.linearVelocity = direction.normalized * speed; // Launch the projectile

        // Force mesh to face the direction of motion
        if (visualModel != null)
            visualModel.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        else
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        Debug.Log($"{name} fired with linear velocity: {rb.linearVelocity}");

        // === Instantiate a new ammo back on deck ===
        if (ammoPrefabToReload != null && reloadPointOnDeck != null)
        {
            GameObject newAmmo = Instantiate(ammoPrefabToReload, reloadPointOnDeck.position, reloadPointOnDeck.rotation);

            // Freeze new ammo physics until clicked
            Rigidbody newRb = newAmmo.GetComponent<Rigidbody>();
            if (newRb != null)
                newRb.isKinematic = true;

            // Pass the reload spawn point to the new ammo
            ammoTrajectoryAndCollision newAmmoScript = newAmmo.GetComponent<ammoTrajectoryAndCollision>();
            if (newAmmoScript != null)
                newAmmoScript.reloadPointOnDeck = this.reloadPointOnDeck;

            // Setup ammo loader script on new ammo
            ammoDeckToCannon cannonLoader = newAmmo.GetComponent<ammoDeckToCannon>();
            if (cannonLoader != null) // Check if the cannonLoader (responsible for moving ammo into cannons) is assigned
            {
                // Try to extract a number (e.g., "1" from "Ammo1") from the new ammo object's name
                string number = System.Text.RegularExpressions.Regex.Match(newAmmo.name, @"\d+").Value;

                if (!string.IsNullOrEmpty(number)) // Make sure a number was successfully extracted
                {
                    // Find the cannon GameObject with a name matching "Cannon" + number (e.g., "Cannon1")
                    GameObject cannonObj = GameObject.Find("Cannon1" + number);
                    
                    if (cannonObj != null) // If such a cannon exists in the scene
                    {
                        // Assign the cannon's click-to-fire script to the cannonLoader so it knows which cannon to load
                        cannonLoader.cannon = cannonObj.GetComponent<clickToFireCannon>();

                        // Look for a child Transform under the cannon whose name matches "Point" + number (e.g., "Point1")
                        Transform point = cannonObj.GetComponentsInChildren<Transform>()
                                                .FirstOrDefault(t => t.name == "Point" + number);

                        if (point != null) // If the correct loading point is found
                            cannonLoader.SetSpawnPoint(point); // Set this point as the spawn (loading) position for the ammo
                        else
                            Debug.LogWarning($"No spawn point named 'Point{number}' found in {cannonObj.name}"); // Warn if the loading point is missing
                    }
                    else
                    {
                        Debug.LogWarning($"No cannon found for Ammo{number}"); // Warn if no cannon matches the extracted number
                    }
                }
            }

            // Print a message to the console saying the new ammo was spawned onto the deck
            Debug.Log($"New {newAmmo.name} spawned at deck location!");
        }
    }


    // === Physics and Gravity Calculations ===
    private void FixedUpdate()
    {
        // Pull toward planet center
        if (planetCenter != null)
        {
            Vector3 gravityDirection = (planetCenter.position - transform.position).normalized;
            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
        }

        // Smoothly rotate ammo (or visual mesh) to match its movement direction
        if (rb.linearVelocity != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);

            if (visualModel != null)
                visualModel.rotation = Quaternion.Slerp(visualModel.rotation, rotation, Time.deltaTime * 5f);
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    // === Collision Logic ===
    private void OnCollisionEnter(Collision other)
    {
        if (!hasBeenFired) return;

        Debug.Log($"Collision detected with: {other.collider.gameObject.name}");

        // Ignore hitting your own cannon
        if (other.collider.gameObject.name.Contains("Cannon")) return;

        // Ship impact logic
        if (other.collider.gameObject.name.Contains("Pirate Ship"))
        {
            Transform mastTransform = other.collider.transform.Find("centerMast");

            if (mastTransform != null)
            {
                MastFallOnHit mastFall = mastTransform.GetComponent<MastFallOnHit>();
                if (mastFall != null)
                    mastFall.TriggerFall();
            }

            Destroy(gameObject); // Destroy ammo after impact
            return;
        }

        // === Water splash effect ===
        ParticleSystem splash = GameController.Instance.waterSplash;
        splash.transform.position = transform.position;

        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;

        splash.transform.forward = planetNormal;
        splash.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);
        splash.Play();

        // === Fish particle effect ===
        GameObject fishEffect = Instantiate(GameController.Instance.fishScatterParticlePrefab, transform.position, Quaternion.identity);
        if (fishEffect != null)
        {
            fishEffect.transform.forward = planetNormal;
            fishEffect.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);

            ParticleSystem fishParticles = fishEffect.GetComponent<ParticleSystem>();
            if (fishParticles != null)
                fishParticles.Play();
            else
                Debug.LogWarning("Could not access fish particle system.");
        }

        Destroy(gameObject); // Destroy ammo after splash
    }
}
