using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class keeps track of all of the required information for a zombie.
 * It requires the following scripts to be attached to the same GameObject:
 * Health.cs
 * ZombieFiniteStateMachine.cs
 * OVRGrabbable.cs
 * It requires the following components in the GameObject:
 * NavMeshAgent
 * Animator
 * Collider (for attack area)
 * Trigger Collider (for damage hitbox during ragdoll)
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
    private OVRGrabbable ovrGrabbable;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        // Get the animator component.
        animator = GetComponent<Animator>();

        // Initialize health
        health = GetComponent<Health>();
        isAttacking = false;

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
        zombieFSM.IdleDelegate += EnableIdle;
        // Actions for ragdoll state.
        zombieFSM.RagdollDelegate += StopAttack;
        zombieFSM.RagdollDelegate += DisableNavigation;
        zombieFSM.RagdollDelegate += DisableIdle;
        zombieFSM.RagdollDelegate += EnableRagdoll;
        // Actions for active state.
        zombieFSM.ActiveDelegate += DisableRagdoll;
        zombieFSM.ActiveDelegate += StopAttack;
        zombieFSM.ActiveDelegate += DisableIdle;
        zombieFSM.ActiveDelegate += EnableNavigation;
        // Actions for attacking state.
        zombieFSM.AttackDelegate += DisableRagdoll;
        zombieFSM.AttackDelegate += DisableNavigation;
        zombieFSM.AttackDelegate += DisableIdle;
        zombieFSM.AttackDelegate += Attack;
        // Default state is idle.
        zombieFSM.Transition(ZombieState.IDLE);

        // Retrieve the grabbable objects.
        ovrGrabbable = GetComponent<OVRGrabbable>();

        // Set all parts of the Zombie to be kinematic to start.
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check to see if the object has been grabbed,
        if (ovrGrabbable.isGrabbed)
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

    public void EnableIdle()
    {
        animator.SetBool("isIdle", true);
    }

    public void DisableIdle()
    {
        animator.SetBool("isIdle", false);
    }

    // Enables navigation.
    public void EnableNavigation()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(player.transform.position);
        transform.LookAt(player.transform.position);
        animator.SetBool("isActive", true);
    }

    // Disables navigation.
    public void DisableNavigation()
    {
        navMeshAgent.enabled = false;
        animator.SetBool("isActive", false);
    }

    // Enables ragdoll mode.
    public void EnableRagdoll()
    {
        // Zombie must not be kinematic for ragdoll mode.
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
        animator.enabled = false;
    }

    // Disables ragdoll mode.
    public void DisableRagdoll()
    {
        // Set all parts of the Zombie to be kinematic for animator.
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void Attack()
    {
        isAttacking = true;
        StartCoroutine(AttackCoroutine());
        animator.SetBool("isAttacking", true);
    }

    public void StopAttack()
    {
        StopCoroutine(AttackCoroutine());
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
}
