using UnityEngine; // Import Unity's core engine functionalities
using System.Linq; // Import LINQ for easier searching through arrays (e.g., finding child objects)

public class ammoDeckToCannon : MonoBehaviour
{
    // === VARIABLES ===

    private Transform ammoInCannonSpawnPoint; // The Transform (position/rotation) inside the cannon where this ammo will be placed
    public clickToFireCannon cannon;           // The specific cannon this ammo belongs to (loaded by clicking)

    // === INITIALIZATION (on game start or instantiation) ===
    private void Start()
    {
        // Clean the object's name by removing "(Clone)" if it was instantiated
        string cleanName = name.Replace("(Clone)", "").Trim();
        
        // Extract the number from the name using a regular expression (e.g., "Ammo3" → "3")
        string number = System.Text.RegularExpressions.Regex.Match(cleanName, @"\d+").Value;

        Debug.Log($"Ammunition {name} (cleaned to {cleanName}) extracted number: {number}");

        if (!string.IsNullOrEmpty(number))
        {
            // Try to find a cannon GameObject named "Cannon1X" where X is the number (e.g., "Cannon13")
            GameObject cannonObj = GameObject.Find("Cannon1" + number); // NOTE: this may have an extra "1" typo (see below)

            if (cannonObj != null)
            {
                Debug.Log($"Found cannon: {cannonObj.name}");

                // Get the clickToFireCannon script attached to that cannon
                cannon = cannonObj.GetComponent<clickToFireCannon>();

                // Find the cannon's child spawn point named "PointX" (e.g., "Point3")
                Transform spawn = cannonObj.GetComponentsInChildren<Transform>()
                                           .FirstOrDefault(t => t.name == "Point" + number);

                if (spawn != null)
                {
                    // Store the spawn point for use when loading the ammo
                    ammoInCannonSpawnPoint = spawn;
                    Debug.Log($"Spawn point assigned for {name} → {spawn.name}");
                }
                else
                {
                    Debug.LogWarning($"Cannon1{number} has no child named 'Point{number}'"); // Warn if spawn point is missing
                }
            }
            else
            {
                Debug.LogWarning($"No GameObject named 'Cannon1{number}' found."); // Warn if cannon not found
            }
        }
        else
        {
            Debug.LogWarning($"No number extracted from ammo name: {name}"); // Warn if regex failed to find a number
        }
    }

    // === WHEN PLAYER CLICKS ON AMMO ===
    private void OnMouseDown()
    {
        Debug.Log($"CLICKED AMMO: {name}");

        if (ammoInCannonSpawnPoint != null && cannon != null)
        {
            // Move the ammo into the correct position and rotation inside the cannon
            transform.position = ammoInCannonSpawnPoint.position;
            transform.rotation = ammoInCannonSpawnPoint.rotation;

            // Parent the ammo object to the spawn point (so it stays in place even if cannon moves)
            transform.SetParent(ammoInCannonSpawnPoint);

            // Set this ammo as the loaded ammo inside the cannon
            cannon.loadedAmmo = GetComponent<ammoTrajectoryAndCollision>();

            Debug.Log($"{name} successfully loaded into {cannon.name}");
        }
        else
        {
            Debug.LogWarning($"{name} missing cannon or spawn point!"); // Warn if missing setup
        }
    }

    // === MANUAL SETUP (external call if needed) ===
    public void SetSpawnPoint(Transform spawn)
    {
        ammoInCannonSpawnPoint = spawn; // Allow external scripts to manually assign a spawn point if needed
    }
}
