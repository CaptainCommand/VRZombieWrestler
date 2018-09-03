using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFiniteStateMachine
{
    // Keeps track of the zombie's current state.
    private ZombieState zombieState;

    // Delegate
    public ZombieBehaviorDelegate ActiveDelegate;
    public ZombieBehaviorDelegate IdleDelegate;
    public ZombieBehaviorDelegate AttackDelegate;
    public ZombieBehaviorDelegate RagdollDelegate;

    // Use this for initialization
    public ZombieFiniteStateMachine()
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
                IdleDelegate();
                break;
            case ZombieState.RAGDOLL:
                RagdollDelegate();
                break;
        }
    }

    public ZombieState GetState()
    {
        return zombieState;
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