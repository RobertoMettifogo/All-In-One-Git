using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text DiamondCount;

    // No need for Start() method if you're assigning in the Inspector

    public void UpdateScoreText(float newScore)
    {
        DiamondCount.text = newScore.ToString();
        Debug.Log("Score UP");
    }
}
