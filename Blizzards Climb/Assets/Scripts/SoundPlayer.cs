using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour

{
    public AudioSource src;
    public AudioClip chirp1, chirp2, damage, jump, pickup, thud, background;

    public void Chirp1()
    {
        src.clip = chirp1;
        src.Play();
    }

    public void Chirp2()
    {
        src.clip = chirp2;
        src.Play();
    }

    public void Damage()
    {
        src.clip = damage;
        src.Play();
    }

    public void Jump()
    {
        src.clip = jump;
        src.Play();
    }

    public void Pickup()
    {
        src.clip = pickup;
        src.Play();
    }

    public void Thud()
    {
        src.clip = thud;
        src.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        src.clip = background;
        src.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
