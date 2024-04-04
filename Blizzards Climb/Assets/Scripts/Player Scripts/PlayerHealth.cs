using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour 
{
	public int health;
	public int maxHealth = 5;

	void Start()
	{
		health = maxHealth;
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0) KillPlayer();
		Debug.Log("Health = " + health.ToString());
	}

	private void KillPlayer()
	{
		// i mean you don't have to do anything fancy just reload the current scene?
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
