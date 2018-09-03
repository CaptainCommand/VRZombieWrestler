using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // Player position
    public Vector3 playerPosition;

    // List of possible zombie types.
    public List<GameObject> zombieTypes;

    // List of the spawn points for the zombies;
    public List<Vector3> spawnPoints;
    private System.Random spawnPointChooser;

    // Game level limits the types of zombies, as well as the max that can attack at the same time.
    private int gameLevel;

    // Maximum number of zombies that can exist at the same time.
    public int zombieMaxSpawnCount;

    // Current number of zombies to track
    private int zombieCurrentSpawnCount;

    // Timer delay for spawning
    public float spawnDelaySeconds;

    // Random number generator to choose the next zombie type
    private System.Random zombieChooser;

    // Use this for initialization
    void Start()
    {
        gameLevel = 0;
        zombieCurrentSpawnCount = 0;
        zombieChooser = new System.Random();
        spawnPointChooser = new System.Random();

        InvokeRepeating("SpawnZombie", 4.0f, spawnDelaySeconds);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnZombie()
    {
        if (zombieCurrentSpawnCount < zombieMaxSpawnCount)
        {
            ++zombieCurrentSpawnCount;

            // The next zombie type to spawn will be chosen randomly from the list,
            // where the list is limited by the game level;
            int zombieTypeMax = ((gameLevel + 1) < (zombieTypes.Count - 1)) ? (zombieTypes.Count - 1) : (gameLevel + 1);
            int zombieType = zombieChooser.Next(0, zombieTypeMax);

            // Choose spawn point randomly.
            int spawnPoint = spawnPointChooser.Next(0, spawnPoints.Count - 1);

            // Spawn the zombie and place it on the queue.
            Instantiate(zombieTypes[zombieType], spawnPoints[spawnPoint], Quaternion.LookRotation(playerPosition - spawnPoints[spawnPoint]));
        }
    }
}