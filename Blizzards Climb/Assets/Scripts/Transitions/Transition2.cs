using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition2 : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public void OnTriggerEnter2D(Collider2D other){

        if (other.gameObject.tag == "Player"){

            //SceneManager.LoadScene("level 3", LoadSceneMode.Single);

            LoadNextLevel();
        }
    }

    public void LoadNextLevel(){
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex){
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
