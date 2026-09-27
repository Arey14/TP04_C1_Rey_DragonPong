using System.Collections;
using UnityEngine;

/// <summary>
/// Componente de obstáculo temporal que se autodestruye aleatoriamente entre 3 y 7 segundos.
/// Posee colisionador físico 2D para interactuar y hacer rebotar la pelota.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float minLifetime = 3f;
    [SerializeField] private float maxLifetime = 7f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        float lifetime = Random.Range(minLifetime, maxLifetime);
        StartCoroutine(LifetimeRoutine(lifetime));
    }

    /// <summary>
    /// Configura los tiempos de vida si se desean sobreescribir desde el Spawner/GameSettings.
    /// </summary>
    public void SetLifetimeRange(float min, float max)
    {
        minLifetime = min;
        maxLifetime = max;
    }

    private IEnumerator LifetimeRoutine(float lifetime)
    {
        // Esperar el tiempo de vida menos los últimos 0.5s para realizar un fade-out suave
        float activeDuration = Mathf.Max(0.1f, lifetime - 0.5f);
        yield return new WaitForSeconds(activeDuration);

        // Desvanecimiento visual antes de destruirse
        if (spriteRenderer != null)
        {
            float fadeElapsed = 0f;
            Color initialColor = spriteRenderer.color;

            while (fadeElapsed < 0.5f)
            {
                fadeElapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / 0.5f);
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
