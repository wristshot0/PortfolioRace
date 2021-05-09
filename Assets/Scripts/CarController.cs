using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarController : MonoBehaviour
{
    protected GameManager gm;
    protected Transform t;
    protected Rigidbody2D rb2d;

    // Physics
    [SerializeField] protected float currentSpeed;
    [SerializeField] protected float topSpeed;
    [SerializeField] protected float acceleration;
    [SerializeField] protected float brakingAcceleration;

    // Risk aversion varies between 0 and 1, where 0 is most risk tolerant and 1 is most risk averse.
    [SerializeField] protected float riskAversion;
    [SerializeField] protected float minBlowupTopSpeedPercentage;

    // Power-up
    [SerializeField] protected float powerUpTime;
    protected IEnumerator currentPowerUpRoutine;
    protected float normalRiskAversion;
    protected float normalTopSpeed;

    // Engine audio
    protected AudioSource engineAudio;

    protected void Awake()
    {
        t = transform;
        rb2d = GetComponent<Rigidbody2D>();
        engineAudio = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();

        normalRiskAversion = riskAversion;
        normalTopSpeed = topSpeed;
    }

    protected void MakeEngineNoise()
    {
        engineAudio.volume = currentSpeed / topSpeed * 0.25f;
    }

    protected abstract void FixedUpdate();

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected IEnumerator PowerUp(PowerUp powerUp)
    {
        riskAversion = powerUp.newRiskAversion;
        topSpeed = powerUp.newTopSpeed;

        yield return new WaitForSeconds(powerUpTime);

        riskAversion = normalRiskAversion;
        topSpeed = normalTopSpeed;
    }

    protected IEnumerator DestroyPowerUp(GameObject powerUp)
    {
        yield return new WaitForEndOfFrame();

        Destroy(powerUp);
    }

    protected void Drive()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, topSpeed);
    }

    protected void Brake()
    {
        currentSpeed = Mathf.Max(currentSpeed + brakingAcceleration * Time.deltaTime, 0f);
    }

    protected float PBlowUp()
    {
        return Mathf.Max(0f, Mathf.Min(1f, riskAversion * (currentSpeed
            - minBlowupTopSpeedPercentage * topSpeed) / (topSpeed - minBlowupTopSpeedPercentage * topSpeed)));
    }
}
