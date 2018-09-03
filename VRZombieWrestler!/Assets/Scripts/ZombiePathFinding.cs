using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePathFinding : MonoBehaviour
{
    // The player to track.
    public GameObject player;

    // Navigation
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Zombie attributes
    public float baseSpeed;
    public float acceleration;
    public float stoppingDistance;

    // Zombie settings, only tries to move if the zombie is active
    public bool isActive;

    // Use this for initialization
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;
        navMeshAgent.acceleration = acceleration;
        navMeshAgent.stoppingDistance = stoppingDistance;
    }

    void Awake()
    {
        // Zombies start out not moving at first.
        navMeshAgent.enabled = false;
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (isActive)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(player.transform.position);
            transform.LookAt(player.transform.position);
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }
}
