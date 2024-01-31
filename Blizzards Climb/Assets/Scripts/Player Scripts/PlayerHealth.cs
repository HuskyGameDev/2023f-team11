using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour 
{
	public int currentHealth;
	public int maxHealth = 10;

	void Start()
	{
		currentHealth = maxHealth;
	}
    public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		Debug.Log("Health = " + currentHealth.ToString());
        if (currentHealth == 0)
        {
            gameObject.SetActive(false);
            
            Destroy(gameObject, .78f);
            // Currently set to TestLevel so you are able to replay.
            // In theory we will either set it to load the current scene or the main menu depending on what
            // We want to happen when blizzard dies.
            SceneManager.LoadScene("TestLevel");
        }
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
