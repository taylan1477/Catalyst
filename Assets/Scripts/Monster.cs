using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class Monster : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 5f;
    public float jumpForce = 10f; // Artırılmış zıplama gücü
    public float horizontalSpeed = 7f; // Yatay hız için yeni parametre
    public float jumpCooldown = 2f;
    
    [Header("References")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded;
    private float _lastJumpTime;
    private bool _isChasing;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        _isChasing = Vector2.Distance(transform.position, _player.position) <= chaseRange;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        Debug.Log($"Grounded: {_isGrounded} | Velocity: {_rb.linearVelocity}"); // Debug log
    }

    void FixedUpdate()
    {
        if(_isChasing && _isGrounded && Time.time > _lastJumpTime + jumpCooldown)
        {
            JumpTowardsPlayer();
            _lastJumpTime = Time.time;
        }
    }

    void JumpTowardsPlayer()
    {
        // Yön hesaplama (güncellenmiş versiyon)
        float xDifference = _player.position.x - transform.position.x;
        int direction = xDifference > 0 ? 1 : -1;

        // Sprite yönü için doğru flip mantığı
        _spriteRenderer.flipX = xDifference > 0; // Düzeltilmiş satır

        // Hareket uygula
        _rb.linearVelocity = new Vector2(direction * horizontalSpeed, jumpForce);

        // Animasyon
        _anim.SetTrigger(AnimatorHashes.JumpBite);
    
        Debug.Log($"X Difference: {xDifference} | Direction: {direction} | FlipX: {_spriteRenderer.flipX}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDeath>()?.Die();
        }
    }
}