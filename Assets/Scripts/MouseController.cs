using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public int health = 3; // Fare canı
    private int _currentWaypointIndex = 0;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isDead = false; // Fare öldü mü?
    private bool _isHurt = false;

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

        if (targetWaypoint.position.x < transform.position.x) // Sola gidiyorsa
        {
            _spriteRenderer.flipX = true; // Sprite'ı sola çevir
        }
        else // Sağa gidiyorsa
        {
            _spriteRenderer.flipX = false; // Sprite'ı sağa çevir
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

        health -= damage; // Canı azalt

        if (health <= 0) // Can sıfır olduysa
        {
            Die();
        }
        else
        {
            // Yaralanma animasyonunu oynat
            _isHurt = true;
            _animator.SetBool("isHurt",_isHurt);
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetBool("isDead", true); // Ölüm animasyonunu oynat
        Destroy(gameObject, 5f); // Fareyi 5 saniye sonra yok et
    }
}