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
    }
}
