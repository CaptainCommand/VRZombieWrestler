using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class allows for the zombie's behavior to change
 * according to the current state of the zombie.
 * A script that implements this FSM must create and add
 * valid delegates to each state's delegate, so that
 * the proper actions are enabled.
 */
public class ZombieFiniteStateMachine : MonoBehaviour
{
    // Keeps track of the zombie's current state.
    private ZombieState zombieState;

    // Delegate
    public ZombieBehaviorDelegate ActiveDelegate;
    public ZombieBehaviorDelegate IdleDelegate;
    public ZombieBehaviorDelegate AttackDelegate;
    public ZombieBehaviorDelegate RagdollDelegate;

    // Use this for initialization
    void Start()
    {
        zombieState = ZombieState.IDLE;
    }

    public void Transition(ZombieState newState)
    {
        zombieState = newState;

        switch (zombieState)
        {
            case ZombieState.ACTIVE:
                ActiveDelegate();
                break;
            case ZombieState.IDLE:
                IdleDelegate();
                break;
            case ZombieState.ATTACK:
                AttackDelegate();
                break;
            case ZombieState.RAGDOLL:
                RagdollDelegate();
                break;
        }
    }

    public ZombieState currentState
    {
        get
        {
            return zombieState;
        }
    }
}

// Keeps track of the actions to call for each zombie state
public delegate void ZombieBehaviorDelegate();

// This enum will keep track of the current state of the zombie.
public enum ZombieState
{
    IDLE = 0, // Not actively moving or tracking; idle.
    ACTIVE = 1, // Actively moving, tracking, attacking.
    ATTACK = 2, // Not moving, attacking
    RAGDOLL = 3, // Ragdoll mode. Zombie should be able to damage other zombies and itself on collision.
}