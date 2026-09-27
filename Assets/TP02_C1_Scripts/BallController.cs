using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el movimiento físico de la pelota, su lanzamiento con AddForce, rebote y aceleración por impactos.
/// Se sincroniza con GameManager y GameSettings para el manejo de rondas y temporizador de 20 segundos.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private GameSettings gameSettings;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float initialSpeed = 8f;
    [SerializeField] private float speedIncreasePerHit = 1.1f;
    [SerializeField] private float maxSpeed = 25f;

    [Header("Configuración de Lanzamiento")]
    [SerializeField] private float startDelay = 1.5f;
    [SerializeField] private bool autoLaunchOnStart = false;

    private Rigidbody2D rb;
    private float currentSpeed;
    private Vector2 startPosition;
    private Coroutine launchCoroutine;
    private bool isInPlay = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // Cargar configuración de GameSettings si existe
        if (gameSettings != null)
        {
            initialSpeed = gameSettings.ballInitialSpeed;
            speedIncreasePerHit = gameSettings.ballSpeedIncreasePerHit;
            maxSpeed = gameSettings.ballMaxSpeed;
            startDelay = gameSettings.ballStartDelay;
        }

        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;

        if (autoLaunchOnStart)
        {
            ResetBall();
        }
    }

    /// <summary>
    /// Detiene la pelota, la ubica en el centro y programa el nuevo saque tras el retraso inicial.
    /// </summary>
    public void ResetBall()
    {
        isInPlay = false;
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(ResetAndLaunchRoutine());
    }

    private IEnumerator ResetAndLaunchRoutine()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        currentSpeed = initialSpeed;

        yield return new WaitForSeconds(startDelay);

        LaunchBall();
    }

    /// <summary>
    /// Aplica una fuerza física inicial (AddForce) hacia una dirección aleatoria y notifica al GameManager.
    /// </summary>
    private void LaunchBall()
    {
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        Vector2 launchDirection = new Vector2(dirX, dirY).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(launchDirection * currentSpeed, ForceMode2D.Impulse);
        isInPlay = true;

        // Notificar al GameManager que la pelota está en juego para iniciar/reiniciar el temporizador
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBallLaunched();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PaddleController paddle = collision.gameObject.GetComponent<PaddleController>();
        if (paddle != null)
        {
            HandlePaddleBounce(collision, paddle);
        }
    }

    /// <summary>
    /// Calcula el rebote angular sobre la paleta e incrementa la velocidad física.
    /// </summary>
    private void HandlePaddleBounce(Collision2D collision, PaddleController paddle)
    {
        currentSpeed = Mathf.Min(currentSpeed * speedIncreasePerHit, maxSpeed);

        float ballX = transform.position.x;
        float paddleX = paddle.transform.position.x;
        float dirX = (ballX > paddleX) ? 1f : -1f;

        float paddleHeight = paddle.GetPaddleHeight();
        float hitOffset = transform.position.y - paddle.transform.position.y;
        float normalizedHit = hitOffset / (paddleHeight * 0.5f);
        normalizedHit = Mathf.Clamp(normalizedHit, -1f, 1f);

        Vector2 bounceDirection = new Vector2(dirX, normalizedHit).normalized;
        rb.linearVelocity = bounceDirection * currentSpeed;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public bool IsInPlay()
    {
        return isInPlay;
    }
}
