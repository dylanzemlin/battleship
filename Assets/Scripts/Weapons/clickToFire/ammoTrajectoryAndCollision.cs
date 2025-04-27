using UnityEngine; // Import Unity engine functionalities (GameObjects, Rigidbody, etc.)
using System.Linq; // Import LINQ, used for searching through collections (like child Transforms)

[RequireComponent(typeof(Rigidbody))] // Force every object using this script to have a Rigidbody component
public class ammoTrajectoryAndCollision : MonoBehaviour
{
    // === Configurable Fields ===
    public float gravityStrength = 9.8f;                   // The strength of the artificial gravity pulling toward the planet center
    public Transform visualModel;                          // Reference to the visible mesh (optional), separated from the Rigidbody for smooth rotation
    public GameObject ammoPrefabToReload;                  // Prefab to instantiate a new ammo on the ship deck after firing
    public Transform reloadPointOnDeck;                    // The spawn location on the ship deck for new ammo
    public Rigidbody rb;                                   // The Rigidbody controlling the ammo's physics
    public Transform planetCenter;                         // The "gravity center" object (e.g., the planet)

    public bool hasBeenFired = false;                      // True once the ammo is fired, used to prevent early collision effects
    private GameObject firingShip;
    // === Initialization ===
    private void Start()
    {
        hasBeenFired = false;                              // Initially not fired
        rb.isKinematic = true;                             // Rigidbody won't simulate physics until firing (it stays still)
        
        // If no planetCenter assigned manually, find GameObject named "Planet"
        if (planetCenter == null)
        {
            GameObject planet = GameObject.Find("Planet"); // Look for a GameObject named "Planet"
            if (planet != null)
                planetCenter = planet.transform;
            else
                Debug.LogWarning("BaseAmmo: No 'Planet' GameObject found."); // Warn if missing
        }

        // If no reload point assigned, guess it from this object's name (e.g., Ammo1 → DeckSpawn1)
        if (reloadPointOnDeck == null)
        {
            string number = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value; // Extract number from name
            if (!string.IsNullOrEmpty(number))
            {
                GameObject reloadPoint = GameObject.Find("DeckSpawn" + number); // Find corresponding DeckSpawn
                if (reloadPoint != null)
                    reloadPointOnDeck = reloadPoint.transform;
                else
                    Debug.LogWarning($"BaseAmmo: No DeckSpawn{number} found for {name}"); // Warn if missing
            }
        }
    }

    // === Fired from Cannon ===
    public void Fire(Vector3 direction, float speed, GameObject ship)
    {
        hasBeenFired = true;                               // Mark that it has now been fired

        rb.isKinematic = false;                            // Enable physics so it can move
        rb.linearVelocity = direction.normalized * speed;  // Launch it with the given direction and speed
        firingShip = ship;
        
        // Rotate visual model (if any) to point in the firing direction
        if (visualModel != null)
            visualModel.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        else
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        Debug.Log($"{name} fired with linear velocity: {rb.linearVelocity}");

        // === Respawn new ammo on the deck after firing ===
        if (ammoPrefabToReload != null && reloadPointOnDeck != null)
        {
            GameObject newAmmo = Instantiate(ammoPrefabToReload, reloadPointOnDeck.position, reloadPointOnDeck.rotation);

            // Freeze new ammo physics so it stays still on the deck
            Rigidbody newRb = newAmmo.GetComponent<Rigidbody>();
            if (newRb != null)
                newRb.isKinematic = true;

            // Extract number from new ammo name
            string newAmmoNumber = System.Text.RegularExpressions.Regex.Match(newAmmo.name, @"\d+").Value;

            if (int.TryParse(newAmmoNumber, out int num))
            {
                // Adjust rotation of new ammo based on its number (to face cannons correctly)
                if (num == 1 || num == 3)
                {
                    newAmmo.transform.Rotate(0f, -90f, 0f, Space.Self);
                }
                else if (num == 2 || num == 4)
                {
                    newAmmo.transform.Rotate(0f, 90f, 0f, Space.Self);
                }
            }

            // Give the new ammo the same reload spawn point
            ammoTrajectoryAndCollision newAmmoScript = newAmmo.GetComponent<ammoTrajectoryAndCollision>();
            if (newAmmoScript != null)
                newAmmoScript.reloadPointOnDeck = this.reloadPointOnDeck;

            // Setup the cannon loading script for the new ammo
            ammoDeckToCannon cannonLoader = newAmmo.GetComponent<ammoDeckToCannon>();
            if (cannonLoader != null)
            {
                // Try to extract the number again
                string number = System.Text.RegularExpressions.Regex.Match(newAmmo.name, @"\d+").Value;

                if (!string.IsNullOrEmpty(number))
                {
                    GameObject cannonObj = GameObject.Find("Cannon1" + number); // <<-- Potential typo? Should it be "Cannon" + number?

                    if (cannonObj != null)
                    {
                        // Assign this cannon to the ammo's loader
                        cannonLoader.cannon = cannonObj.GetComponent<clickToFireCannon>();

                        // Find the specific spawn point inside the cannon
                        Transform point = cannonObj.GetComponentsInChildren<Transform>()
                                                .FirstOrDefault(t => t.name == "Point" + number);

                        if (point != null)
                            cannonLoader.SetSpawnPoint(point); // Set spawn point
                        else
                            Debug.LogWarning($"No spawn point named 'Point{number}' found in {cannonObj.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"No cannon found for Ammo{number}");
                    }
                }
            }

            Debug.Log($"New {newAmmo.name} spawned at deck location!");
        }
    }

    // === Physics and Gravity Calculations ===
    private void FixedUpdate()
    {
        // Apply custom gravity toward planet center every physics update
        if (planetCenter != null)
        {
            Vector3 gravityDirection = (planetCenter.position - transform.position).normalized;
            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
        }

        // Smoothly rotate to match flight direction
        if (rb.linearVelocity != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);

            if (visualModel != null)
                visualModel.rotation = Quaternion.Slerp(visualModel.rotation, rotation, Time.deltaTime * 5f); // Smoother turning
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    // === Collision Logic ===
    private void OnCollisionEnter(Collision other)
    {
        if (!hasBeenFired) return; // Ignore early collisions (while sitting on deck)

        Debug.Log($"Collision detected with: {other.collider.gameObject.name}");

        if (other.collider.gameObject.name.Contains("Cannon")) return; // Ignore collisions with cannons

        if (other.collider.gameObject.name.Contains("Pirate Ship"))
        {
            // Check all masts on the ship to make one fall
            foreach (Transform child in other.transform)
            {
                if (child.name.Contains("Mast"))
                {
                    MastFallOnHit mastFall = child.GetComponent<MastFallOnHit>();
                    if (mastFall != null && !mastFall.shouldFall)
                    {
                        mastFall.TriggerFall(); // Make the mast fall
                        break; // Only trigger one mast per hit
                    }
                }
            }

            Destroy(gameObject); // Destroy the fired ammo after hitting the ship
            return;
        }

        // If collided with something else (likely water) - create splash

        // Setup splash particle effect
        ParticleSystem splash = GameController.Instance.waterSplash;
        splash.transform.position = transform.position;

        // Align splash to be "up" relative to the planet surface
        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;
        splash.transform.forward = planetNormal;
        splash.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);
        splash.Play(); // Play splash particles

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
