using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    // Zombie health attributes
    private Health health;

    // The player to track.
    public GameObject player;

    // Navigation
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Zombie attributes
    public float baseSpeed;
    public float acceleration;
    public float attackRange;
    private bool isAttacking;
    public float damage;
    public float attackCooldown;

    public ZombieFiniteStateMachine zombieFSM;
    public List<GameObject> grabbableObjects;
    private List<OVRGrabbable> ovrGrabbables;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        // Initialize health
        health = GetComponent<Health>();
        isAttacking = false;

        // Set the nav mesh agent parameters.
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;
        navMeshAgent.acceleration = acceleration;
        // Stop at a distance where it is still possible to attack.
        navMeshAgent.stoppingDistance = attackRange * 0.8f;

        // Set up the finite state machine
        zombieFSM = GetComponent<ZombieFiniteStateMachine>();
        // Actions for idle state.
        zombieFSM.IdleDelegate += DisableRagdoll;
        zombieFSM.IdleDelegate += StopAttack;
        zombieFSM.IdleDelegate += DisableNavigation;
        // Actions for ragdoll state.
        zombieFSM.RagdollDelegate += StopAttack;
        zombieFSM.RagdollDelegate += DisableNavigation;
        zombieFSM.RagdollDelegate += EnableRagdoll;
        // Actions for active state.
        zombieFSM.ActiveDelegate += DisableRagdoll;
        zombieFSM.ActiveDelegate += StopAttack;
        zombieFSM.ActiveDelegate += EnableNavigation;
        // Actions for attacking state.
        zombieFSM.AttackDelegate += DisableRagdoll;
        zombieFSM.AttackDelegate += DisableNavigation;
        zombieFSM.AttackDelegate += Attack;

        // Retrive all of the ovrGrabbable objects
        // from each of the grababble game objects in the list.
        foreach (GameObject gameObject in grabbableObjects)
        {
            ovrGrabbables.Add(gameObject.GetComponent<OVRGrabbable>());
        }

        // Get the animator component.
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        zombieFSM.Transition(ZombieState.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        // Loop over the set of grabbable objects
        // Check to see is any is being grappled.
        // If so, then ragdoll.
        foreach (OVRGrabbable ovrGrabbable in ovrGrabbables)
        {
            if (ovrGrabbable.isGrabbed)
            {
                zombieFSM.Transition(ZombieState.RAGDOLL);
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        // If the zombie is currently attacking.
        while (isAttacking)
        {
            // Attack the player in range.
            player.GetComponent<Health>().TakeDamage(damage);

            yield return new WaitForSeconds(attackCooldown);
        }

        yield return null;
    }

    // Use a normal collider for the attack hurtbox
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            zombieFSM.Transition(ZombieState.ATTACK);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            zombieFSM.Transition(ZombieState.IDLE);
        }
    }

    // Use a trigger collider for ragdoll collisions
    void OnTriggerEnter(Collider other)
    {
        // Takes damage from the environment.
        if (other.CompareTag("Environment"))
        {
            health.TakeDamage(damage);
            // Check for dead state.
            if (health.isDead)
            {
                zombieFSM.Transition(ZombieState.RAGDOLL);
            }
            else
            {
                zombieFSM.Transition(ZombieState.IDLE);
            }
        }
        // Damages both the zombie and the zombie it hits.
        else if (other.CompareTag("Zombie"))
        {
            health.TakeDamage(damage);
            other.GetComponent<Zombie>().health.TakeDamage(damage);
            // Check for dead state.
            if (health.isDead)
            {
                zombieFSM.Transition(ZombieState.RAGDOLL);
            }
            else
            {
                zombieFSM.Transition(ZombieState.IDLE);
            }
        }
    }

    // Enables navigation.
    public void EnableNavigation()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(player.transform.position);
        transform.LookAt(player.transform.position);
    }

    // Disables navigation.
    public void DisableNavigation()
    {
        navMeshAgent.enabled = false;
    }

    // Enables ragdoll mode.
    public void EnableRagdoll()
    {

    }

    // Disables ragdoll mode.
    public void DisableRagdoll()
    {

    }

    public void Attack()
    {
        isAttacking = true;
        StartCoroutine(AttackCoroutine());
    }

    public void StopAttack()
    {
        isAttacking = false;
        StopCoroutine(AttackCoroutine());
    }
}
