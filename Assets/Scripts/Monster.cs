using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 5f;
    public float jumpForce = 8f;
    public float horizontalForce = 4f;
    public float jumpCooldown = 2f;
    
    [Header("References")]
    public Transform groundCheck;
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
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Oyuncu takip kontrolü
        _isChasing = Vector2.Distance(transform.position, _player.position) <= chaseRange;
        
        // Zemin kontrolü
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
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
        // Yön hesaplama (daha güvenli versiyon)
        float xDifference = _player.position.x - transform.position.x;
        float direction = xDifference > 0 ? 1 : -1;

        // Zıplama vektörü
        Vector2 jumpVector = new Vector2(direction * horizontalForce, jumpForce);
        _rb.AddForce(jumpVector, ForceMode2D.Impulse);

        // Sprite yönünü doğru şekilde çevirme
        if (xDifference != 0)
        {
            _spriteRenderer.flipX = direction < 0;
        }
    
        // Animasyon tetikleme
        _anim.SetTrigger(AnimatorHashes.JumpBite);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Oyuncuya temas anında ölüm
            collision.gameObject.GetComponent<PlayerDeath>()?.Die();
        }
    }

    // GroundCheck görselleştirme (Scene penceresinde görmek için)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }
}

// _anim.SetBool(AnimatorHashes.JumpBite, true);