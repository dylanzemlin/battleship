using UnityEngine;

// This script allows a user to move an ammo GameObject on the ship deck
// into the cannon's loading position when the player clicks it.
public class AmmoDeckToCannon : MonoBehaviour
{
    // This is the location inside the cannon where the ammo should be positioned
    // This is assign in the Inspector by dragging the "spawn point" inside the cannon
    public Transform AmmoInCannonSpawnPoint;

    // Called automatically by Unity when the object is clicked in the Game view
    private void OnMouseDown()
    {
        // Check to make sure a spawn point has been assigned
        if (AmmoInCannonSpawnPoint != null)
        {
            // Move the current ammo GameObject (this object) to the cannon's spawn point
            transform.position = AmmoInCannonSpawnPoint.position;

            // Align its rotation with the cannon's spawn point
            transform.rotation = AmmoInCannonSpawnPoint.rotation;

            // Parent the ammo to the cannon so it moves with the cannon if needed
            transform.SetParent(AmmoInCannonSpawnPoint);

        }
    }
}


