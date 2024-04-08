using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerTarget;
    public Transform background1;
    public Transform background2;
    public float size;

    void Start()
    {
        size = background1.GetComponent<BoxCollider2D>().size.y * background1.GetComponent<Transform>().localScale.y;
    }

    void FixedUpdate()
    {
        //camera
        Vector3 playerTargetPos = new Vector3(transform.position.x, playerTarget.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, playerTargetPos, 0.2f);

        //background
        if (transform.position.y >= background2.position.y)
        {
            background1.position = new Vector3(background1.position.x, background2.position.y + size, background1.position.z);
            SwitchBackground();
        }

        if (transform.position.y < background1.position.y)
        {
            background2.position = new Vector3(background2.position.x, background1.position.y - size, background2.position.z);
            SwitchBackground();
        }
    }

    private void SwitchBackground()
    {
        Transform temp = background1;
        background1 = background2;
        background2 = temp;
    }
}
