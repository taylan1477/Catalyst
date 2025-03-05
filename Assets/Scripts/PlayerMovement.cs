using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; // Mevcut hız
    public float acceleration = 0.08f; // Hızlanma
    public float deceleration = 0.12f; // Yavaşlama
    public float maxSpeed = 14f; // Maks hız
    public float normalJumpForce = 20f; // Normal zıplama
    public float chargedJumpForce = 28f; // Charged Jump 
    public float chargeThreshold = 0.6f; // Charged Jump için gereken süre
    public float groundCheckDistance = 0.1f; // Ground check mesafesi

    public bool isSlowed; // Yavaşlatma durumu

    public float attackRange = 1f; // Vuruş menzili
    public int attackDamage = 1; // Vuruş hasarı
    public LayerMask mouseLayer; // Fare katmanı
    public Transform carryPosition; // Fareyi taşıma pozisyonu (kedinin ağzı)
    private GameObject _carriedMouse; // Taşınan fare
    private bool _isCarrying; // Fare taşınıyor mu?

    public LayerMask groundLayer;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded; 
    private bool _isCharging;
    private bool _isStoping;
    private float _chargeStartTime;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimator();
        CheckGrounded();
        HandleJump();
        if (Input.GetKeyDown(KeyCode.E)) // Boşluk tuşu ile vur
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.F)) // F tuşuna basıldığında
        {
            if (_isCarrying)
            {
                DropMouse(); // Fareyi bırak
            }
            else
            {
                TryPickUpMouse(); // Fareyi al
            }
        }
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.D)) // Sağa hareket
        {
            if (speed < 0) speed *= -1; // Eğer sola gidiyorsa, hızı tersine çevir
            speed += acceleration;
            _spriteRenderer.flipX = false; // Kediyi sağa çevir
            UpdateCarryPosition(); // CarryPosition'ı güncelle
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            if (speed > 0) speed *= -1;
            speed -= acceleration;
            _spriteRenderer.flipX = true; // Kediyi sola çevir
            UpdateCarryPosition(); // CarryPosition'ı güncelle
        }
        else if (Input.GetKey(KeyCode.S) && Mathf.Abs(speed) > 0) // Fren
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration*2f);
            _isStoping = true;
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration);
            _isStoping = false;
        }
        
        // BURAYA isSlowed i ekleriz hem maks hem de uygulanan hızı 2 ya da 4 kat azaltırız
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed); // Hızı sınırla
        _rigidbody2D.linearVelocity = new Vector2(speed, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        _animator.SetFloat("speed", Mathf.Abs(speed));
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isCharging", _isCharging);
        _animator.SetBool("isStoping", _isStoping);
    }

    void CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;
        float radius = 0.2f;
        float distance = groundCheckDistance;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, distance, groundLayer);
        _isGrounded = hit.collider != null;

        Debug.DrawRay(origin, direction * distance, _isGrounded ? Color.green : Color.red);
    }

    void HandleJump()
    {
        if (_isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isCharging = true;
                _chargeStartTime = Time.time; // Basıldığı an kaydedilir
            }

            if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
            {
                float heldTime = Time.time - _chargeStartTime;
                float jumpPower = (heldTime >= chargeThreshold) ? chargedJumpForce : normalJumpForce;

                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpPower);

                _isCharging = false;
            }
        }
    }

    void Attack()
    {
        // Saldırı animasyonunu başlat
        _animator.SetTrigger("AttackTrigger");

        // Fareleri algıla ve hasar ver
        Collider2D[] hitMice = Physics2D.OverlapCircleAll(transform.position, attackRange, mouseLayer);
        foreach (Collider2D mouseCollider in hitMice)
        {
            MouseController mouse = mouseCollider.GetComponent<MouseController>();
            if (mouse != null)
            {
                mouse.TakeDamage(attackDamage);
            }
        }
    }

    void TryPickUpMouse()
    {
        // Kedinin etrafındaki fareleri algıla
        Collider2D[]
            nearbyMice = Physics2D.OverlapCircleAll(transform.position, 1f); // 1 birim yarıçapında fareleri algıla
        foreach (Collider2D mouseCollider in nearbyMice)
        {
            MouseController mouse = mouseCollider.GetComponent<MouseController>();
            if (mouse != null && mouse.IsDead()) // Fare öldüyse
            {
                PickUpMouse(mouseCollider.gameObject); // Fareyi al
                break;
            }
        }
    }

    void PickUpMouse(GameObject mouse)
    {
        _carriedMouse = mouse;
        _isCarrying = true;

        // Fareyi kedinin child objesi yap
        _carriedMouse.transform.SetParent(carryPosition);
        _carriedMouse.transform.localPosition = Vector3.zero; // Fareyi ağzın tam ortasına yerleştir

        // Fare collider'ını devre dışı bırak (çarpışmaları engelle)
        _carriedMouse.GetComponent<Collider2D>().enabled = false;

        // Fare rigidbody'sini devre dışı bırak (yerçekimi etkisini kaldır)
        Rigidbody2D mouseRigidbody = _carriedMouse.GetComponent<Rigidbody2D>();
        if (mouseRigidbody != null)
        {
            mouseRigidbody.simulated = false; // Rigidbody'yi devre dışı bırak
        }
    }

    void DropMouse()
    {
        if (_carriedMouse != null)
        {
            _isCarrying = false;

            // Fareyi kedinin child objesi olmaktan çıkar
            _carriedMouse.transform.SetParent(null);

            // Fare collider'ını tekrar etkinleştir
            _carriedMouse.GetComponent<Collider2D>().enabled = true;

            // Fare rigidbody'sini tekrar etkinleştir
            Rigidbody2D mouseRigidbody = _carriedMouse.GetComponent<Rigidbody2D>();
            if (mouseRigidbody != null)
            {
                mouseRigidbody.simulated = true; // Rigidbody'yi tekrar aktif et
            }

            // Fare bırakıldığı anda dişi kediye yakın mı kontrol et
            FemaleCat femaleCat = FindAnyObjectByType<FemaleCat>();
            if (femaleCat != null && femaleCat.IsMouseNearby())
            {
                Debug.Log("Fare dişi kedinin yanına bırakıldı!");
                femaleCat.GetComponent<Animator>().SetTrigger("ReceiveMouse");
                Destroy(_carriedMouse);
            }
            _carriedMouse = null;
        }
    }

    void UpdateCarryPosition()
    {
        if (_isCarrying)
        {
            // Kedinin yönüne göre carryPosition'ı güncelle
            if (_spriteRenderer.flipX) // Sola bakıyorsa
            {
                carryPosition.localPosition = new Vector3(-0.065f, 0.06f, 0); // Sola göre pozisyon
                _carriedMouse.GetComponent<SpriteRenderer>().flipX = true; // Fareyi sola çevir
            }
            else // Sağa bakıyorsa
            {
                carryPosition.localPosition = new Vector3(0.065f, 0.06f, 0); // Sağa göre pozisyon
                _carriedMouse.GetComponent<SpriteRenderer>().flipX = false; // Fareyi sağa çevir
            }
        }
    }
}