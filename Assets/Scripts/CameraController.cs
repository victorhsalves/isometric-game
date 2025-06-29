using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;

    private void LateUpdate()
    {
        Zoom();
        transform.position =  target.position + offset;
    }

    private void Zoom()
    {
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput != 0)
        {
            Camera.main.orthographicSize -= zoomInput * zoomSpeed;
            if (Camera.main.orthographicSize < minZoom)
            {
                Camera.main.orthographicSize = minZoom;
            }
            else if (Camera.main.orthographicSize > maxZoom)
            {
                Camera.main.orthographicSize = maxZoom;
            }
        }
    }
}