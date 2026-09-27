using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona el estado de la partida, marcador al mejor de 5, temporizador de 20s y condición de victoria.
/// Cumple con la regla de gol por tiempo agotado según la mitad de cancha donde esté la pelota.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración")]
    [SerializeField] private GameSettings gameSettings;

    [Header("Referencias de Escena")]
    [SerializeField] private BallController ball;

    [Header("HUD / UI")]
    [SerializeField] private TextMeshProUGUI scoreTextP1;
    [SerializeField] private TextMeshProUGUI scoreTextP2;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private int scoreP1 = 0;
    private int scoreP2 = 0;
    private int pointsToWin = 3;
    private float roundTimeRemaining = 20f;
    private float roundTimeLimit = 20f;
    private bool isTimerRunning = false;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadSettings();
        AutoFindUI();
    }

    void Start()
    {
        if (ball == null)
        {
            ball = FindFirstObjectByType<BallController>(FindObjectsInactive.Include);
        }

        AutoFindUI();
        BindButtons();
        ResetMatch();
    }

    private void LoadSettings()
    {
        if (gameSettings != null)
        {
            pointsToWin = gameSettings.pointsToWin;
            roundTimeLimit = gameSettings.roundTimeLimit;
        }
        else
        {
            pointsToWin = 3; // Al mejor de 5
            roundTimeLimit = 20f; // 20 segundos límite
        }
    }

    /// <summary>
    /// Auto-vincula componentes de UI (incluyendo inactivos) si no se configuraron en el Inspector.
    /// </summary>
    public void AutoFindUI()
    {
        // 1. Textos
        if (scoreTextP1 == null || scoreTextP2 == null || timerText == null || winnerText == null)
        {
            TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in texts)
            {
                string n = t.gameObject.name.ToLower();
                if ((n.Contains("score") || n.Contains("puntaje") || n.Contains("gol")) && (n.Contains("1") || n.Contains("p1") || n.Contains("left")) && scoreTextP1 == null)
                    scoreTextP1 = t;
                else if ((n.Contains("score") || n.Contains("puntaje") || n.Contains("gol")) && (n.Contains("2") || n.Contains("p2") || n.Contains("right")) && scoreTextP2 == null)
                    scoreTextP2 = t;
                else if ((n.Contains("timer") || n.Contains("tiempo") || n.Contains("reloj")) && timerText == null)
                    timerText = t;
                else if ((n.Contains("winner") || n.Contains("ganador") || n.Contains("victory") || n.Contains("victoria")) && winnerText == null)
                    winnerText = t;
            }
        }

        // 2. Panel Game Over
        if (gameOverPanel == null)
        {
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var obj in allObjects)
            {
                string n = obj.name.ToLower();
                if (n.Contains("gameover") || n.Contains("game_over") || n.Contains("findejuego") || n.Contains("fin_partida") || n.Contains("victorypanel"))
                {
                    gameOverPanel = obj;
                    break;
                }
            }
        }

        // 3. Botones (incluso dentro de paneles inactivos)
        if (restartButton == null || menuButton == null)
        {
            Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var b in buttons)
            {
                string n = b.gameObject.name.ToLower();
                if ((n.Contains("restart") || n.Contains("reintentar") || n.Contains("again") || n.Contains("reiniciar") || n.Contains("playagain")) && restartButton == null)
                {
                    restartButton = b;
                }
                else if ((n.Contains("menu") || n.Contains("mainmenu") || n.Contains("volver") || n.Contains("exitmenu") || n.Contains("menubutton")) && menuButton == null)
                {
                    // Evitar vincular los botones del menú principal propiamente dichos
                    if (!n.Contains("play") && !n.Contains("settings") && !n.Contains("credits"))
                    {
                        menuButton = b;
                    }
                }
            }
        }
    }

    private void BindButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartMatch);
            restartButton.onClick.AddListener(RestartMatch);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(ReturnToMenu);
            menuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    void Update()
    {
        if (isGameOver || !MainMenu.IsGamePlaying) return;

        // Cuenta regresiva del temporizador de 20s mientras la pelota esté en juego
        if (isTimerRunning)
        {
            roundTimeRemaining -= Time.deltaTime;

            if (roundTimeRemaining <= 0f)
            {
                roundTimeRemaining = 0f;
                HandleTimeExpired();
            }

            UpdateTimerUI();
        }
    }

    /// <summary>
    /// Inicia el temporizador de 20s cuando la pelota es puesta en juego.
    /// </summary>
    public void OnBallLaunched()
    {
        roundTimeRemaining = roundTimeLimit;
        isTimerRunning = true;
        UpdateTimerUI();
    }

    /// <summary>
    /// Se ejecuta cuando se agotan los 20 segundos sin gol.
    /// Determina en qué mitad de cancha se encuentra la pelota y cobra el gol en contra correspondiente.
    /// </summary>
    private void HandleTimeExpired()
    {
        isTimerRunning = false;

        if (ball != null)
        {
            float ballX = ball.transform.position.x;
            if (ballX < 0f)
            {
                // Pelota en campo de Jugador 1 -> Gol en contra para P1 (Punto para P2)
                ScoreGoal(scoringPlayer: 2, isAutoGoal: true);
            }
            else
            {
                // Pelota en campo de Jugador 2 -> Gol en contra para P2 (Punto para P1)
                ScoreGoal(scoringPlayer: 1, isAutoGoal: true);
            }
        }
        else
        {
            if (ball != null) ball.ResetBall();
        }
    }

    /// <summary>
    /// Registra un gol para el jugador indicado, actualiza el marcador y comprueba la victoria.
    /// </summary>
    public void ScoreGoal(int scoringPlayer, bool isAutoGoal = false)
    {
        if (isGameOver) return;

        isTimerRunning = false;

        if (scoringPlayer == 1)
        {
            scoreP1++;
        }
        else
        {
            scoreP2++;
        }

        UpdateScoreUI();

        // Comprobar condición de victoria (al llegar a pointsToWin goles)
        if (scoreP1 >= pointsToWin)
        {
            EndGame(1);
        }
        else if (scoreP2 >= pointsToWin)
        {
            EndGame(2);
        }
        else
        {
            // Continuar partido: lanzar pelota nuevamente
            if (ball != null)
            {
                ball.ResetBall();
            }
        }
    }

    /// <summary>
    /// Notificación desde GoalTrigger al cruzar la pelota el arco.
    /// </summary>
    public void OnGoalScored(bool isPlayer1Goal)
    {
        // Si entra en el arco del Jugador 1, el punto es para Jugador 2, y viceversa
        int scoringPlayer = isPlayer1Goal ? 2 : 1;
        ScoreGoal(scoringPlayer);
    }

    private void EndGame(int winner)
    {
        isGameOver = true;
        isTimerRunning = false;

        if (winnerText != null)
        {
            winnerText.text = $"¡Jugador {winner} es el Ganador!";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Re-asegurar listeners de los botones al mostrar el panel
        BindButtons();

        Time.timeScale = 0f;
    }

    public void RestartMatch()
    {
        ResetMatch();
        ResetPaddles();
        ClearObstacles();
        Time.timeScale = 1f;

        if (ball != null)
        {
            ball.ResetBall();
        }
    }

    public void ResetMatch()
    {
        scoreP1 = 0;
        scoreP2 = 0;
        isGameOver = false;
        isTimerRunning = false;
        roundTimeRemaining = roundTimeLimit;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        ResetPaddles();
        ClearObstacles();

        UpdateScoreUI();
        UpdateTimerUI();
    }

    /// <summary>
    /// Destruye todos los obstáculos activos en la cancha.
    /// </summary>
    public void ClearObstacles()
    {
        ObstacleSpawner spawner = FindFirstObjectByType<ObstacleSpawner>(FindObjectsInactive.Include);
        if (spawner != null)
        {
            spawner.ClearAllObstacles();
        }
        else
        {
            Obstacle[] allObstacles = FindObjectsByType<Obstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var obs in allObstacles)
            {
                if (obs != null)
                {
                    Destroy(obs.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Restablece las paletas de ambos jugadores a sus posiciones de inicio.
    /// </summary>
    public void ResetPaddles()
    {
        PaddleController[] paddles = FindObjectsByType<PaddleController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var paddle in paddles)
        {
            if (paddle != null)
            {
                paddle.ResetPosition();
            }
        }
    }

    /// <summary>
    /// Limpia el estado de juego y regresa de manera robusta al Menú Principal.
    /// </summary>
    public void ReturnToMenu()
    {
        isGameOver = false;
        isTimerRunning = false;
        roundTimeRemaining = roundTimeLimit;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Limpiar obstáculos activos
        ObstacleSpawner spawner = FindFirstObjectByType<ObstacleSpawner>(FindObjectsInactive.Include);
        if (spawner != null)
        {
            spawner.ClearAllObstacles();
        }

        // Detener la pelota
        if (ball != null)
        {
            ball.StopAllCoroutines();
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
            ball.transform.position = Vector3.zero;
        }

        ResetMatch();

        // Buscar MainMenu (incluyendo inactivos)
        MainMenu menu = FindFirstObjectByType<MainMenu>(FindObjectsInactive.Include);
        if (menu != null)
        {
            menu.gameObject.SetActive(true);
            menu.ShowMainMenu();
        }

        Time.timeScale = 0f;
    }

    private void UpdateScoreUI()
    {
        if (scoreTextP1 != null) scoreTextP1.text = scoreP1.ToString();
        if (scoreTextP2 != null) scoreTextP2.text = scoreP2.ToString();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"{Mathf.CeilToInt(roundTimeRemaining)}s";
            timerText.color = (roundTimeRemaining <= 5f && isTimerRunning) ? Color.red : Color.white;
        }
    }

    public int GetScoreP1() => scoreP1;
    public int GetScoreP2() => scoreP2;
    public bool IsGameOver() => isGameOver;
}
