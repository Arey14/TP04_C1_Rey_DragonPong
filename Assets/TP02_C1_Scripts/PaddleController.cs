using UnityEngine;

/// <summary>
/// Controla el movimiento físico 2D de la paleta del jugador (WASD / Flechas) mediante AddForce.
/// Implementa límites estrictos por mitad de cancha, cambios de color según impacto (Negro al chocar límites, aleatorio con pelota),
/// e inicialización mediante GameSettings.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    [Header("Configuración General")]
    // Error: Player1 y Player2 tienen gameSettings: {fileID: 0} en SampleScene.unity, así que paddleSpeed y paddleHeight del SO nunca se leen y manda el speed: 15 del inspector.
    // Asigná GameSettings.asset en las dos paletas para que el ScriptableObject sea realmente la fuente de inicialización.
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private bool isPlayer1 = true;

    [Header("Configuración de Teclas")]
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    [Header("Parámetros de Movimiento")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private bool usePositionClamping = true;

    [Header("Límites de Cancha")]
    [SerializeField] private float yBoundary = 4.2f;
    [SerializeField] private float minX = -8.0f;
    [SerializeField] private float maxX = -0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        // Configuración de físicas 2D: Sin gravedad y rotación fija
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Auto-detectar si es Jugador 1 o Jugador 2 según la posición inicial X
        if (transform.position.x > 0)
        {
            isPlayer1 = false;
        }

        SetupDefaultBounds();
    }

    void Start()
    {
        // Cargar valores iniciales desde GameSettings si existen
        if (gameSettings != null)
        {
            speed = gameSettings.paddleSpeed;
            SetPaddleHeight(gameSettings.paddleHeight);
        }
    }

    /// <summary>
    /// Configura los límites de movimiento según el lado de la cancha del jugador.
    /// </summary>
    private void SetupDefaultBounds()
    {
        if (isPlayer1)
        {
            // Jugador 1 (Lado izquierdo): Entre el arco izquierdo y la línea media
            // Suggestion: Acá se pisan minX, maxX e isPlayer1 que están serializados en el inspector (Player2 tiene isPlayer1: 1, minX: -8 y maxX: -0.3 en la escena, y se ignoran).
            // O los límites salen del inspector (o de GameSettings), o no los serialices. Tener las dos fuentes confunde y los números -8, -0.3, 0.3 y 8 quedan mágicos.
            minX = -8.0f;
            maxX = -0.3f;
            if (upKey == KeyCode.None) upKey = KeyCode.W;
            if (downKey == KeyCode.None) downKey = KeyCode.S;
            if (leftKey == KeyCode.None) leftKey = KeyCode.A;
            if (rightKey == KeyCode.None) rightKey = KeyCode.D;
        }
        else
        {
            // Jugador 2 (Lado derecho): Entre la línea media y el arco derecho
            minX = 0.3f;
            maxX = 8.0f;
            if (upKey == KeyCode.None) upKey = KeyCode.UpArrow;
            if (downKey == KeyCode.None) downKey = KeyCode.DownArrow;
            if (leftKey == KeyCode.None) leftKey = KeyCode.LeftArrow;
            if (rightKey == KeyCode.None) rightKey = KeyCode.RightArrow;
        }
    }

    void Update()
    {
        // Lectura de inputs 2D (Vertical y Horizontal)
        moveInput = Vector2.zero;

        if (Input.GetKey(upKey)) moveInput.y += 1f;
        if (Input.GetKey(downKey)) moveInput.y -= 1f;
        if (Input.GetKey(rightKey)) moveInput.x += 1f;
        if (Input.GetKey(leftKey)) moveInput.x -= 1f;

        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // Movimiento físico mediante AddForce utilizando Time.fixedDeltaTime
        if (moveInput != Vector2.zero)
        {
            // Suggestion: ForceMode2D.Force ya integra la fuerza con el paso de física; multiplicar por Time.fixedDeltaTime * 50f da 1 con el timestep por defecto y no aporta nada.
            // Sacá el 50f mágico y ajustá speed como fuerza directa.
            Vector2 force = moveInput * (speed * Time.fixedDeltaTime * 50f);
            rb.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            // Frenado suave cuando no hay input
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
        }

        // Delimitación estricta de límites de su lado de la cancha
        if (usePositionClamping)
        {
            Vector3 pos = transform.position;
            bool hitBoundary = false;

            if (pos.x <= minX || pos.x >= maxX || pos.y <= -yBoundary || pos.y >= yBoundary)
            {
                hitBoundary = true;
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, -yBoundary, yBoundary);
            // Bug: Se reposiciona el transform de un Rigidbody2D dinámico pero no se toca su velocidad. Mientras el jugador empuja contra un límite, AddForce sigue sumando velocidad (no hay damping ni tope)
            // y al invertir la tecla la paleta queda pegada al borde hasta que la fuerza contraria compensa lo acumulado. Usá rb.position, anulá la componente de rb.linearVelocity que apunta hacia afuera y poné una velocidad máxima.
            transform.position = pos;

            // Si el paddle presiona contra el límite de la pantalla, cambia a color Negro
            if (hitBoundary && moveInput != Vector2.zero)
            {
                SetPaddleColor(Color.black);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con la pelota -> cambia a color aleatorio
        if (collision.gameObject.GetComponent<BallController>() != null)
        {
            SetPaddleColor(new Color(Random.value, Random.value, Random.value));
        }
        else
        {
            // Si choca con paredes o límites de la pantalla -> cambia a color Negro
            SetPaddleColor(Color.black);
        }
    }

    #region Métodos de Configuración y Personalización (Settings)

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetPaddleHeight(float newHeight)
    {
        Vector3 currentScale = transform.localScale;
        currentScale.y = newHeight;
        transform.localScale = currentScale;
    }

    public float GetPaddleHeight()
    {
        return transform.localScale.y;
    }

    public void SetPaddleColor(Color newColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
    }

    public Color GetPaddleColor()
    {
        return spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    public bool IsPlayer1()
    {
        return isPlayer1;
    }

    /// <summary>
    /// Restablece la paleta a su posición inicial original y anula la velocidad física.
    /// </summary>
    public void ResetPosition()
    {
        transform.position = startPosition;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        moveInput = Vector2.zero;
    }

    #endregion
}
