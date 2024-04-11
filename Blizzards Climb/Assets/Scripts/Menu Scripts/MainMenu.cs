using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject main, options, controls;

    public GameObject mainMenuFirst, optionsMenuFirst, optionsClosedButton, controlsMenuFirst, controlsClosedButton;

    private void Start()
    {
        main.SetActive(true);
        options.SetActive(false);
        controls.SetActive(false);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // set a new selected object
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    public void OpenOptions()
    {
        main.SetActive(false);
        options.SetActive(true);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // set a new selected object
        EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
    }

    public void CloseOptions()
    {
        main.SetActive(true);
        options.SetActive(false);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // set a new selected object
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void OpenControls()
    {
        main.SetActive(false);
        controls.SetActive(true);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // set a new selected object
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }

    public void CloseControls()
    {
        main.SetActive(true);
        controls.SetActive(false);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // set a new selected object
        EventSystem.current.SetSelectedGameObject(controlsClosedButton);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
