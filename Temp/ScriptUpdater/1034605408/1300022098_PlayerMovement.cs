using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 0.012f; // Hızlanma oranı
    public float deceleration = 0.02f;  // Yavaşlama oranı
    public float maxSpeed = 2.0f;       // Maksimum hız
    private float speed = 0.0f;         // Mevcut hız
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private int direction = 1;          // 1: Sağa, -1: Sola

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        ApplyMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.D)) // Sağa hareket
        {
            direction = 1;
            speed += acceleration;
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            direction = -1;
            speed += acceleration;
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed -= deceleration;
        }

        // Hızı sınırla
        speed = Mathf.Clamp(speed, 0, maxSpeed);
    }

    void ApplyMovement()
    {
        // Hızı yön ile çarp ve karaktere uygula
        _rigidbody2D.linearVelocity = new Vector2(speed * direction, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        // Animator'a hızı ve yönü ilet
        _animator.SetFloat("speed", speed);
        _animator.SetInteger("direction", direction);
    }
}