using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipSelection : MonoBehaviour
{
    public Transform planetTrans;
    public GameObject[] ships;
    public int shipIndex;
    private int shipCount;

    private void Start()
    {
        ships = null;
        shipIndex = 0;
        shipCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Pirate Ship") == null)
        {
            Debug.LogError("No Ships Found");
            return;
        }
        else
        {
            ships = GameObject.FindGameObjectsWithTag("Pirate Ship");
            shipCount = ships.Length;
        }
    }

    public void nextShip()
    {
        PirateShip currentShip;
        PirateShip nextShip;

        // Sends warning to console if not enough ships or at the end of them
        if (shipCount < 1 || shipIndex == shipCount - 1)
        {
            Debug.LogError("Not enough ships or at the last ship");
            return;
        }

        // Grabs PirateShip component
        if (ships[shipIndex].GetComponent<PirateShip>() != null || ships[shipIndex + 1].GetComponent<PirateShip>() != null)
        {
            currentShip = ships[shipIndex].GetComponent<PirateShip>();
            nextShip = ships[shipIndex + 1].GetComponent<PirateShip>();
        }
        else
        {
            currentShip = null;
            nextShip = null;

            Debug.LogError("Current or next ship does not have PirateShip component");
        }

        currentShip.deselectShip();
        nextShip.selectShip();

        moveCamera(ships[shipIndex + 1]);
        shipIndex++;
    }

    public void previousShip()
    {
        PirateShip currentShip;
        PirateShip previousShip;

        // Sends warning to console if not enough ships or at the end of them
        if (shipCount < 1 || shipIndex == 0)
        {
            Debug.LogError("Not enough ships or at the firstship");
            return;
        }

        // Grabs PirateShip component
        if (ships[shipIndex].GetComponent<PirateShip>() != null || ships[shipIndex - 1].GetComponent<PirateShip>() != null)
        {
            currentShip = ships[shipIndex].GetComponent<PirateShip>();
            previousShip = ships[shipIndex - 1].GetComponent<PirateShip>();
        }
        else
        {
            currentShip = null;
            previousShip = null;

            Debug.LogError("Current or previous ship does not have PirateShip component");
        }

        currentShip.deselectShip();
        previousShip.selectShip();

        moveCamera(ships[shipIndex - 1]);
        shipIndex--;
    }

    public void moveCamera(GameObject ship)
    {
        // Compute direction vector from A to B
        Vector3 direction = ship.transform.position - planetTrans.position;
        float placementDistance = 30.0f;

        // Compute placement position at the given distance along the direction
        Vector3 placementPos = ship.transform.position + direction.normalized * placementDistance;

        Camera.main.transform.position = placementPos;
        Camera.main.transform.LookAt(ship.transform.position);
    }
}
