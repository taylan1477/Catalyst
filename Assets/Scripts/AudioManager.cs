using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Footstep Clips")]
    public AudioClip[] slowSteps;
    public AudioClip[] mediumSteps;
    public AudioClip[] fastSteps;
    public AudioClip[] fastestSteps;

    AudioSource _sfxSource;

    void Awake()
    {
        // Eğer başka örnek varsa, kendini sil
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ses kaynaklarını ayarla (Inspector’dan da ekleyebilirsin)
        var sources = GetComponents<AudioSource>();
        _sfxSource   = sources[0];
    }

    // Footstep çalma metodu
    public void PlayFootstep(float speed)
    {
        AudioClip[] clips = speed < 5f  ? slowSteps
            : speed < 8f  ? mediumSteps
            : speed < 11.9f ? fastSteps
            : fastestSteps;

        var clip = clips[Random.Range(0, clips.Length)];
        _sfxSource.pitch = Random.Range(0.95f, 1.05f);
        _sfxSource.PlayOneShot(clip);
    }
}