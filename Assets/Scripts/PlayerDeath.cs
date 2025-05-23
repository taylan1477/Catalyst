using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform respawnPoint;
    public float respawnDelay = 1f;
    private Animator _animator;
    private bool _isDead;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator = GetComponent<Animator>();
        _animator.SetTrigger(AnimatorHashes.SpawnTrigger);
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
        _animator.SetTrigger(AnimatorHashes.DeadTrigger);
        GetComponent<Rigidbody2D>().simulated = false;
    }

    // Animation event'ten çağrılacak
    public void OnDeathAnimationEnd()
    {
        StartCoroutine(DelayedRespawn());
    }

    IEnumerator DelayedRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
        GetComponent<Rigidbody2D>().simulated = true;
        _isDead = false;
        _animator.SetTrigger(AnimatorHashes.SpawnTrigger);
    }
}