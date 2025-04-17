using UnityEngine;

public class AmmoDeckToCannon : MonoBehaviour
{
    public Transform cannonSpawnPoint; // The spawnAmmo GameObject inside the cannon

    private void OnMouseDown()
    {
        if (cannonSpawnPoint != null)
        {
            transform.position = cannonSpawnPoint.position;
            transform.rotation = cannonSpawnPoint.rotation;

            // Optional: Parent it to the cannon for alignment
            transform.SetParent(cannonSpawnPoint);

            Debug.Log("Ammo moved into the cannon!");
        }
        else
        {
            Debug.LogWarning("Cannon spawn point not assigned!");
        }
    }
}

