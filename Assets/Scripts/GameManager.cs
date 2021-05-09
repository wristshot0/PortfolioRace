using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Game data.
    public int numPlayers = 4;

    public int numFinishers = 0;

    public GameState gameState;

    public enum GameState
    {
        pregame, gameplay, endgame
    }

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        StartCoroutine(BeginRace());
    }

    private IEnumerator BeginRace()
    {
        yield return new WaitForSeconds(3f);

        gameState = GameState.gameplay;
    }

    private void Update()
    {
        if (numFinishers >= numPlayers)
            gameState = GameState.endgame;
    }
}
