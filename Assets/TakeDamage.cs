using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject deathMessagePrefab;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WebBall"))
        {
            currentHealth -= 10;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Instantiate(deathMessagePrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        Destroy(gameObject);
    }
}