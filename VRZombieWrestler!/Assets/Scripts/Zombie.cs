using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class keeps track of all of the required information for a zombie.
 * It requires the following scripts to be attached to the same GameObject:
 * Health.cs
 * ZombieFiniteStateMachine.cs
 * It requires the following components in the GameObject:
 * NavMeshAgent
 * Animator
 * Collider (for attack area)
 * Trigger Collider (for damage hitbox during ragdoll)
 * In addition, for the ragdoll, this GameObject must also utilize
 * the OVRGrabbable.cs script on any parts that can be grabbed.
 * Those parts must also be dragged into the list of grabbable objects.
 */
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
    public List<OVRGrabbable> ovrGrabbables;
    private int grabCount;
    private Animator animator;

    // Use this for initialization
    void Start()
    {

        // Initialize health
        health = GetComponent<Health>();
        isAttacking = false;

        // Get the animator component.
        animator = GetComponent<Animator>();


        // Set the nav mesh agent parameters.
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;
        navMeshAgent.acceleration = acceleration;
        // Stop at a distance where it is still possible to attack.
        navMeshAgent.stoppingDistance = attackRange * 0.5f;

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

        zombieFSM.Transition(ZombieState.IDLE);

        // Retrive all of the ovrGrabbable objects
        // from each of the grababble game objects in the list.
        ovrGrabbables = new List<OVRGrabbable>();
        foreach (GameObject gameObject in grabbableObjects)
        {
            ovrGrabbables.Add(gameObject.GetComponent<OVRGrabbable>());
        }
        grabCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // Loop over the set of grabbable objects
        // Check to see is any is being grappled by two points.
        // If so, then ragdoll.
        grabCount = 0;
        foreach (OVRGrabbable ovrGrabbable in ovrGrabbables)
        {
            if (ovrGrabbable.isGrabbed)
            {
                ++grabCount;
            }
        }
        if (grabCount >= 2)
        {
            zombieFSM.Transition(ZombieState.RAGDOLL);
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
        if (zombieFSM.currentState != ZombieState.RAGDOLL)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                zombieFSM.Transition(ZombieState.ATTACK);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (zombieFSM.currentState != ZombieState.RAGDOLL)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                zombieFSM.Transition(ZombieState.IDLE);
            }
        }
    }

    // Use a trigger collider for ragdoll collisions
    void OnTriggerEnter(Collider other)
    {
        if (zombieFSM.currentState == ZombieState.RAGDOLL)
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
        animator.enabled = false;
    }

    // Disables ragdoll mode.
    public void DisableRagdoll()
    {
        // TODO: animator snaps back too quickly, consider advanced transition and Mechanims
        animator.enabled = true;
    }

    public void Attack()
    {
        isAttacking = true;
        StartCoroutine(AttackCoroutine());
    }

    public void StopAttack()
    {
        StopCoroutine(AttackCoroutine());
        isAttacking = false;
    }
}
