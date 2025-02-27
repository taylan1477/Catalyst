using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public int health = 3; 
    private int _currentWaypointIndex = 0;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isDead = false; 

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!_isDead)
        {
            MoveAlongWaypoints();
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

        _animator.SetBool("isWalking", true);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        health -= damage; 

        if (health <= 0) 
        {
            Die();
        }
        else
        {
            // Yaralanma animasyonunu TRIGGER ile tetikle
            _animator.SetTrigger("isHurt");
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger("isDead"); // Ölüm animasyonu için de TRIGGER kullan
        Destroy(gameObject, 5f); 
        _animator.SetBool("isWalking", false);
    }
}