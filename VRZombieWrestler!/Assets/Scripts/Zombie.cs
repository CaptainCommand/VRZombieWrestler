using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    // Zombie health attributes
    public float healthMax;
    private float healthCurrent;
    public float damage;

    // The player to track.
    public GameObject player;

    // Navigation
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Zombie attributes
    public float baseSpeed;
    public float acceleration;
    public float attackRange;

    // Current total of zombie grapple points being grappled.
    public int grapplePointsTotal;

    public ZombieFiniteStateMachine zombieFSM;

    // Use this for initialization
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;
        navMeshAgent.acceleration = acceleration;
        // Stop at a distance where it is still possible to attack.
        navMeshAgent.stoppingDistance = attackRange * 0.8f;

        zombieFSM = new ZombieFiniteStateMachine();
        zombieFSM.RagdollDelegate += DisableNavigation;
        zombieFSM.RagdollDelegate += EnableRagdoll;

        grapplePointsTotal = 0;
    }

    void Awake()
    {
        zombieFSM.Transition(ZombieState.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        if (grapplePointsTotal == 2)
        {
            zombieFSM.Transition(ZombieState.RAGDOLL);
        }
    }

    void FixedUpdate()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {

        }
    }

    public void EnableNavigation()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(player.transform.position);
        transform.LookAt(player.transform.position);
    }

    public void DisableNavigation()
    {
        navMeshAgent.enabled = false;
    }

    public void EnableRagdoll()
    {

    }
}
