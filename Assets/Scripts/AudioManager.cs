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

    public void PlayFootstep(float speed)
    {
        if (_sfxSource.isPlaying) return; // Önceki ses bitmeden yeni ses çalma

        AudioClip[] clips = GetSpeedCategory(speed);
        AudioClip clip = clips[Random.Range(0, clips.Length)];
    
        _sfxSource.clip = clip;
        _sfxSource.pitch = Random.Range(0.9f, 1.1f);
        _sfxSource.Play();
    }

    private AudioClip[] GetSpeedCategory(float speed)
    {
        if (speed < 5f) return slowSteps;
        if (speed < 8f) return mediumSteps;
        if (speed < 11.9f) return fastSteps;
        return fastestSteps;
    }
}