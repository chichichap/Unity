using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {
    public static GameplayController Singleton;
    public Transform BallPrefab;

    private int playerScore = 0;
    private int aIScore = 0;
    private Text Scoreboard;
    private Transform playerPaddle;
    private Transform aIPaddle;
    private Transform ball;

    #region Setters & Getters
    public Transform PlayerPaddle
    {
        get
        {
            return playerPaddle;
        }

        set
        {
            playerPaddle = value;
        }
    }
    public Transform AIPaddle
    {
        get
        {
            return aIPaddle;
        }

        set
        {
            aIPaddle = value;
        }
    }
    public int PlayerScore
    {
        get
        {
            return playerScore;
        }

        set
        {
            playerScore = value;
        }
    }
    public int AIScore
    {
        get
        {
            return aIScore;
        }

        set
        {
            aIScore = value;
        }
    }
    #endregion

    void Awake() {
        Singleton = this;
    }

    void Start () {
        Scoreboard = transform.Find("Canvas/Scoreboard").GetComponent<Text>();
        StartCoroutine(delayAddBall(1));
    }

    public void PlayerScored() {
        playerScore++;
        AddBall();
    }

    public void AIScored() {
        aIScore++;
        AddBall();
    }

    private void AddBall() {
        Scoreboard.text = "Player: " + playerScore + " AI: " + AIScore;

        if (ball != null) {
            Destroy(ball.gameObject);
        }
            
        ball = Instantiate(BallPrefab,
                PlayerPaddle.transform.position + PlayerPaddle.forward * 0.5f,
                Quaternion.identity, PlayerPaddle.transform);
        
        PlayerPaddle.GetComponent<PlayerPaddleController>().Ball = ball;
        AIPaddle.GetComponent<AIPaddleController>().Ball = ball;
    }

    #region Coroutines
    IEnumerator delayAddBall(float delay) { 
        yield return new WaitForSecondsRealtime(delay);
        AddBall();
    }
    #endregion
}
