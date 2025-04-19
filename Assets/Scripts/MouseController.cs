using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public int health = 3; 
    
    [Header("Audio Settings")]
    [SerializeField] private Vector2 idleSoundInterval;
    
    private int _currentWaypointIndex;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isDead;
    private float _idleTimer;
    private float _nextIdleTime;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _nextIdleTime = Random.Range(idleSoundInterval.x, idleSoundInterval.y);
    }

    void Update()
    {
        if (!_isDead)
        {
            MoveAlongWaypoints();
            HandleIdleSound();
        }
    }

    void MoveAlongWaypoints()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[_currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (targetWaypoint.position.x < transform.position.x) 
        {
            _spriteRenderer.flipX = true; 
        }
        else 
        {
            _spriteRenderer.flipX = false; 
        }

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        }

        _animator.SetBool(AnimatorHashes.Walk, true);
    }

    void HandleIdleSound()
    {
        _idleTimer += Time.deltaTime;
        if(_idleTimer >= _nextIdleTime)
        {
            AudioManager.Instance.PlayMouseIdle();
            _idleTimer = 0f;
            _nextIdleTime = Random.Range(idleSoundInterval.x, idleSoundInterval.y);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        health -= damage;
        
        AudioManager.Instance.PlayMouseHurt();

        if (health <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger(AnimatorHashes.Hurt);
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger(AnimatorHashes.Dead);
        _animator.SetBool(AnimatorHashes.Walk, false);
        AudioManager.Instance.StopMouseSounds();
        
        transform.localScale *= 0.7f;
    }
    
    public bool IsDead()
    {
        return _isDead; // Fare öldü mü?
    }
}