using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class keeps track of health. This can be attached
 * to both the players and enemies.
 */
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

    public float health
    {
        get
        {
            return healthCurrent;
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
        if ((healthCurrent - damage) < 0.0f)
            healthCurrent = 0.0f;
        else healthCurrent -= damage;
    }
}
