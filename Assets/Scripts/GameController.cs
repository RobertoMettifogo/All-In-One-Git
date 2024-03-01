using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController playerController;
    public UIController uiController;

    public float score = 0;

    public void IncreaseScore()
    {
        score++;
        Debug.Log("score ++");
        uiController.UpdateScoreText(score);
    }
}
