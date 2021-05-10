using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUController : CarController
{
    // Aggression determines how fast CPU is willing to go relative to topSpeed before it slows down.
    [SerializeField] private float aggression;

    private void Update()
    {
        MakeEngineNoise();
    }

    protected override void FixedUpdate()
    {
        if (gm.gameState == GameManager.GameState.gameplay)
        {
            if (currentSpeed < aggression * topSpeed && !engineBlown)
                Drive();
            else
                Brake();

            rb2d.MovePosition(rb2d.position + currentSpeed * (Vector2)t.up * Time.deltaTime);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("powerup"))
        {
            if (currentPowerUpRoutine != null)
                StopCoroutine(currentPowerUpRoutine);

            currentPowerUpRoutine = PowerUp(collision.GetComponent<PowerUp>());
            StartCoroutine(currentPowerUpRoutine);

            StartCoroutine(DestroyPowerUp(collision.gameObject));
        }
        else if (collision.CompareTag("finishline"))
            FinishRace();
    }
}
