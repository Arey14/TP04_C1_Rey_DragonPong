using UnityEngine;

/// <summary>
/// ScriptableObject que centraliza las variables de configuración e inicialización del juego.
/// Permite configurar partidas al mejor de 5 (3 puntos para ganar), límite de 20s por punto y velocidades.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "DragonPong/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Reglas de Partida")]
    [Tooltip("Puntos necesarios para ganar (por defecto 3 para 'al mejor de 5')")]
    public int pointsToWin = 3;

    [Tooltip("Tiempo límite en segundos para anotar antes de cobrar gol en contra")]
    public float roundTimeLimit = 20f;

    [Header("Configuración de Paletas")]
    public float paddleSpeed = 15f;
    public float paddleHeight = 1f;

    [Header("Configuración de Pelota")]
    public float ballInitialSpeed = 8f;
    public float ballSpeedIncreasePerHit = 1.1f;
    public float ballMaxSpeed = 25f;
    public float ballStartDelay = 1.5f;

    [Header("Configuración de Obstáculos (Avanzado)")]
    public float obstacleWidth = 0.35f;
    public float obstacleHeight = 0.75f;
    public float obstacleMinLifetime = 3f;
    public float obstacleMaxLifetime = 7f;
    public float obstacleSpawnInterval = 4f;
    public int maxSimultaneousObstacles = 2;

    /// <summary>
    /// Genera una instancia por defecto en memoria si no se asignó un asset en el Inspector.
    /// </summary>
    public static GameSettings CreateDefaultSettings()
    {
        GameSettings settings = CreateInstance<GameSettings>();
        settings.pointsToWin = 3;
        settings.roundTimeLimit = 20f;
        settings.paddleSpeed = 15f;
        settings.paddleHeight = 1f;
        settings.ballInitialSpeed = 8f;
        settings.ballSpeedIncreasePerHit = 1.1f;
        settings.ballMaxSpeed = 25f;
        settings.ballStartDelay = 1.5f;
        settings.obstacleWidth = 0.35f;
        settings.obstacleHeight = 0.75f;
        settings.obstacleMinLifetime = 3f;
        settings.obstacleMaxLifetime = 7f;
        settings.obstacleSpawnInterval = 4f;
        settings.maxSimultaneousObstacles = 2;
        return settings;
    }
}
