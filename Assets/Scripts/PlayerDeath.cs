using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform respawnPoint;
    public float respawnDelay = 1f;
    public TextMeshProUGUI lifeTextUI;// UI'daki "9x" yazısı

    private Animator _animator;
    private bool _isDead;

    public static int lives = 9; // Oyuncunun toplam canı (oyun boyunca sabit)

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetTrigger(AnimatorHashes.SpawnTrigger);
        UpdateLifeUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Acids") && !_isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        _isDead = true;
        _animator.SetTrigger(AnimatorHashes.DeadTrigger);
        GetComponent<Rigidbody2D>().simulated = false;

        lives = Mathf.Max(0, lives - 1);
        UpdateLifeUI();

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            // GameOver ekranı vs.
        }
    }

    // Animation event'ten çağrılacak
    public void OnDeathAnimationEnd()
    {
        if (lives > 0)
            StartCoroutine(DelayedRespawn());
        else
            Debug.Log("Respawn edilmedi çünkü can bitti.");
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

    void UpdateLifeUI()
    {
        if (lifeTextUI != null)
        {
            lifeTextUI.text = $"{lives}x";
        }
    }
}