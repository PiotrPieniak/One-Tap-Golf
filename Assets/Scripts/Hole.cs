using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    private const float ballMovementEdge = -3.4f;
    private const float basePosition = 4.061f;
    private const float leftModifier = -4.6f;
    private const float rightModifier = 3.65f;
    
    public GameObject Ball;
    public Text ScoreDisplay;
    public int score;
    private bool isNewXSet;

    void Start()
    {
        transform.position = new Vector2(basePosition + Random.Range(leftModifier, rightModifier), transform.position.y);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            score++;
        }
    }

    void Update()
    {
        ScoreDisplay.text = score.ToString();
        if (Ball.transform.position.y < ballMovementEdge && !isNewXSet)
        {
            isNewXSet = true;
            Invoke("LoadNewHoleLocation", 2);
        }
    }

    void LoadNewHoleLocation()
    {
        transform.position = new Vector2(basePosition + Random.Range(leftModifier, rightModifier), transform.position.y);
        Invoke("SetIsNewXSet", 0.3f);
    }

    void SetIsNewXSet()
    {
        isNewXSet = false;
    }
}
