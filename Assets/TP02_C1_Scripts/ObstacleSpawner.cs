using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera obstáculos en posiciones aleatorias de la zona central de la cancha.
/// Soporta asignación directa de Sprite, Prefab personalizado o generación procedural.
/// Los obstáculos duran entre 3 y 7 segundos.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameSettings gameSettings;
    [Tooltip("Opcional: Asigna un Prefab completo si lo tienes armado")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Sprite / Imagen del Obstáculo")]
    [Tooltip("Arrastra tu imagen/sprite aquí para usarla como obstáculo")]
    [SerializeField] private Sprite obstacleSprite;

    [Header("Dimensiones de los Obstáculos")]
    [SerializeField] private Vector2 obstacleSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private Color obstacleColor = Color.white;

    [Header("Rotación y Variedad de Ángulos")]
    [SerializeField] private bool randomizeRotation = true;
    [SerializeField] private float minAngle = -75f;
    [SerializeField] private float maxAngle = 75f;

    [Header("Área Central de Spawn")]
    [SerializeField] private Vector2 centerSpawnArea = new Vector2(4.5f, 5.5f); // Rango central

    [Header("Intervalos de Generación")]
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxSimultaneousObstacles = 2;

    private readonly List<GameObject> activeObstacles = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private static PhysicsMaterial2D bouncyMat;

    void Start()
    {
        if (gameSettings != null)
        {
            obstacleSize = new Vector2(gameSettings.obstacleWidth, gameSettings.obstacleHeight);
            spawnInterval = gameSettings.obstacleSpawnInterval;
            maxSimultaneousObstacles = gameSettings.maxSimultaneousObstacles;
        }

        if (bouncyMat == null)
        {
            bouncyMat = new PhysicsMaterial2D("ObstacleBouncy")
            {
                bounciness = 1f,
                friction = 0f
            };
        }

        spawnCoroutine = StartCoroutine(SpawnLoopRoutine());
    }

    private IEnumerator SpawnLoopRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Solo spawnea si el juego está activo y no en pausa/gameOver
            if (MainMenu.IsGamePlaying && (GameManager.Instance == null || !GameManager.Instance.IsGameOver()))
            {
                CleanDeadReferences();

                if (activeObstacles.Count < maxSimultaneousObstacles)
                {
                    SpawnObstacle();
                }
            }
        }
    }

    /// <summary>
    /// Instancia un nuevo obstáculo en una posición aleatoria de la zona central con rotación aleatoria.
    /// </summary>
    public void SpawnObstacle()
    {
        float xPos = Random.Range(-centerSpawnArea.x * 0.5f, centerSpawnArea.x * 0.5f);
        float yPos = Random.Range(-centerSpawnArea.y * 0.5f, centerSpawnArea.y * 0.5f);
        Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

        // Ángulo de inclinación aleatorio para generar rebotes dinámicos
        float angle = randomizeRotation ? Random.Range(minAngle, maxAngle) : 0f;
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, angle);

        GameObject obstacleObj;

        if (obstaclePrefab != null)
        {
            obstacleObj = Instantiate(obstaclePrefab, spawnPos, spawnRotation);
        }
        else
        {
            // Generar obstáculo usando el Sprite asignado o forma procedural
            obstacleObj = CreateProceduralObstacle(spawnPos, spawnRotation);
        }

        // Configurar tiempo de vida desde GameSettings si corresponde
        Obstacle obsComp = obstacleObj.GetComponent<Obstacle>();
        if (obsComp != null && gameSettings != null)
        {
            obsComp.SetLifetimeRange(gameSettings.obstacleMinLifetime, gameSettings.obstacleMaxLifetime);
        }

        activeObstacles.Add(obstacleObj);
    }

    /// <summary>
    /// Crea un obstáculo 2D con SpriteRenderer, BoxCollider2D físico y componente Obstacle.
    /// </summary>
    private GameObject CreateProceduralObstacle(Vector3 position, Quaternion rotation)
    {
        GameObject obj = new GameObject("Obstacle_Dynamic");
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();

        if (obstacleSprite != null)
        {
            // Usar la imagen personalizada del usuario
            sr.sprite = obstacleSprite;
            sr.color = obstacleColor;

            // Escalar según el tamaño del sprite original y obstacleSize
            float spriteWidth = obstacleSprite.bounds.size.x;
            float spriteHeight = obstacleSprite.bounds.size.y;
            if (spriteWidth > 0f && spriteHeight > 0f)
            {
                obj.transform.localScale = new Vector3(obstacleSize.x / spriteWidth, obstacleSize.y / spriteHeight, 1f);
            }
            else
            {
                obj.transform.localScale = new Vector3(obstacleSize.x, obstacleSize.y, 1f);
            }
        }
        else
        {
            // Fallback: Bloque dorado sólido si no se asignó imagen
            Texture2D tex = Texture2D.whiteTexture;
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1f);
            sr.color = new Color(0.95f, 0.75f, 0.15f, 1f);
            obj.transform.localScale = new Vector3(obstacleSize.x / tex.width, obstacleSize.y / tex.height, 1f);
        }

        // Colisionador 2D con rebote físico
        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.sharedMaterial = bouncyMat;

        obj.AddComponent<Obstacle>();
        return obj;
    }

    private void CleanDeadReferences()
    {
        activeObstacles.RemoveAll(item => item == null);
    }

    /// <summary>
    /// Destruye todos los obstáculos en pantalla al cambiar de ronda o reiniciar la partida.
    /// </summary>
    public void ClearAllObstacles()
    {
        foreach (var obs in activeObstacles)
        {
            if (obs != null)
            {
                Destroy(obs);
            }
        }
        activeObstacles.Clear();

        // Limpiar cualquier obstáculo remanente en la escena
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
