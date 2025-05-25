using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Main Pause Menu")]
    public GameObject pauseScreen; // ESC ile açılan panel
    public Button continueButton;
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
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);

        // Ayarlar menüsü butonları
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
                CloseSettings(); // Ayarlar açıksa önce onu kapat
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
