using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public GameObject pauseMenuFirst;

    private CustomInput inputActions;
    private PlayerHealth ph;
    private bool playerDead => ph.health <= 0;

    private void Awake()
    {
        inputActions = new CustomInput();
        ph = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (!playerDead)
        {
            if (inputActions.UI.Pause.WasPressedThisFrame() && !pauseMenu.activeInHierarchy)
                Pause();
            else if (inputActions.UI.Pause.WasPressedThisFrame() && pauseMenu.activeInHierarchy)
                Resume();
        }
        
    }

    public void Pause(){
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
        Time.timeScale = 0f;

    }
    public void Resume(){
        pauseMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1f;

    }
    public void Home(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Restart(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
