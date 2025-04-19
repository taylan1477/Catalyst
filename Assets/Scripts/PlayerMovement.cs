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
    public float groundCheckDistance = 0.8f; // Ground check
    public float jumpBufferTime = 0.1f; // Buffer süresi
    private float _jumpBufferTimer;
    public float coyoteTime = 0.1f;    // Coyote jump süresi
    private float _coyoteTimeTimer; // Coyote time için sayaç
    
    public bool isSlowed; // Yavaşlatma durumu
    
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

    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.35f;
    private float _footstepTimer;
    private bool _wasMoving;

    void Update()
    {
        HandleMovement();
        UpdateAnimator();
        CheckGrounded();
        HandleJump();
        HandleFootsteps(); // Yeni eklenen fonksiyon
    }

    void HandleFootsteps()
    {
        if (_isGrounded && Mathf.Abs(speed) > 0.1f)
        {
            _footstepTimer -= Time.deltaTime;
            
            if (_footstepTimer <= 0)
            {
                PlayFootstepSound();
                ResetFootstepTimer();
            }
            
            _wasMoving = true;
        }
        else
        {
            if (_wasMoving)
            {
                // Hareket durduğunda son bir adım sesi çal
                PlayFootstepSound();
                _wasMoving = false;
            }
            ResetFootstepTimer();
        }
    }

    void PlayFootstepSound()
    {
        if (AudioManager.Instance != null)
        {
            // Mutlak hızı kullanarak uygun ses setini seç
            float currentSpeed = Mathf.Abs(_rigidbody2D.linearVelocity.x);
            AudioManager.Instance.PlayFootstep(currentSpeed);
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
    }

    void ResetFootstepTimer()
    {
        _footstepTimer = footstepInterval;
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
                // UpdateCarryPosition(); // CarryPosition'ı güncelle
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
                // UpdateCarryPosition(); // CarryPosition'ı güncelle
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
            float slowedSpeed = speed * 0.4f; // Hızı yarıya indir x = 3/8
            speed = Mathf.Clamp(speed, -maxSpeed * 0.4f, maxSpeed * 0.4f);
            slowedSpeed = Mathf.Clamp(slowedSpeed, -maxSpeed * 0.4f, maxSpeed * 0.4f); // Yavaşlatılmış hızı sınırla
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
}