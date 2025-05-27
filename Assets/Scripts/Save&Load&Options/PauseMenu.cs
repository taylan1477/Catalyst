using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Save_Load_Options
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Main Pause Menu")]
        public GameObject pauseScreen; 
        public Button continueButton;
        public Button saveButton;
        public Button settingsButton;
        public Button quitButton;

        [Header("Settings Menu")]
        public GameObject settingsPanel;
        public AudioMixer mixer;
        public Button backButton;
        public Slider musicSlider;
        public Slider sfxSlider;

        private bool _isPaused;

        void Awake()
        {
            // Menüler başlangıçta kapalı
            pauseScreen.SetActive(false);
            settingsPanel.SetActive(false);
            
            // Pause ekran butonları
            continueButton.onClick.AddListener(ResumeGame);
            saveButton.onClick.AddListener(SaveGame);
            settingsButton.onClick.AddListener(OpenSettings);
            quitButton.onClick.AddListener(QuitGame);
            backButton.onClick.AddListener(CloseSettings);

            // Slider ayarları
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);

            // PlayerPrefs'ten kayıtlı değerleri al ve hem slider'a hem mixere uygula
            float savedMusic = PlayerPrefs.GetFloat("Music", 1f);
            float savedSfx = PlayerPrefs.GetFloat("SFX", 1f);

            musicSlider.value = savedMusic;
            sfxSlider.value = savedSfx;

            SetMusicVolume(savedMusic);
            SetSfxVolume(savedSfx);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else if (_isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        void PauseGame()
        {
            _isPaused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        void ResumeGame()
        {
            _isPaused = false;
            pauseScreen.SetActive(false);
            settingsPanel.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    
        int SceneNameToChapterIndex(string sceneName)
        {
            switch (sceneName)
            {
                case "Chapter1": return 0;
                case "Chapter2": return 1;
                case "Chapter3": return 2;
                case "Chapter4": return 3;
                default: return 0;
            }
        }

        void SaveGame()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player bulunamadı!");
                return;
            }

            Vector3 pos = player.transform.position;
            
            string sceneName = SceneManager.GetActiveScene().name;
            int chapterIndex = SceneNameToChapterIndex(sceneName);

            // Önceki kayıt verisini al, sadece pozisyon ve chapter'ı güncelle
            SaveData data = SaveSystem.LoadGame(GameState.activeSlot) ?? new SaveData();
            data.playerPosition = pos;
            data.chapterIndex = chapterIndex;
            data.sceneName = sceneName;

            SaveSystem.SaveGame(data, GameState.activeSlot);

            Debug.Log($"Oyun kaydedildi: Slot {GameState.activeSlot}, Chapter {chapterIndex}, Pos {pos}");
        }

        void OpenSettings()
        {
            pauseScreen.SetActive(false);
            settingsPanel.SetActive(true);
        }

        void CloseSettings()
        {
            settingsPanel.SetActive(false);
            pauseScreen.SetActive(true);
        }

        void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat("Music", value);
            PlayerPrefs.Save();

            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("Music", dB);
        }

        void SetSfxVolume(float value)
        {
            PlayerPrefs.SetFloat("SFX", value);
            PlayerPrefs.Save();

            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("SFX", dB);
        }

        void QuitGame()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
