using System.Diagnostics;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void OnShipSelected(int shipIndex)
    {
        GameController.Instance.SetSelectedShipByIndex(shipIndex);
    }

    void Update()
    {
        // If the user presses escape, deselect the currently selected ship
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameController.Instance.SetSelectedShipByIndex(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) // Cycle to the next ship
        {
            GameController.Instance.GetComponent<ShipSelection>().nextShip();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Cycle to the previous ship
        {
            GameController.Instance.GetComponent<ShipSelection>().previousShip();
        }
        if (Input.GetKeyDown(KeyCode.F)) // Focus on currrent ship
        {
            ShipSelection instance = GameController.Instance.GetComponentInParent<ShipSelection>();

            instance.moveCamera(instance.ships[instance.shipIndex]);
        }
    }
}
