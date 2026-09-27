using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el Menú de Pausa durante una partida en curso.
/// Gestiona la transición hacia submenús (Settings, Credits) y regreso correcto a la pausa.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Botones de Pausa")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [Header("Botones de Regreso a Pausa (Opcional)")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button creditsBackButton;

    private Canvas pauseCanvas;
    private bool isPaused = false;

    void Awake()
    {
        pauseCanvas = GetComponent<Canvas>();
    }

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(Resume);
            continueButton.onClick.AddListener(Resume);
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

        Resume();
    }

    void Update()
    {
        // Si el menú principal está en pantalla o el juego aún no ha iniciado, no permitimos pausar
        if (!MainMenu.IsGamePlaying || (mainMenuPanel != null && mainMenuPanel.activeInHierarchy))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si está dentro de Settings o Credits durante la pausa, Escape regresa al menú de pausa
            if (isPaused && ((settingsPanel != null && settingsPanel.activeInHierarchy) || (creditsPanel != null && creditsPanel.activeInHierarchy)))
            {
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Reanuda la partida y oculta todos los paneles de pausa/opciones.
    /// </summary>
    public void Resume()
    {
        SetPauseUIVisible(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        Time.timeScale = 1f; 
        isPaused = false;
    }

    /// <summary>
    /// Congela el juego y muestra la interfaz de pausa.
    /// </summary>
    public void Pause()
    {
        SetPauseUIVisible(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        Time.timeScale = 0f;  
        isPaused = true;
    }

    private void SetPauseUIVisible(bool visible)
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = visible;
        }
        else if (pauseMenuPanel != null && pauseMenuPanel != this.gameObject)
        {
            pauseMenuPanel.SetActive(visible);
        }
        else if (pauseMenuPanel != null && pauseMenuPanel == this.gameObject)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(visible);
            }
        }
    }

    public void OpenSettings()
    {
        SetPauseUIVisible(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        SetPauseUIVisible(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Regresa a la pantalla de Pausa desde un submenú durante una partida activa.
    /// </summary>
    public void BackToPauseMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        SetPauseUIVisible(true);
    }

    private void OnBackFromSubmenu()
    {
        if (MainMenu.IsGamePlaying)
        {
            BackToPauseMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        SetPauseUIVisible(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
        else
        {
            MainMenu menu = FindFirstObjectByType<MainMenu>(FindObjectsInactive.Include);
            if (menu != null)
            {
                menu.ShowMainMenu();
            }
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
