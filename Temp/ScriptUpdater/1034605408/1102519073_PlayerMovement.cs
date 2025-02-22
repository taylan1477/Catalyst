using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.0f; // Mevcut hız (pozitif veya negatif olabilir)
    public float acceleration = 0.012f; // Hızlanma oranı
    public float deceleration = 0.02f; // Yavaşlama oranı
    public float maxSpeed = 2.0f; // Maksimum hız
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer; // Sprite Renderer bileşeni

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Sprite Renderer'ı al
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
            if (speed < 0) // Eğer sola gidiyorsa, hızı tersine çevir
            {
                speed = 0; // Hızı sıfırla (isteğe bağlı, direkt tersine de çevrilebilir)
            }
            speed += acceleration; // Hızı artır
            _spriteRenderer.flipX = false; // Sprite'ı sağa dönük yap
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            if (speed > 0) // Eğer sağa gidiyorsa, hızı tersine çevir
            {
                speed = 0; // Hızı sıfırla (isteğe bağlı, direkt tersine de çevrilebilir)
            }
            speed -= acceleration; // Hızı azalt (negatif yönde artır)
            _spriteRenderer.flipX = true; // Sprite'ı sola dönük yap
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            if (speed > 0)
            {
                speed -= deceleration;
                if (speed < 0) speed = 0; // Hızı sıfırın altına düşürme
            }
            else if (speed < 0)
            {
                speed += deceleration;
                if (speed > 0) speed = 0; // Hızı sıfırın üstüne çıkarma
            }
        }

        // Hızı sınırla
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
    }

    void ApplyMovement()
    {
        // Hızı yön ile çarp ve karaktere uygula
        _rigidbody2D.linearVelocity = new Vector2(speed * 4, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        // Animator'a hızın mutlak değerini ilet
        _animator.SetFloat("speed", Mathf.Abs(speed)); // Hızın mutlak değerini kullan
    }
}