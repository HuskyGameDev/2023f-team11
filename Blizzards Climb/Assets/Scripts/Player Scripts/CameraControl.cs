using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerTarget;
    public Transform backgroundTarget;
    public Transform cameraObject;
    public GameObject background;
    public float leftBound = 0.3f; //Left boundary of the camera as a percent of screen width
    public float rightBound = 0.7f; //Right boundary of the camera as a percent of screen width

    void Start()
    {
        background?.transform.SetParent(cameraObject);
    }

    void FixedUpdate()
    {
        Vector3 playerTargetPos = new Vector3(playerTarget.position.x, playerTarget.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, playerTargetPos, 0.2f);
    }
}
