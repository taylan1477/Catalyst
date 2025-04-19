using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Footstep Clips")]
    public AudioClip[] slowSteps;
    public AudioClip[] mediumSteps;
    public AudioClip[] fastSteps;
    public AudioClip[] fastestSteps;
    
    [Header("Volume Settings")]
    [Range(0, 1)] public float footstepVolume = 0.3f; // Inspector'dan ayarlanabilir

    AudioSource _sfxSource;
    bool _isFootstepPlaying; // Sesin aktif olarak çalınıp çalmadığını takip et

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var sources = GetComponents<AudioSource>();
        _sfxSource = sources[0];
        _sfxSource.volume = footstepVolume; // Volume'ü başlat
    }

    public void PlayFootstep(float speed)
    {
        StopFootstep();
        AudioClip[] clips = GetSpeedCategory(speed);
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        
        _sfxSource.volume = footstepVolume; // Her ses çalınmadan önce volume güncelle
        _sfxSource.clip = clip;
        _sfxSource.pitch = Random.Range(0.9f, 1.1f);
        _sfxSource.Play();
        _isFootstepPlaying = true;
    }

    public void StopFootstep()
    {
        if(_isFootstepPlaying)
        {
            _sfxSource.Stop();
            _isFootstepPlaying = false;
        }
    }

    private AudioClip[] GetSpeedCategory(float speed)
    {
        if (speed < 5f) return slowSteps;
        if (speed < 8f) return mediumSteps;
        if (speed < 11.9f) return fastSteps;
        return fastestSteps;
    }
}