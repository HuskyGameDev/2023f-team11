﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour 
{
	public int health { get; private set; }
	public int maxHealth { get; private set; } = 5;

	public TextMeshPro fullHealth_Text;

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

	public bool HealPlayer(int healAmount)
	{

		// player health should not go above max health.
		if(health+healAmount > maxHealth)
		{
            StartCoroutine(DisplayFullMessage());
            return false;
        }
			
		else
			health += healAmount;
        Debug.Log("Health = " + health.ToString());

		return true;
    }

	private void KillPlayer()
	{
		// i mean you don't have to do anything fancy just reload the current scene?
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator DisplayFullMessage()
	{
		if (fullHealth_Text)
		{
			fullHealth_Text.gameObject.SetActive(true);

			yield return new WaitForSeconds(2);

			fullHealth_Text.gameObject.SetActive(false);
		}
		else Debug.LogWarning($"PlayerHealth.cs: fullHealth_text reference not set.");
		
	}
}
