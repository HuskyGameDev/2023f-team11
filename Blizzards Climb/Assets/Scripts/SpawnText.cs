using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnText : MonoBehaviour
{
    [SerializeField] private GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
        this.gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        text.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        text.SetActive(false);
    }
}
