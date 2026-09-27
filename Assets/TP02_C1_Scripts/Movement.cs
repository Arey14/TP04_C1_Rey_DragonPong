using UnityEngine;

public class Movement : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationAngle = 10.0f;
    [SerializeField] private KeyCode KeyCodeUp = KeyCode.W;
    [SerializeField] private KeyCode KeyCodeDown = KeyCode.S;
    [SerializeField] private KeyCode KeyCodeLeft = KeyCode.A;
    [SerializeField] private KeyCode KeyCodeRight = KeyCode.D;
    [SerializeField] private KeyCode KeyCodeRotateLeft = KeyCode.Q;
    [SerializeField] private KeyCode KeyCodeRotateRight = KeyCode.E;
    [SerializeField] private KeyCode KeyCodeChangeColor = KeyCode.R;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {   

        // 1. Logica Movimiento
        float step = speed * Time.deltaTime;

        // 2. Lógica movimiento
        if (Input.GetKey(KeyCodeUp))
        {
            transform.Translate(Vector2.up * step);
        }
        if (Input.GetKey(KeyCodeDown))
        {
            transform.Translate(Vector2.down * step);
        }
        if (Input.GetKey(KeyCodeLeft))
        {
            transform.Translate(Vector2.left * step);
        }
        if (Input.GetKey(KeyCodeRight))
        {
            transform.Translate(Vector2.right * step);
        }
    
        // 3. Lógica giritos
        if (Input.GetKeyDown(KeyCodeRotateLeft))
        {
            transform.Rotate(0.0f, 0.0f, rotationAngle * Time.timeScale, Space.Self);
        }

        if (Input.GetKeyDown(KeyCodeRotateRight))
        {
            transform.Rotate(0.0f, 0.0f, -rotationAngle * Time.timeScale, Space.Self);
        }
        
        // 4. Lógica colorcitos
        if (Input.GetKeyUp(KeyCodeChangeColor) && Time.timeScale != 0f)
        {
            sr.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    // 5. Lógica velocidad (Settings / Sliders)
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}