using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other){

        if (other.gameObject.tag == "Player"){
            SceneManager.LoadScene("level 2", LoadSceneMode.Single);
        }
    }
}
