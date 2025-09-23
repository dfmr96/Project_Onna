using UnityEngine;

public class MouseGroundAiming : MonoBehaviour
{
    [Header("Targeting")]
    public Transform aimTarget;
    public LayerMask groundLayerMask = 1;
    public Camera playerCamera;

    [Header("Ground Plane")]
    public Transform playerTransform;
    public float planeOffset = 0f;

    [Header("Debug")]
    public bool showDebugRay = true;

    private Plane groundPlane;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerTransform == null)
            playerTransform = transform;
    }

    private void Update()
    {
        UpdateGroundPlane();
        HandleMouseAiming();
    }

    private void UpdateGroundPlane()
    {
        Vector3 planePosition = new Vector3(playerTransform.position.x, playerTransform.position.y + planeOffset, playerTransform.position.z);
        groundPlane = new Plane(Vector3.up, planePosition);
    }

    private void HandleMouseAiming()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        }

        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            if (aimTarget != null)
            {
                aimTarget.position = hitPoint;
            }

            if (showDebugRay)
            {
                Debug.DrawLine(ray.origin, hitPoint, Color.green);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Vector3 planeCenter = new Vector3(playerTransform.position.x, playerTransform.position.y + planeOffset, playerTransform.position.z);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(planeCenter, new Vector3(10f, 0.1f, 10f));
        }

        if (aimTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(aimTarget.position, 0.5f);
        }
    }
}