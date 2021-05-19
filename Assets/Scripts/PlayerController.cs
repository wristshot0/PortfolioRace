using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreateNeptune;
using TMPro;
using UnityEngine.UI;

public class PlayerController : CarController
{
    // Controls
    [SerializeField] private bool phoneTesting;
    private bool playerTouching;

    // Tachometer needle
    [SerializeField] private RectTransform tachNeedleRT;

    // Gas pedal.
    [SerializeField] private RectTransform gasPedal;

    // Risk assessment.
    [SerializeField] private Image riskBannerImage;
    [SerializeField] private Text riskText;
    [SerializeField] private Image crashBannerImage;
    [SerializeField] private Text crashProbability;

    // Power-up
    [SerializeField] private GameObject powerUpText;

    // Engine blown text.
    [SerializeField] private GameObject engineBlownText;

    // Place.
    [SerializeField] private Text placeText;
    [SerializeField] private Transform[] playerTs;
    [SerializeField] private Transform[] placeMarkers;
    [SerializeField] private TextMeshPro[] playerDistances;

    private void Update()
    {
        if (gm.gameState == GameManager.GameState.gameplay && !finished && !engineBlown)
        {
            GetInput();
        }
        else
            playerTouching = false;

        AssessRisk();
        MoveTachometer();
        PushGasPedal();
        MakeEngineNoise();
        GetDistances();
    }

    private void GetDistances()
    {
        for (int i = 1; i < playerTs.Length; i++)
        {
            float distancefromP1 = playerTs[i].position.y - playerTs[0].position.y;

            // Direct place marker.
            if (distancefromP1 > 0)
                placeMarkers[i - 1].localScale = new Vector2(0.1f, -0.1f);
            else
                placeMarkers[i - 1].localScale = new Vector2(0.1f, 0.1f);

            playerDistances[i - 1].text = distancefromP1.ToString("0") + " m";
        }
    }

    private void AssessRisk()
    {
        float relativeRisk = currentSpeed / topSpeed;

        Color newColor = Color.HSVToRGB((1f - relativeRisk) * 0.333f, 1f, 1f);
        newColor.a = 0.5f;
        riskBannerImage.color = newColor;
        crashBannerImage.color = newColor;

        if (relativeRisk < 0.33f)
            riskText.text = "Risk: Low";
        else if (relativeRisk < 0.67f)
            riskText.text = "Risk: Medium";
        else if (relativeRisk < 0.9f)
            riskText.text = "Risk: High";
        else
            riskText.text = "Risk: High+";

        crashProbability.text = "CRASH " + (PBlowUp() * 100f).ToString("0") + "%";
    }

    private void MoveTachometer()
    {
        Vector3 startRotation = new Vector3(0f, 0f, 90f);
        Vector3 endRotation = new Vector3(0f, 0f, -90f);

        tachNeedleRT.rotation = Quaternion.Euler(startRotation + (endRotation - startRotation) * currentSpeed / topSpeed);
    }

    private void PushGasPedal()
    {
        if (playerTouching)
            gasPedal.localScale = Vector2.one * 0.75f;
        else
            gasPedal.localScale = Vector2.one;
    }

    protected override void FixedUpdate()
    {
        if (playerTouching)
        {
            Drive();
        }
        else
        {
            Brake();
        }

        rb2d.MovePosition(rb2d.position + currentSpeed * (Vector2)t.up * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("powerup"))
        {
            StartCoroutine(AnnouncePowerUp(collision.GetComponent<PowerUp>()));

            if (currentPowerUpRoutine != null)
                StopCoroutine(currentPowerUpRoutine);

            currentPowerUpRoutine = PowerUp(collision.GetComponent<PowerUp>());
            StartCoroutine(currentPowerUpRoutine);

            StartCoroutine(DestroyPowerUp(collision.gameObject));
        }
        else if (collision.CompareTag("finishline"))
            FinishRace();
    }

    private IEnumerator AnnouncePowerUp(PowerUp powerUp)
    {
        powerUpText.GetComponent<TextMeshPro>().text = powerUp.powerUpMessage;

        yield return MPAction.ScaleObject(powerUpText, Vector2.zero, Vector2.one, 0.5f, "easeineaseout", false, false, false, false);

        yield return new WaitForSeconds(2f);

        StartCoroutine(MPAction.ScaleObject(powerUpText, Vector2.one, Vector2.zero, 0.5f, "easeineaseout", false, false, false, false));
    }

    private void GetInput()
    {
#if UNITY_EDITOR
        if (phoneTesting)
            GetTouchInput();
        else
            playerTouching = Input.anyKey;
#else
        GetTouchInput();
#endif
    }

    private void GetTouchInput()
    {
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    playerTouching = true;
                    break;
                case TouchPhase.Moved:
                    playerTouching = true;
                    break;
                case TouchPhase.Stationary:
                    playerTouching = true;
                    break;
                case TouchPhase.Canceled:
                    playerTouching = false;
                    break;
                case TouchPhase.Ended:
                    playerTouching = false;
                    break;
            }
        }
    }

    protected override IEnumerator EngineBlownRoutine()
    {
        engineSmoke.Play();

        yield return MPAction.ScaleObject(engineBlownText, Vector2.zero, Vector2.one, 0.25f, "easeineaseout", false, false, false, false);

        yield return new WaitForSeconds(engineBlownTime - 0.5f);

        yield return MPAction.ScaleObject(engineBlownText, Vector2.one, Vector2.zero, 0.25f, "easeineaseout", false, false, false, false);

        engineSmoke.Stop();
    }

    protected override void FinishRace()
    {
        if (!finished)
        {
            finished = true;

            gm.numFinishers++;

            switch (gm.numFinishers)
            {
                case 1:
                    placeText.text = "1st Place!";
                    break;
                case 2:
                    placeText.text = "2nd Place!";
                    break;
                case 3:
                    placeText.text = "3rd Place!";
                    break;
                default:
                    placeText.text = "Last Place...";
                    break;
            }

            if (!gm.gpsPopped)
            {
                print("gps enabled");

                gm.gpsPopped = true;
                gm.gpsCanvas.enabled = true;
            }
        }
    }
}
