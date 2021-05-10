using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreateNeptune;
using TMPro;

public class PlayerController : CarController
{
    // Controls
    [SerializeField] private bool phoneTesting;
    private bool playerTouching;

    // Tachometer needle
    [SerializeField] private RectTransform tachNeedleRT;
    private Vector2 minTach = new Vector2(-950f, 350f);
    private Vector2 maxTach = new Vector2 (950f, 350f);

    // Power-up
    [SerializeField] private GameObject powerUpText;

    private void Update()
    {
        if (gm.gameState == GameManager.GameState.gameplay && !finished && !engineBlown)
        {
            GetInput();
        }
        else
            playerTouching = false;

        MoveTachometer();
        MakeEngineNoise();
    }

    private void MoveTachometer()
    {
        tachNeedleRT.anchoredPosition = minTach + (maxTach - minTach) * currentSpeed / topSpeed;
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
                    break;
                case TouchPhase.Stationary:
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
}
