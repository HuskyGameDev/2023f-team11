using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    [SerializeField] public GameObject enemy1Prefab;
    [SerializeField] public bool isDead = false;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float timer = 0f;
    [SerializeField] private GameObject startPoint, endPoint;
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            timer += Time.deltaTime;
        }
        if(timer > cooldown)
        {
            Vector2 spawnPoint = new Vector2(transform.position.x, transform.position.y);
            GameObject newEnemy = Instantiate(enemy1Prefab,spawnPoint, Quaternion.identity);
            
            newEnemy.GetComponent<SmartBirdAI>().endPoint = endPoint;
            newEnemy.GetComponent<SmartBirdAI>().startPoint = startPoint;
            isDead = false;
            timer = 0f;
        }
    }
}
