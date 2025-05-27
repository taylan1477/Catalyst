using PlayerControllers;
using UnityEngine;

namespace NPCControllers
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
    public class MonsterController : MonoBehaviour
    {
        [Header("Settings")]
        public float chaseRange = 5f;
        public float jumpForce = 10f;
        public float horizontalSpeed = 7f;
        public float jumpCooldown = 2f;
    
        [Header("References")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;
    
        [Header("Death")]
        public float deathAnimationDuration = 1f; // Animasyon süresi

        private bool _ismDead;

        private Transform _player;
        private Rigidbody2D _rb;
        private Animator _anim;
        private SpriteRenderer _spriteRenderer;
        private bool _isGrounded;
        private float _lastJumpTime;
        private bool _isChasing;
    
        [Header("Audio")]
        public AudioClip[] jumpSounds;
        public AudioClip[] deathSounds;

        private AudioSource _audioSource;


        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if(_ismDead) return;
            _isChasing = Vector2.Distance(transform.position, _player.position) <= chaseRange;
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void FixedUpdate()
        {
            if(_ismDead) return;
            if(_isChasing && _isGrounded && Time.time > _lastJumpTime + jumpCooldown)
            {
                JumpTowardsPlayer();
                _lastJumpTime = Time.time;
            }
        }

        void JumpTowardsPlayer()
        {
            // Yön hesaplamaca
            float xDifference = _player.position.x - transform.position.x;
            int direction = xDifference > 0 ? 1 : -1;

            // flip mantığı
            _spriteRenderer.flipX = xDifference > 0; 

            // Hareket uygula
            _rb.linearVelocity = new Vector2(direction * horizontalSpeed, jumpForce);
            
            _anim.SetTrigger(AnimatorHashes.JumpBite);
            if (jumpSounds.Length > 0)
            {
                AudioClip clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
                _audioSource.PlayOneShot(clip);
            }

    
            Debug.Log($"X Difference: {xDifference} | Direction: {direction} | FlipX: {_spriteRenderer.flipX}");
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerDeath>()?.Die();
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Acids") && !_ismDead)
            {
                Die();
            }
        }
    
        void Die()
        {
            _ismDead = true;
        
            // Animasyonu tetikle
            _anim.SetTrigger(AnimatorHashes.MonsterDead);
            // rigidbody kapa
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;
        
            // Collider kapa
            foreach(Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = false;
            }
            if (deathSounds.Length > 0)
            {
                AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];
                _audioSource.PlayOneShot(clip);
            }

            Destroy(gameObject, deathAnimationDuration);
        }
    }
}