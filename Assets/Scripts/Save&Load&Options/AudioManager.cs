using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save_Load_Options
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Footstep Clips")]
        public AudioClip[] slowSteps;
        public AudioClip[] mediumSteps;
        public AudioClip[] fastSteps;
        public AudioClip[] fastestSteps;
    
        [Header("Mouse Clips")]
        public AudioClip[] mousehurt;

        [Header("Volume Settings")]
        [Range(0, 1)] public float footstepVolume = 0.1f;
        [Range(0, 1)] public float mouseVolume = 0.3f;

        AudioSource _sfxSource;
        AudioSource _mouseSource;
        bool _isFootstepPlaying;

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
            _mouseSource = sources[1]; // İkinci audio source
        
            _sfxSource.volume = footstepVolume;
            _mouseSource.volume = mouseVolume;
        }
    
        [Header("Music")]
        public AudioSource musicSource;
    
        [Header("Music Clips")]
        public AudioClip mainMenuMusic;
        public AudioClip chapter1Music;
        public AudioClip chapter2Music;

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene Loaded: " + scene.name);

            switch (scene.name)
            {
                case "MainMenu":
                    PlayMusic(mainMenuMusic, 0.6f);
                    break;
                case "Chapter 1":
                    PlayMusic(chapter1Music, 0.7f);
                    break;
                case "Chapter 2":
                    PlayMusic(chapter2Music, 0.7f);
                    break;
            }
        }

        public void PlayMusic(AudioClip musicClip, float volume = 1f)
        {
            if (musicClip == null) return;

            musicSource.Stop(); // Kesin dursun
            musicSource.clip = musicClip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }
    
        public void PlayMouseHurt()
        {
            if(mousehurt.Length == 0) return;
        
            AudioClip clip = mousehurt[Random.Range(0, mousehurt.Length)];
            _mouseSource.PlayOneShot(clip, mouseVolume);
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
}