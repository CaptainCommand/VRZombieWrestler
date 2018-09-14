using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public GameObject player;
    public Image healthImage;
    public Text scoreText;
    public GameObject startMenu;
    public GameObject gameOver;

	// Use this for initialization
	void Start () {
        startMenu.SetActive(true);
        gameOver.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        // Update health bar according to player health.
        healthImage.fillAmount = player.GetComponent<Health>().health / player.GetComponent<Health>().healthMax;

        // Update score according to current score.
        scoreText.text = "Zombies Slammed: " + player.GetComponent<Player>().score.ToString();

        if (player.GetComponent<Health>().isDead)
        {
            gameOver.SetActive(true);
        }

    }
}
