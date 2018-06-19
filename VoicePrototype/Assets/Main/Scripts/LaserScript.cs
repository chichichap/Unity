using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour
{
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.005f;
    public float laserMaxLength = 5f;

    void Start()
    {
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        ShootLaserFromTargetPosition(transform.position, transform.forward, laserMaxLength);
        laserLineRenderer.enabled = true;
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            if (raycastHit.collider.CompareTag("Interactable"))
            {
                PlayerController.main.PointedObject = raycastHit.collider.GetComponent<VRInteractable>();
            }
            else if (raycastHit.collider.CompareTag("InteractableChild"))
            {
                PlayerController.main.PointedObject = raycastHit.collider.transform.parent.GetComponent<VRInteractable>();
            }
            else
            {
                PlayerController.main.PointedObject = null;
            }

            endPosition = raycastHit.point;
        }
        else
        {
            PlayerController.main.PointedObject = null;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}