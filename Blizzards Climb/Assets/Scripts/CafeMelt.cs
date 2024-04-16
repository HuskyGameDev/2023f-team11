using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeMelt : MonoBehaviour
{
    public int healthToRestore = 1;
    public AudioSource audio;
    public AudioClip clip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out var player))
        {
            if (player.HealPlayer(healthToRestore))
            {
                audio.clip = clip;
                audio.Play();
                Destroy(this.gameObject);
            }
                
        }
    }
}
