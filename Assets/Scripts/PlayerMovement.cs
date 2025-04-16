using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; // Mevcut hız
    public float acceleration = 0.08f; // Hızlanma
    public float deceleration = 0.12f; // Yavaşlama
    public float maxSpeed = 12f; // Maks hız
    
    public float normalJumpForce = 20f; // Normal zıplama
    public float chargedJumpForce = 28f; // Charged Jump 
    public float chargeThreshold = 0.6f; // Charged Jump için gereken süre
    public float groundCheckDistance = 0.1f; // Ground check pornosu
    public float jumpBufferTime = 0.12f; // Buffer süresi
    private float _jumpBufferTimer;
    public float coyoteTime = 0.12f;    // Coyote jump süresi
    private float _coyoteTimeTimer; // Coyote time için sayaç
    
    public bool isSlowed; // Yavaşlatma durumu
    public bool isPushing; // Çekiş Porn bebeğim
    public bool isPulling;

    public float attackRange = 1f; // Vuruş menzili
    public int attackDamage = 1; // Vuruş hasarı
    public LayerMask mouseLayer; // Fare katmanı
    public Transform carryPosition; // Fareyi taşıma pozisyonu (kedinin ağzı)
    private GameObject _carriedMouse; // Taşınan fare
    private bool _isCarrying; // Fare taşınıyor mu?

    public LayerMask groundLayer;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    [SerializeField] private SpriteRenderer spriteRenderer; // Serialize ederiz, dışarıdan atanabilir
    public SpriteRenderer SpriteRenderer => spriteRenderer; // Read-only property
    private bool _isGrounded; 
    private bool _isCharging;
    private float _chargeStartTime;
    private bool _isStoping;


    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        // Eğer Inspector'dan atamadıysan, otomatik al
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (isSlowed == false)
            {
                SpriteRenderer.flipX = false; // Kediyi sola çevir
                UpdateCarryPosition(); // CarryPosition'ı güncelle
            }
            _isStoping = false;
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            if (speed > 0) speed *= -1;  // Eğer sağa gidiyorsa, hızı tersine çevir
            speed -= acceleration;
            if (isSlowed == false)
            {
                SpriteRenderer.flipX = true; // Kediyi sola çevir
                UpdateCarryPosition(); // CarryPosition'ı güncelle
            }
            _isStoping = false;
        }
        else if (Input.GetKey(KeyCode.S) && Mathf.Abs(speed) > 0) // Fren
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration*3f);
            _isStoping = true;
            if (speed == 0)
            {
                _isStoping = false;
            }
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration);
            _isStoping = false;
        }
        
        if (isSlowed)
        {
            // Yavaşlatılmış hızı hesapla ve sınırla
            float slowedSpeed = speed * 0.3f; // Hızı yarıya indir
            speed = Mathf.Clamp(speed, -maxSpeed * 0.3f, maxSpeed * 0.3f);
            slowedSpeed = Mathf.Clamp(slowedSpeed, -maxSpeed * 0.3f, maxSpeed * 0.3f); // Yavaşlatılmış hızı sınırla
            _rigidbody2D.linearVelocity = new Vector2(slowedSpeed, _rigidbody2D.linearVelocity.y);
        }
        else
        {
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
            _rigidbody2D.linearVelocity = new Vector2(speed, _rigidbody2D.linearVelocity.y);
        }
    }

    void UpdateAnimator()
    {
        _animator.SetFloat(AnimatorHashes.Speed, Mathf.Abs(speed));
        _animator.SetBool(AnimatorHashes.IsGrounded, _isGrounded);
        _animator.SetBool(AnimatorHashes.IsCharging, _isCharging);
        _animator.SetBool(AnimatorHashes.IsStoping, _isStoping);
        _animator.SetBool(AnimatorHashes.IsPushing, isPushing);
        _animator.SetBool(AnimatorHashes.IsPulling, isPulling);
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
        // Space tuşuna basıldığında jump buffering başlat
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferTimer = jumpBufferTime;
            if (_isGrounded)
            {
                _isCharging = true;           // Yere değdiğinde charged jump için şarj başlasın
                _chargeStartTime = Time.time;   // Şarj başlangıç zamanı
            }
        }

        // Zamanlayıcıları güncelle
        if (_jumpBufferTimer > 0)
            _jumpBufferTimer -= Time.deltaTime;

        if (_isGrounded)
            _coyoteTimeTimer = coyoteTime;
        else
            _coyoteTimeTimer -= Time.deltaTime;

        // Space tuşu bırakıldığında charged jump ya da normal jump gerçekleştir
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_isCharging)
            {
                // Charged jump: Tuş ne kadar uzun tutulduysa ona göre zıplama kuvveti
                float heldTime = Time.time - _chargeStartTime;
                float jumpPower = (heldTime >= chargeThreshold) ? chargedJumpForce : normalJumpForce;
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpPower);
                _isCharging = false;
                _jumpBufferTimer = 0;
                _coyoteTimeTimer = 0;
            }
            else if (_jumpBufferTimer > 0 && _coyoteTimeTimer > 0)
            {
                // Eğer _isCharging false ise, yani havadaysa (örneğin buffer sayesinde) normal zıplama
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, normalJumpForce);
                _jumpBufferTimer = 0;
                _coyoteTimeTimer = 0;
            }
        }
    }

    void Attack()
    {
        // Saldırı animasyonunu başlat
        _animator.SetTrigger(AnimatorHashes.AttackTrigger);

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
            if (SpriteRenderer.flipX) // Sola bakıyorsa
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