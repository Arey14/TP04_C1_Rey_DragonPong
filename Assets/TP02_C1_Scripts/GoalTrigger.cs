using UnityEngine;

/// <summary>
/// Detecta cuando la pelota cruza el arco y notifica al GameManager para sumar el gol al jugador rival.
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [Header("Configuración de Arco")]
    [Tooltip("Indica si este arco pertenece al Jugador 1 (true) o Jugador 2 (false)")]
    [SerializeField] private bool isPlayer1Goal = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball != null)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGoalScored(isPlayer1Goal);
            }
            else
            {
                ball.ResetBall();
            }
        }
    }
}
