using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuePlaying : MonoBehaviour
{
    private CustomInput PlayerInput;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        PlayerInput = new CustomInput();
        PlayerInput.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        getInput();
    }
    private void getInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
        }
    }
}
