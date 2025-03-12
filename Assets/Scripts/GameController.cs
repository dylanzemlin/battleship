using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    // The list of ships that the player can select from.
    public GameObject[] ships;

    // The currently selected ship. Can be null if no ship is selected.
    public GameObject selectedShip;

    // The ghost ship that follows the mouse cursor when a ship is selected.
    private GameObject ghostShip;

    private List<GameObject> shipInstances = new();

    public void SetSelectedShipByIndex(int index)
    {
        // If the index is -1, deselect the currently selected ship
        if (index == -1)
        {
            if (selectedShip != null)
            {
                Destroy(ghostShip);
                ghostShip = null;
                selectedShip = null;
            }
            return;
        }

        // If there is already a selected ship, destroy it
        if (selectedShip != null)
        {
            Destroy(ghostShip);
            ghostShip = null;
            selectedShip = null;
        }

        // If the index is within the bounds of the ships array, set the selected ship
        if (index >= 0 && index < ships.Length)
        {
            selectedShip = ships[index];
            return;
        }

        Debug.LogError("Ship index " + index + " is out bounds: 0 <= index < " + ships.Length);
    }

    private void HandleGhostShip()
    {
        // If the ghost ship doesn't exist, create it
        if (ghostShip == null)
        {
            ghostShip = Instantiate(selectedShip);
            ghostShip.name = "GhostShip";
            ghostShip.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Disable the collider on the ghost ship
            Collider collider = ghostShip.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        // Raycast from the mouse cursor to the planet
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Water")))
        {
            // Position the ghost ship at the hit point
            ghostShip.transform.position = hit.point;

            // Rotate the ghost ship to face the planet
            Vector3 waterNormal = hit.normal;
            ghostShip.transform.up = waterNormal;

            // Push the ship into the water a bit
            ghostShip.transform.position -= waterNormal * 1.3f;
        }

        // Check for mouse button input to place the ship
        if (Input.GetMouseButtonDown(0))
        {
            // Ensure they are clicking on the water
            if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Water"))
            {
                return;
            }

            // Instantiate the selected ship at the ghost ship's position and rotation
            GameObject newShip = Instantiate(selectedShip, ghostShip.transform.position, ghostShip.transform.rotation);
            shipInstances.Add(newShip);

            // Destroy the ghost ship
            Destroy(ghostShip);
            ghostShip = null;

            // Deselect the ship
            selectedShip = null;
        }
    }

    void Update()
    {
        if (selectedShip != null)
        {
            HandleGhostShip();
        }
    }
}