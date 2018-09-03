using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // Player position
    public Vector3 playerPosition;

    // List of possible zombie types.
    public List<GameObject> zombieTypes;

    // List of zombies currently spawned.
    private List<GameObject> zombies;

    // List of the spawn points for the zombies;
    public List<Vector3> spawnPoints;
    private System.Random spawnPointChooser;

    // Game level limits the types of zombies, as well as the max that can attack at the same time.
    private int gameLevel;

    // Maximum number of zombies that can exist at the same time.
    public int zombieMaxSpawnCount;
    public int zombieMaxAttackCount;

    // Timer delay for spawning
    public float spawnDelaySeconds;

    // Random number generator to choose the next zombie type
    private System.Random zombieChooser;

    // Use this for initialization
    void Start()
    {
        zombies = new List<GameObject>();

        gameLevel = 0;
        zombieChooser = new System.Random();
        spawnPointChooser = new System.Random();

        InvokeRepeating("SpawnZombie", spawnDelaySeconds, spawnDelaySeconds);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnZombie()
    {
        // Remove any dead zombies from the scene
        zombies.RemoveAll(zombie => zombie.GetComponent<Health>().isDead == true);
        for (int index = zombies.Count - 1; index >= 0; index--)
        {
            if (zombies[index].GetComponent<Health>().isDead)
            {
                GameObject zombieDying = zombies[index];
                zombies.RemoveAt(index);
                Destroy(zombieDying);
            }
        }

        // Spawn a zombie is we are under the max number.
        if (zombies.Count < zombieMaxSpawnCount)
        {
            // The next zombie type to spawn will be chosen randomly from the list,
            // where the list is limited by the game level;
            int zombieTypeMax = ((gameLevel + 1) < (zombieTypes.Count - 1)) ? (zombieTypes.Count - 1) : (gameLevel + 1);
            int zombieType = zombieChooser.Next(0, zombieTypeMax);

            // Choose spawn point randomly.
            int spawnPoint = spawnPointChooser.Next(0, spawnPoints.Count - 1);

            // Spawn the zombie and add to the list.
            zombies.Add(Instantiate(zombieTypes[zombieType], spawnPoints[spawnPoint], Quaternion.LookRotation(playerPosition - spawnPoints[spawnPoint])));
        }

        // Set the first four zombies in the list as active
        for (int index = 0; index < zombies.Count; index++)
        {
            if (index < zombieMaxAttackCount)
            {
                // If the zombie is currently idle, set it to active.
                if (zombies[index].GetComponent<Zombie>().zombieFSM.GetState() == ZombieState.IDLE)
                {
                    zombies[index].GetComponent<Zombie>().zombieFSM.Transition(ZombieState.ACTIVE);
                }
            }
        }
    }
}