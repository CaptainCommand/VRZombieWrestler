using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int currentScore;

	void Start ()
    {
        currentScore = 0;
	}

    public int score
    {
        get
        {
            return currentScore;
        }
    }

    void addScore(int add)
    {
        currentScore += add;
    }
	
	void Update ()
    {
		
	}
}
