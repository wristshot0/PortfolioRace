using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Game data.
    public int numPlayers = 4;

    // Speed is the player's speed. All things move relative to player.
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
}
