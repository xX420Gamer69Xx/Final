using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public static int scoreValue = 0;
    Text score;

    public Text winText;



    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
        winText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Fixed Robots: " + scoreValue;

    }
}