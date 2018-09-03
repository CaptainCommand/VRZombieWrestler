using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class can go on both players and enemies.
public class Health : MonoBehaviour
{
    public float healthMax;
    private float healthCurrent;

    public bool isDead
    {
        get
        {
            return healthCurrent <= 0.0f;
        }
    }

    // Use this for initialization
    void Start()
    {
        healthCurrent = healthMax;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        healthCurrent -= damage;
    }
}
