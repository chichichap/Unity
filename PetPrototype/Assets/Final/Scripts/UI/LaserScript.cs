using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour
{
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.005f;
    public float laserMaxLength = 5f;
    public Collider hitCollider = null;

    void Start()
    {
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        if (transform.parent.GetComponent<PickupParent>().heldItem) {
            laserLineRenderer.enabled = false;
        }
        else {
            ShootLaserFromTargetPosition(transform.position, transform.forward, laserMaxLength);
            laserLineRenderer.enabled = true;
        }
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            if (raycastHit.collider.CompareTag("Item"))
                hitCollider = raycastHit.collider;
            else
                hitCollider = null;

            endPosition = raycastHit.point;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}