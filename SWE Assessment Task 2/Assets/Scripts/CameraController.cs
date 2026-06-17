using UnityEngine;

public class CameraFollow2D : MonoBehaviour {
    public Transform target;
    [SerializeField] private float smoothSpeed;

    // Boundaries of the map
    [SerializeField] private float minX, maxX, minY, maxY;

    [SerializeField] private float camHalfHeight;
    [SerializeField] private float camHalfWidth;

    void Start() {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate() {
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Clamp camera inside bounds
        float clampedX = Mathf.Clamp(desiredPosition.x, minX + camHalfWidth, maxX - camHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY + camHalfHeight, maxY - camHalfHeight);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, transform.position.z), smoothSpeed);

        transform.position = smoothedPosition;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0),
            new Vector3(maxX - minX, maxY - minY, 0)
        );
    }
}
