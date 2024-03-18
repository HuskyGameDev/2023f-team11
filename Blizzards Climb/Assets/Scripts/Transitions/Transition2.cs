using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition2 : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other){

        if (other.gameObject.tag == "Player"){
            SceneManager.LoadScene("level 3", LoadSceneMode.Single);
        }
    }
}
