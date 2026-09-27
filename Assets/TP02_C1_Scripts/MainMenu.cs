using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la navegación del Menú Principal y el inicio de la partida.
/// Mantiene el estado global de si la partida está en curso.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public static bool IsGamePlaying { get; private set; } = false;

    [Header("Paneles del Menú")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Botones del Menú Principal")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [Header("Botones de Regreso (Opcional)")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button creditsBackButton;

    [Header("Referencia a la Pelota")]
    [SerializeField] private BallController ball;

    void Start()
    {
        if (ball == null)
        {
            ball = FindFirstObjectByType<BallController>();
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlayGame);
            playButton.onClick.AddListener(PlayGame);
        }
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettings);
            settingsButton.onClick.AddListener(OpenSettings);
        }
        if (creditsButton != null)
        {
            creditsButton.onClick.RemoveListener(OpenCredits);
            creditsButton.onClick.AddListener(OpenCredits);
        }
        if (exitButton != null)
        {
            #if UNITY_WEBGL
            exitButton.gameObject.SetActive(false);
            #else
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                exitButton.gameObject.SetActive(false);
            }
            else
            {
                exitButton.onClick.RemoveListener(ExitGame);
                exitButton.onClick.AddListener(ExitGame);
            }
            #endif
        }

        // Manejador compartido de botones Back según el estado del juego
        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveListener(OnBackFromSubmenu);
            settingsBackButton.onClick.AddListener(OnBackFromSubmenu);
        }
        if (creditsBackButton != null)
        {
            creditsBackButton.onClick.RemoveListener(OnBackFromSubmenu);
            creditsBackButton.onClick.AddListener(OnBackFromSubmenu);
        }

        ShowMainMenu();
    }

    /// <summary>
    /// Lógica Botón Play: Inicia la partida y cambia el estado global a en juego.
    /// </summary>
    public void PlayGame()
    {
        IsGamePlaying = true;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMatch();
        }

        // Lanza la pelota con su retraso inicial configurado
        if (ball != null)
        {
            ball.ResetBall();
        }
    }

    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Muestra el menú principal y detiene el tiempo.
    /// </summary>
    public void ShowMainMenu()
    {
        IsGamePlaying = false;

        gameObject.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Maneja el regreso desde Settings o Credits hacia el Menú Principal si no se está jugando.
    /// </summary>
    public void OnBackFromSubmenu()
    {
        if (!IsGamePlaying)
        {
            ShowMainMenu();
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
        #else
        Application.Quit();
        #endif
    }
}