using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreateNeptune;

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
    protected bool engineBlown = false;
    [SerializeField] protected float engineCheckTime;
    [SerializeField] protected float engineBlownTime;
    [SerializeField] protected ParticleSystem engineSmoke;

    // Power-up
    [SerializeField] protected GameObject car;
    private Color startColor;
    [SerializeField] protected float powerUpTime;
    protected IEnumerator currentPowerUpRoutine;
    protected float normalRiskAversion;
    protected float normalTopSpeed;

    // Engine audio
    protected AudioSource engineAudio;

    // Finishing
    protected bool finished = false;

    protected void Awake()
    {
        t = transform;
        rb2d = GetComponent<Rigidbody2D>();
        engineAudio = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        startColor = car.GetComponent<SpriteRenderer>().color;
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();

        normalRiskAversion = riskAversion;
        normalTopSpeed = topSpeed;

        StartCoroutine(CheckEngineBlown());
    }

    protected IEnumerator CheckEngineBlown()
    {
        for (; ;)
        {
            yield return new WaitForSeconds(engineCheckTime);

            if (finished)
                yield break;

            if (Random.Range(0f, 1f) < PBlowUp())
            {
                engineBlown = true;

                print("engine blown!");

                StartCoroutine(EngineBlownRoutine());

                yield return new WaitForSeconds(engineBlownTime);

                print("engine recovered!");

                engineBlown = false;
            }
        }
    }

    protected abstract IEnumerator EngineBlownRoutine();

    protected void MakeEngineNoise()
    {
        engineAudio.volume = currentSpeed / topSpeed * 0.25f;
    }

    protected abstract void FixedUpdate();

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected IEnumerator PowerUp(PowerUp powerUp)
    {
        // Engine unblown!
        engineBlown = false;

        riskAversion = powerUp.newRiskAversion;
        topSpeed = powerUp.newTopSpeed;

        yield return MPAction.FlashAnimation(car, 0.5f, powerUpTime, startColor, Color.white, false, false, false);

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
        float pBlow = riskAversion * (currentSpeed
            - minBlowupTopSpeedPercentage * topSpeed)
            / (topSpeed - minBlowupTopSpeedPercentage * topSpeed);

        return Mathf.Max(0f, Mathf.Min(1f, pBlow));
    }

    protected abstract void FinishRace();
}
