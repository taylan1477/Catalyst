using UnityEngine;

public class MonsterRat : MonoBehaviour
{
    public float chaseRange = 5f;
    public float moveSpeed = 2f;
    public LayerMask playerLayer;

    private Transform _player;
    private bool _isChasing;
    private Rigidbody2D _rb;
    private Animator _animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        _isChasing = distanceToPlayer <= chaseRange;
    }

    void FixedUpdate()
    {
        if (_isChasing)
        {
            Vector2 direction = (_player.position - transform.position).normalized;
            _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);
            _animator.SetBool(AnimatorHashes.RatWalk, true);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _animator.SetTrigger(AnimatorHashes.RatAttack);
            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                playerDeath.Die();
            }
        }
    }
}