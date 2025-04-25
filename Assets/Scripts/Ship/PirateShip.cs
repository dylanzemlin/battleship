using UnityEngine;

public class PirateShip : MonoBehaviour
{
    public bool isSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSelected = false;
    }

    // Make ship selected
    public void selectShip()
    {
        isSelected = true;
    }

    // Deselect ship
    public void deselectShip()
    {
        isSelected = false;
    }
}
