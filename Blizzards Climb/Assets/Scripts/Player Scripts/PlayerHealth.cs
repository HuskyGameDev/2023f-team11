﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
		Debug.Log("Health = " + health.ToString());
	}
}
