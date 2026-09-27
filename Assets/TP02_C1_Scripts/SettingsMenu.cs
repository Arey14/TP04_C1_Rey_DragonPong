using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona la configuración del juego dentro del menú de opciones (velocidad, alto de paleta y color de los jugadores).
/// Soporta asignación manual en Inspector y auto-vinculación automática por código, incluyendo botones de cambio de color.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Referencias a Jugadores (Se auto-vinculan si están vacíos)")]
    [SerializeField] private PaddleController player1;
    [SerializeField] private PaddleController player2;

    [Header("Configuración Velocidad - Jugador 1")]
    [SerializeField] private Slider speedSliderP1;
    [SerializeField] private TextMeshProUGUI speedTextP1;

    [Header("Configuración Velocidad - Jugador 2")]
    [SerializeField] private Slider speedSliderP2;
    [SerializeField] private TextMeshProUGUI speedTextP2;

    [Header("Configuración Altura Paleta - Jugador 1")]
    [SerializeField] private Slider heightSliderP1;
    [SerializeField] private TextMeshProUGUI heightTextP1;

    [Header("Configuración Altura Paleta - Jugador 2")]
    [SerializeField] private Slider heightSliderP2;
    [SerializeField] private TextMeshProUGUI heightTextP2;

    [Header("Botones de Cambio de Color")]
    [SerializeField] private Button colorButtonP1;
    [SerializeField] private Button colorButtonP2;
    [SerializeField] private Image colorPreviewP1;
    [SerializeField] private Image colorPreviewP2;

    [Header("Configuración de Colores Predefinidos")]
    [SerializeField] private Color[] colorPalette = new Color[] 
    { 
        Color.white, 
        new Color(0.2f, 0.6f, 1f), // Celeste / Azul
        new Color(1f, 0.3f, 0.3f), // Rojo
        new Color(0.3f, 0.9f, 0.4f), // Verde
        new Color(1f, 0.85f, 0.2f), // Amarillo
        new Color(0.8f, 0.3f, 0.9f), // Violeta / Magenta
        new Color(1f, 0.5f, 0.1f)   // Naranja
    };

    private int p1ColorIndex = 0;
    private int p2ColorIndex = 0;

    void Awake()
    {
        AutoFindReferences();
    }

    void Start()
    {
        AutoFindReferences();
        InitializeControls();
        SetupColorButtons();
    }

    void OnEnable()
    {
        AutoFindReferences();
        InitializeControls();
        SetupColorButtons();
    }

    /// <summary>
    /// Busca y vincula automáticamente los jugadores y componentes de UI si no fueron asignados en el Inspector.
    /// </summary>
    public void AutoFindReferences()
    {
        // 1. Auto-vincular Jugadores por posición X (Izquierda = P1, Derecha = P2)
        if (player1 == null || player2 == null)
        {
            PaddleController[] paddles = FindObjectsByType<PaddleController>(FindObjectsSortMode.None);
            if (paddles.Length >= 2)
            {
                if (paddles[0].transform.position.x < paddles[1].transform.position.x)
                {
                    if (player1 == null) player1 = paddles[0];
                    if (player2 == null) player2 = paddles[1];
                }
                else
                {
                    if (player1 == null) player1 = paddles[1];
                    if (player2 == null) player2 = paddles[0];
                }
            }
            else if (paddles.Length == 1 && player1 == null)
            {
                player1 = paddles[0];
            }
        }

        // 2. Auto-vincular Sliders por nombre entre los hijos
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        foreach (Slider s in sliders)
        {
            string name = s.gameObject.name.ToLower();
            if (name.Contains("speed") || name.Contains("velocidad"))
            {
                if ((name.Contains("1") || name.Contains("p1") || name.Contains("player1") || name.Contains("left") || name.Contains("izq")) && speedSliderP1 == null)
                    speedSliderP1 = s;
                else if ((name.Contains("2") || name.Contains("p2") || name.Contains("player2") || name.Contains("right") || name.Contains("der")) && speedSliderP2 == null)
                    speedSliderP2 = s;
            }
            else if (name.Contains("height") || name.Contains("alto") || name.Contains("tamaño") || name.Contains("size"))
            {
                if ((name.Contains("1") || name.Contains("p1") || name.Contains("player1") || name.Contains("left") || name.Contains("izq")) && heightSliderP1 == null)
                    heightSliderP1 = s;
                else if ((name.Contains("2") || name.Contains("p2") || name.Contains("player2") || name.Contains("right") || name.Contains("der")) && heightSliderP2 == null)
                    heightSliderP2 = s;
            }
        }

        // 3. Auto-vincular Textos TMP por nombre entre los hijos
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI t in texts)
        {
            string name = t.gameObject.name.ToLower();
            if (name.Contains("speed") || name.Contains("velocidad"))
            {
                if ((name.Contains("1") || name.Contains("p1") || name.Contains("player1")) && speedTextP1 == null)
                    speedTextP1 = t;
                else if ((name.Contains("2") || name.Contains("p2") || name.Contains("player2")) && speedTextP2 == null)
                    speedTextP2 = t;
            }
            else if (name.Contains("height") || name.Contains("alto") || name.Contains("size"))
            {
                if ((name.Contains("1") || name.Contains("p1") || name.Contains("player1")) && heightTextP1 == null)
                    heightTextP1 = t;
                else if ((name.Contains("2") || name.Contains("p2") || name.Contains("player2")) && heightTextP2 == null)
                    heightTextP2 = t;
            }
        }

        // 4. Auto-vincular Botones de Color
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button b in buttons)
        {
            string name = b.gameObject.name.ToLower();
            if (name.Contains("color"))
            {
                if ((name.Contains("1") || name.Contains("p1") || name.Contains("player1")) && colorButtonP1 == null)
                    colorButtonP1 = b;
                else if ((name.Contains("2") || name.Contains("p2") || name.Contains("player2")) && colorButtonP2 == null)
                    colorButtonP2 = b;
            }
        }
    }

    /// <summary>
    /// Sincroniza los valores de los sliders con el estado actual de los jugadores.
    /// </summary>
    private void InitializeControls()
    {
        // 1. Velocidad P1
        if (player1 != null && speedSliderP1 != null)
        {
            speedSliderP1.minValue = 5f;
            speedSliderP1.maxValue = 30f;
            speedSliderP1.value = player1.GetSpeed();
            UpdatePlayer1Speed(speedSliderP1.value);
            speedSliderP1.onValueChanged.RemoveListener(UpdatePlayer1Speed);
            speedSliderP1.onValueChanged.AddListener(UpdatePlayer1Speed);
        }

        // 2. Velocidad P2
        if (player2 != null && speedSliderP2 != null)
        {
            speedSliderP2.minValue = 5f;
            speedSliderP2.maxValue = 30f;
            speedSliderP2.value = player2.GetSpeed();
            UpdatePlayer2Speed(speedSliderP2.value);
            speedSliderP2.onValueChanged.RemoveListener(UpdatePlayer2Speed);
            speedSliderP2.onValueChanged.AddListener(UpdatePlayer2Speed);
        }

        // 3. Altura P1
        if (player1 != null && heightSliderP1 != null)
        {
            heightSliderP1.minValue = 0.5f;
            heightSliderP1.maxValue = 3f;
            heightSliderP1.value = player1.GetPaddleHeight();
            UpdatePlayer1Height(heightSliderP1.value);
            heightSliderP1.onValueChanged.RemoveListener(UpdatePlayer1Height);
            heightSliderP1.onValueChanged.AddListener(UpdatePlayer1Height);
        }

        // 4. Altura P2
        if (player2 != null && heightSliderP2 != null)
        {
            heightSliderP2.minValue = 0.5f;
            heightSliderP2.maxValue = 3f;
            heightSliderP2.value = player2.GetPaddleHeight();
            UpdatePlayer2Height(heightSliderP2.value);
            heightSliderP2.onValueChanged.RemoveListener(UpdatePlayer2Height);
            heightSliderP2.onValueChanged.AddListener(UpdatePlayer2Height);
        }
    }

    private void SetupColorButtons()
    {
        if (colorButtonP1 != null)
        {
            colorButtonP1.onClick.RemoveListener(CyclePlayer1Color);
            colorButtonP1.onClick.AddListener(CyclePlayer1Color);
        }

        if (colorButtonP2 != null)
        {
            colorButtonP2.onClick.RemoveListener(CyclePlayer2Color);
            colorButtonP2.onClick.AddListener(CyclePlayer2Color);
        }

        UpdateColorVisuals();
    }

    #region Handlers de Velocidad

    public void UpdatePlayer1Speed(float value)
    {
        if (player1 != null) player1.SetSpeed(value);
        if (speedTextP1 != null) speedTextP1.text = value.ToString("0.0");
    }

    public void UpdatePlayer2Speed(float value)
    {
        if (player2 != null) player2.SetSpeed(value);
        if (speedTextP2 != null) speedTextP2.text = value.ToString("0.0");
    }

    #endregion

    #region Handlers de Altura de Paleta

    public void UpdatePlayer1Height(float value)
    {
        if (player1 != null) player1.SetPaddleHeight(value);
        if (heightTextP1 != null) heightTextP1.text = value.ToString("0.0");
    }

    public void UpdatePlayer2Height(float value)
    {
        if (player2 != null) player2.SetPaddleHeight(value);
        if (heightTextP2 != null) heightTextP2.text = value.ToString("0.0");
    }

    #endregion

    #region Handlers de Color

    /// <summary>
    /// Cicla al siguiente color disponible para el Jugador 1.
    /// </summary>
    public void CyclePlayer1Color()
    {
        if (colorPalette.Length == 0) return;

        p1ColorIndex = (p1ColorIndex + 1) % colorPalette.Length;
        SetPlayer1Color(colorPalette[p1ColorIndex]);
        UpdateColorVisuals();
    }

    /// <summary>
    /// Cicla al siguiente color disponible para el Jugador 2.
    /// </summary>
    public void CyclePlayer2Color()
    {
        if (colorPalette.Length == 0) return;

        p2ColorIndex = (p2ColorIndex + 1) % colorPalette.Length;
        SetPlayer2Color(colorPalette[p2ColorIndex]);
        UpdateColorVisuals();
    }

    private void UpdateColorVisuals()
    {
        // Actualizar vista previa o color del botón P1
        if (player1 != null)
        {
            Color c1 = player1.GetPaddleColor();
            if (colorPreviewP1 != null) colorPreviewP1.color = c1;
            if (colorButtonP1 != null && colorButtonP1.targetGraphic != null)
            {
                colorButtonP1.targetGraphic.color = c1;
            }
        }

        // Actualizar vista previa o color del botón P2
        if (player2 != null)
        {
            Color c2 = player2.GetPaddleColor();
            if (colorPreviewP2 != null) colorPreviewP2.color = c2;
            if (colorButtonP2 != null && colorButtonP2.targetGraphic != null)
            {
                colorButtonP2.targetGraphic.color = c2;
            }
        }
    }

    /// <summary>
    /// Cambia el color de Player 1 pasando el índice de la paleta predefinida.
    /// </summary>
    public void SetPlayer1ColorIndex(int index)
    {
        if (index >= 0 && index < colorPalette.Length)
        {
            p1ColorIndex = index;
            SetPlayer1Color(colorPalette[index]);
            UpdateColorVisuals();
        }
    }

    /// <summary>
    /// Cambia el color de Player 2 pasando el índice de la paleta predefinida.
    /// </summary>
    public void SetPlayer2ColorIndex(int index)
    {
        if (index >= 0 && index < colorPalette.Length)
        {
            p2ColorIndex = index;
            SetPlayer2Color(colorPalette[index]);
            UpdateColorVisuals();
        }
    }

    /// <summary>
    /// Permite asignar un color directo a Player 1.
    /// </summary>
    public void SetPlayer1Color(Color color)
    {
        if (player1 != null) player1.SetPaddleColor(color);
        UpdateColorVisuals();
    }

    /// <summary>
    /// Permite asignar un color directo a Player 2.
    /// </summary>
    public void SetPlayer2Color(Color color)
    {
        if (player2 != null) player2.SetPaddleColor(color);
        UpdateColorVisuals();
    }

    #endregion
}
