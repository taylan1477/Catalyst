using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform respawnPoint; // Checkpoint varsa buraya dön
    public float deathDelay = 1f; // Ölümden sonra bekleme

    private Animator _animator;
    private bool _isDead;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Acids") && !_isDead)
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger(AnimatorHashes.DeadTrigger); // Ölüm animasyonu varsa tetikle
        GetComponent<Rigidbody2D>().simulated = false; // Hareketi durdur

        Invoke(nameof(Respawn), deathDelay);
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
        GetComponent<Rigidbody2D>().simulated = true;
        _isDead = false;
    }
}