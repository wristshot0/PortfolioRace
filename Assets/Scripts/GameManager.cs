using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using CreateNeptune;

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

    // Countdown at start.
    [SerializeField] private GameObject countDown;
    private float countDownTime = 3f;

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
        float relativeTimeToDisplayNums = 0.5f;

        for (int i = 3; i > 0; i--)
        {
            countDown.GetComponent<TextMeshPro>().text = i.ToString();

            yield return MPAction.ScaleObject(countDown, Vector2.zero, Vector2.one,
                countDownTime / 3f * (1f - relativeTimeToDisplayNums) * 0.5f, "easeineaseout", false, false, false, false);

            yield return new WaitForSeconds(countDownTime / 3f * relativeTimeToDisplayNums);

            yield return MPAction.ScaleObject(countDown, Vector2.one, Vector2.zero,
                countDownTime / 3f * (1f - relativeTimeToDisplayNums) * 0.5f, "easeineaseout", false, false, false, false);
        }

        countDown.SetActive(false);

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
