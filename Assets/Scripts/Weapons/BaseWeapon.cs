using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour {
    public GameObject ammoPrefab;

    public virtual void Fire() {
        Debug.LogWarning("BaseWeapon.Fire() called");
    }

    protected void OnUpdate() {
        if (!Input.GetMouseButtonDown(0)) {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        if (hit.collider.gameObject != gameObject) {
            return;
        }

        Fire();
    }
}