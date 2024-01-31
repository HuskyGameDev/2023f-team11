using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerTarget;
    public Transform backgroundTarget;
    public Transform cameraObject;
    public GameObject background;

    void Start()
    {
        background?.transform.SetParent(cameraObject);
    }

    void FixedUpdate()
    {
        Vector3 playerTargetPos = new Vector3(transform.position.x, playerTarget.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, playerTargetPos, 0.2f);
    }
}
