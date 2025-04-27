using UnityEngine;
using System.Linq;

public class ammoDeckToCannon : MonoBehaviour
{
    private Transform ammoInCannonSpawnPoint; // The position inside the cannon where this ammo will be placed
    public clickToFireCannon cannon; // Reference to the cannon this ammo belongs to

    private void Start()
    {
        // Extract the number from this ammo’s name (e.g., "Ammo3" → "3")
        string cleanName = name.Replace("(Clone)", "").Trim();
        string number = System.Text.RegularExpressions.Regex.Match(cleanName, @"\d+").Value;
        Debug.Log($"Ammunition {name} (cleaned to {cleanName}) extracted number: {number}");


        if (!string.IsNullOrEmpty(number))
        {
            // Find the matching cannon by name
            GameObject cannonObj = GameObject.Find("Cannon1" + number);
            if (cannonObj != null)
            {
                Debug.Log($"Found cannon: {cannonObj.name}");
                cannon = cannonObj.GetComponent<clickToFireCannon>();

                // Find the cannon’s internal spawn point (e.g., "Point3")
                Transform spawn = cannonObj.GetComponentsInChildren<Transform>()
                                           .FirstOrDefault(t => t.name == "Point" + number);

                if (spawn != null)
                {
                    ammoInCannonSpawnPoint = spawn;
                    Debug.Log($"Spawn point assigned for {name} → {spawn.name}");
                }
                else
                {
                    Debug.LogWarning($"Cannon1{number} has no child named 'Point{number}'");
                }
            }
            else
            {
                Debug.LogWarning($"No GameObject named 'Cannon1{number}' found.");
            }
        }
        else
        {
            Debug.LogWarning($"No number extracted from ammo name: {name}");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"CLICKED AMMO: {name}");

        if (ammoInCannonSpawnPoint != null && cannon != null)
        {
            // Move the ammo into position inside the cannon
            transform.position = ammoInCannonSpawnPoint.position;
            transform.rotation = ammoInCannonSpawnPoint.rotation;
            transform.SetParent(ammoInCannonSpawnPoint);
            
            // Now directly using cannon
            cannon.loadedAmmo = GetComponent<ammoTrajectoryAndCollision>(); 
            Debug.Log($"{name} successfully loaded into {cannon.name}");
            
        }
        else
        {
            Debug.LogWarning($"{name} missing cannon or spawn point!");
        }
    }

    // External method for setting the cannon's ammo loading point manually
    public void SetSpawnPoint(Transform spawn)
    {
        ammoInCannonSpawnPoint = spawn;
    }
}
