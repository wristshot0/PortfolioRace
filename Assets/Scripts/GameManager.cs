using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int numPlayers = 4;
    public int numFinishers = 0;
    public GameState gameState;
    [SerializeField] private int numPowerUpsPerLane;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private Transform[] playerTs;
    [SerializeField] private Transform startingLineT;
    [SerializeField] private Transform finishLineT;

    // GPS
    public bool gpsPopped = false;
    public Canvas gpsCanvas;

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
        CreatePowerUps();

        StartCoroutine(BeginRace());
    }

    private void CreatePowerUps()
    {
        // Instantiate power-ups between start and finish line at random. Create only 3 power-ups per lane for player.
        for (int i = 0; i < playerTs.Length; i++)
        {
            for (int j = 0; j < numPowerUpsPerLane; j++)
            {
                float xLoc = playerTs[i].position.x;
                float yLoc = Random.Range(0.25f * (finishLineT.position.y - startingLineT.position.y)
                    + startingLineT.position.y, 0.9f * (finishLineT.position.y - startingLineT.position.y)
                    + startingLineT.position.y);

                Instantiate(powerUps[Random.Range(0, powerUps.Length)], new Vector2(xLoc, yLoc), Quaternion.identity);
            }
        }
    }

    private IEnumerator BeginRace()
    {
        yield return new WaitForSeconds(3f);

        gameState = GameState.gameplay;
    }

    private void Update()
    {
        if (numFinishers >= numPlayers)
        {
            gameState = GameState.endgame;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }
}
