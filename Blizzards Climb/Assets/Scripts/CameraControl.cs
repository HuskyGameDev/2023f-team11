using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    //public Transform background1;
    //public Transform background2;
    //private float size;

    // Start is called before the first frame update
    void Start()
    {
        //size = background1.GetComponent<BoxCollider2D>().size.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //camera
        Vector3 targetPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);

        //background
        //if (transform.position.y >= background2.position.y)
        //{
        //    background1.position = new Vector3(background1.position.x, background2.position.y + size, background1.position.z);
        //    SwitchBackground();
        //}
    }

    //private void SwitchBackground()
    //{
    //    Transform temp = background1;
    //    background1 = background2;
    //    background2 = temp;
    //}
}
