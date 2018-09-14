using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class will spawn zombies randomly among the spawn points
 * specified. It also randomizes zombie types among a given set of types.
 * It only allows a maximum number of zombie spawns, and only a set
 * amount of those zombies are active at any given time.
 */
public class ZombieSpawner : MonoBehaviour
{
    // Player position
    public GameObject player;

    // List of possible zombie types.
    public List<GameObject> zombieTypes;

    // List of zombies currently spawned.
    private List<GameObject> zombies;

    // List of the spawn points for the zombies;
    public List<GameObject> spawnPoints;
    private System.Random spawnPointChooser;

    // Game level limits the types of zombies, as well as the max that can attack at the same time.
    private int gameLevel;

    // This determines the number of zombie kills that will increase the level.
    public int zombieScorePerLevel;

    // This is the total number of zombies killed so far.
    public int totalScore;

    // This is the current number of zombies killed in a difficulty level.
    private int zombieScore;

    // Maximum number of zombies that can exist at the same time.
    public int zombieMaxSpawnCount;

    // Maximum number of zombies active at the same time.
    public int zombieMaxActiveCount;

    // Timer delay for spawning
    public float spawnDelaySeconds;

    // Random number generator to choose the next zombie type
    private System.Random zombieChooser;

    // Zombie blood particle effect
    public ParticleSystem bloodExplosion;

    // Use this for initialization
    void Start()
    {
        zombies = new List<GameObject>();

        gameLevel = 0;
        zombieScore = 0;
        zombieChooser = new System.Random();
        spawnPointChooser = new System.Random();

        // GameStart(); Debug only.
    }

    public void GameStart()
    {
        InvokeRepeating("SpawnZombie", 1.0f, spawnDelaySeconds);
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    // Increases the max number of zombies spawned as well as the level.
    void LevelUp()
    {
        if (zombieScore >= zombieScorePerLevel)
        {
            // Reset current zombie score for the level.
            zombieScore = 0;
            gameLevel++;
            zombieMaxSpawnCount++;
            zombieMaxActiveCount++;
        }
    }

    void DestroyZombies()
    {
        // Remove any dead zombies from the scene
        for (int index = zombies.Count - 1; index >= 0; index--)
        {
            if (zombies[index].GetComponent<Health>().isDead)
            {
                // Create a blood splatter effect.
                Instantiate<ParticleSystem>(bloodExplosion,
                    zombies[index].transform.position, zombies[index].transform.rotation);
                // Destroy the zombie and remove from the list.
                Destroy(zombies[index]);
                zombies.RemoveAt(index);
                
                totalScore++;
                zombieScore++;
                // Check to increase difficulty level.
                LevelUp();
            }
        }
    }

    void SpawnZombie()
    {
        // Destroy zombies
        DestroyZombies();

        // Spawn a zombie if we are under the max number.
        if (zombies.Count < zombieMaxSpawnCount)
        {
            // The next zombie type to spawn will be chosen randomly from the list.
            int zombieType = zombieChooser.Next(0, zombieTypes.Count - 1);

            // Choose spawn point randomly.
            int spawnPoint = spawnPointChooser.Next(0, spawnPoints.Count - 1);

            // Spawn the zombie and add to the list.
            zombies.Add(Instantiate(zombieTypes[zombieType], spawnPoints[spawnPoint].transform.position,
                Quaternion.LookRotation(player.transform.position - spawnPoints[spawnPoint].transform.position)));
            zombies[zombies.Count - 1].GetComponent<Zombie>().player = player;
        }

        // Set the first zombies in the list as active
        for (int index = 0; index < zombies.Count; index++)
        {
            if (index < zombieMaxActiveCount)
            {
                // If the zombie is currently idle, set it to active.
                if (zombies[index].GetComponent<Zombie>().zombieFSM.currentState == ZombieState.IDLE)
                {
                    zombies[index].GetComponent<Zombie>().zombieFSM.Transition(ZombieState.ACTIVE);
                }
            }
        }
    }
}